//#define DEBUG_CC2D_RAYS

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable EventNeverSubscribedTo.Global

// external character controller
// https://github.com/prime31/CharacterController2D
// works without build-in unity physics
namespace Entity.Movement
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class MovementController : MonoBehaviour
    {
        #region fields
        
        private readonly List<RaycastHit2D> _raycastHitsThisFrame = new List<RaycastHit2D>(2);
        private RaycastHit2D _raycastHit;
        private CharacterRaycastOrigins _raycastOrigins;
        
        private float _horizontalDistanceBetweenRays;
        private float _verticalDistanceBetweenRays;
        private bool _isGoingUpSlope;
        
        #endregion

        #region internal types

        private struct CharacterRaycastOrigins
        {
            public Vector3 TopLeft;
            public Vector3 BottomRight;
            public Vector3 BottomLeft;
        }

        public class CharacterCollisionState2D
        {
            public bool Above;
            public bool Below;
            public bool Left;
            public bool Right;
            
            public float SlopeAngle;
            public bool MovingDownSlope;
            public bool MovingUpSlope;
            
            public bool WasGroundedLastFrame;
            public bool BecameGroundedThisFrame;

            public bool HasCollision() => Below || Right || Left || Above;

            public void Reset()
            {
                Right = Left = Above = Below = BecameGroundedThisFrame = MovingDownSlope = MovingUpSlope = false;
                SlopeAngle = 0f;
            }

            public override string ToString()
            {
                return $"[CharacterCollisionState2D] r: {Right}, l: {Left}, a: {Above}, b: {Below}, movingDownSlope: " +
                       $"{MovingDownSlope}, angle: {SlopeAngle}, wasGroundedLastFrame: {WasGroundedLastFrame}, " +
                       $"becameGroundedThisFrame: {BecameGroundedThisFrame}";
            }
        }

        #endregion internal types

        #region events, properties and fields

        public event Action<RaycastHit2D> OnControllerCollidedEvent;
        public event Action<Collider2D> OnTriggerEnterEvent;
        public event Action<Collider2D> OnTriggerStayEvent;
        public event Action<Collider2D> OnTriggerExitEvent;
        
        public bool ignoreOneWayPlatformsThisFrame;

        [SerializeField] [Range(0.001f, 0.3f)] private float skinWidth = 0.02f;

        /// <summary>
        ///     defines how far in from the edges of the collider rays are cast from.
        ///     If cast with a 0 extent it will often result in ray hits that are not desired
        ///     (for example a foot collider casting horizontally from directly on the surface can result in a hit)
        /// </summary>
        public float SkinWidth
        {
            get => skinWidth;
            set
            {
                skinWidth = value;
                RecalculateDistanceBetweenRays();
            }
        }
        
        public LayerMask platformMask = 0;
        public LayerMask triggerMask = 0;
        public LayerMask oneWayPlatformMask = 0;
        
        [Range(0f, 90f)] public float slopeLimit = 30f;

        /// <summary>
        ///     the threshold in the change in vertical movement between frames that constitutes jumping
        /// </summary>
        /// <value>The jumping threshold.</value>
        public float jumpingThreshold = 0.07f;

        /// <summary>
        ///     curve for multiplying speed based on slope (negative = down slope and positive = up slope)
        /// </summary>
        public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(
                new Keyframe(-90f, 1.5f), 
                new Keyframe(0f, 1f), 
                new Keyframe(90f, 0f)
                );

        [Range(2, 20)] public int totalHorizontalRays = 8;
        [Range(2, 20)] public int totalVerticalRays = 4;

        /// <summary>
        ///     this is used to calculate the downward ray that is cast to check for slopes.
        ///     We use the somewhat arbitrary value 75 degrees to calculate the length of the
        ///     ray that checks for slopes.
        /// </summary>
        private readonly float _slopeLimitTangent = Mathf.Tan(75f * Mathf.Deg2Rad);

        [NonSerialized] public Transform Transform;
        [NonSerialized] public BoxCollider2D BoxCollider;
        [NonSerialized] public readonly CharacterCollisionState2D CollisionState = new CharacterCollisionState2D();
        [NonSerialized] public Vector3 Velocity;
        
        public bool IsGrounded => CollisionState.Below;

        private const float KSkinWidthFloatFudgeFactor = 0.001f;

        #endregion events, properties and fields

        #region Monobehaviour

        private void Awake()
        {
            // add our one-way platforms to our normal platform mask so that we can land on them from above
            platformMask |= oneWayPlatformMask;
            
            Transform = GetComponent<Transform>();
            BoxCollider = GetComponent<BoxCollider2D>();

            // trigger properties setter
            SkinWidth = skinWidth;
        }

        public void OnTriggerEnter2D(Collider2D collider2d) => OnTriggerEnterEvent?.Invoke(collider2d);
        public void OnTriggerStay2D(Collider2D collider2d) => OnTriggerStayEvent?.Invoke(collider2d);
        public void OnTriggerExit2D(Collider2D collider2d) => OnTriggerExitEvent?.Invoke(collider2d);

        #endregion Monobehaviour

        #region Public
        
        public void Move(Vector3 deltaMovement)
        {
            CollisionState.WasGroundedLastFrame = CollisionState.Below;

            // clear states
            CollisionState.Reset();
            _raycastHitsThisFrame.Clear();
            _isGoingUpSlope = false;
            CollisionState.MovingUpSlope = false;

            PrimeRaycastOrigins();

            // check slope below before moving only when grounded and moving down
            if (deltaMovement.y < 0f && CollisionState.WasGroundedLastFrame)
                HandleVerticalSlope(ref deltaMovement);
            // check movement in the horizontal direction
            if (deltaMovement.x != 0f)
                MoveHorizontally(ref deltaMovement);
            // check movement in the vertical direction
            if (deltaMovement.y != 0f)
                MoveVertically(ref deltaMovement);

            // move then update our state
            deltaMovement.z = 0;
            Transform.Translate(deltaMovement, Space.World);

            // only calculate velocity if we have a non-zero deltaTime
            if (Time.deltaTime > 0f)
                Velocity = deltaMovement / Time.deltaTime;

            // set our becameGrounded state based on the previous and current collision state
            if (!CollisionState.WasGroundedLastFrame && CollisionState.Below)
                CollisionState.BecameGroundedThisFrame = true;

            // if we are going up a slope we artificially set a y velocity so we need to zero it out here
            if (_isGoingUpSlope)
                Velocity.y = 0;

            // send off the collision events if we have a listener
            if (OnControllerCollidedEvent != null)
                foreach (var t in _raycastHitsThisFrame)
                    OnControllerCollidedEvent(t);

            ignoreOneWayPlatformsThisFrame = false;
        }

        /// <summary>
        ///     moves directly down until grounded
        /// </summary>
        public void WarpToGrounded()
        {
            do
            {
                Move(new Vector3(0, -1f, 0));
            } while (!IsGrounded);
        }

        /// <summary>
        ///     this should be called anytime you have to modify the BoxCollider2D at runtime.
        ///     It will recalculate the distance between the rays used for collision detection.
        ///     It is also used in the skinWidth setter in case it is changed at runtime.
        /// </summary>
        public void RecalculateDistanceBetweenRays()
        {
            // figure out the distance between our rays in both directions
            // horizontal
            var localScale = Transform.localScale;
            var colliderUsableHeight = BoxCollider.size.y * Mathf.Abs(localScale.y) - 2f * skinWidth;
            _verticalDistanceBetweenRays = colliderUsableHeight / (totalHorizontalRays - 1);

            // vertical
            var colliderUsableWidth = BoxCollider.size.x * Mathf.Abs(localScale.x) - 2f * skinWidth;
            _horizontalDistanceBetweenRays = colliderUsableWidth / (totalVerticalRays - 1);
        }

        #endregion Public

        #region Movement Methods

        /// <summary>
        ///     resets the raycastOrigins to the current extents of the box collider inset by the skinWidth.
        ///     It is inset to avoid casting a ray from a position directly touching another collider
        ///     which results in wonky normal data.
        /// </summary>
        private void PrimeRaycastOrigins()
        {
            // our raycasts need to be fired from the bounds inset by the skinWidth
            var modifiedBounds = BoxCollider.bounds;
            modifiedBounds.Expand(-2f * skinWidth);

            _raycastOrigins.TopLeft = new Vector2(modifiedBounds.min.x, modifiedBounds.max.y);
            _raycastOrigins.BottomRight = new Vector2(modifiedBounds.max.x, modifiedBounds.min.y);
            _raycastOrigins.BottomLeft = modifiedBounds.min;
        }

        /// <summary>
        ///     (Little trickery) The rays must be cast from a small distance inside of our collider (skinWidth)
        ///     to avoid zero distance rays which will get the wrong normal.
        ///     Because of this small offset we have to increase the ray distance skinWidth then remember
        ///     to remove skinWidth from deltaMovement before actually moving the player
        /// </summary>
        private void MoveHorizontally(ref Vector3 deltaMovement)
        {
            var isGoingRight = deltaMovement.x > 0;
            var rayDistance = Mathf.Abs(deltaMovement.x) + skinWidth;
            var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
            var initialRayOrigin = isGoingRight ? _raycastOrigins.BottomRight : _raycastOrigins.BottomLeft;

            for (var i = 0; i < totalHorizontalRays; i++)
            {
                var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * _verticalDistanceBetweenRays);
                DrawRay(ray, rayDirection * rayDistance, Color.red);

                // if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one).
                // this will allow us to walk up sloped oneWayPlatforms
                if (i == 0 && CollisionState.WasGroundedLastFrame)
                {
                    _raycastHit = Physics2D.Raycast(
                        ray,
                        rayDirection,
                        rayDistance,
                        platformMask
                    );
                }
                else
                {
                    _raycastHit = Physics2D.Raycast(
                        ray,
                        rayDirection, 
                        rayDistance, 
                        platformMask & ~oneWayPlatformMask
                        );
                }

                if (_raycastHit)
                {
                    var handled = i == 0 && HandleHorizontalSlope(
                        ref deltaMovement,
                        Vector2.Angle(_raycastHit.normal, Vector2.up)
                        );
                    
                    // the bottom ray can hit a slope but no other ray can so we have special handling for these cases
                    if (handled)
                    {
                        _raycastHitsThisFrame.Add(_raycastHit);
                        // if we weren't grounded last frame, that means we're landing on a slope horizontally.
                        // this ensures that we stay flush to that slope
                        if (!CollisionState.WasGroundedLastFrame)
                        {
                            var flushDistance = Mathf.Sign(deltaMovement.x) * (_raycastHit.distance - SkinWidth);
                            Transform.Translate(new Vector2(flushDistance, 0));
                        }
                        break;
                    }

                    // set our new deltaMovement and recalculate the rayDistance taking it into account
                    deltaMovement.x = _raycastHit.point.x - ray.x;
                    rayDistance = Mathf.Abs(deltaMovement.x);

                    // remember to remove the skinWidth from our deltaMovement
                    if (isGoingRight)
                    {
                        deltaMovement.x -= skinWidth;
                        CollisionState.Right = true;
                    }
                    else
                    {
                        deltaMovement.x += skinWidth;
                        CollisionState.Left = true;
                    }

                    _raycastHitsThisFrame.Add(_raycastHit);
                    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
                    // than the width + fudge bail out because we have a direct impact
                    if (rayDistance < skinWidth + KSkinWidthFloatFudgeFactor)
                        break;
                }
            }
        }

        /// <summary>
        ///     handles adjusting deltaMovement if we are going up a slope.
        /// </summary>
        /// <returns><c>true</c>, if horizontal slope was handled, <c>false</c> otherwise.</returns>
        /// <param name="deltaMovement">Delta movement.</param>
        /// <param name="angle">Angle.</param>
        private bool HandleHorizontalSlope(ref Vector3 deltaMovement, float angle)
        {
            // disregard 90 degree angles (walls)
            if (Mathf.RoundToInt(angle) == 90)
                return false;

            // if we can walk on slopes and our angle is small enough we need to move up
            if (angle < slopeLimit)
            {
                // we only need to adjust the deltaMovement if we are not jumping
                // TODO: magic number isn't ideal better to have the user pass in if there is a jump this frame
                if (deltaMovement.y < jumpingThreshold)
                {
                    // apply the slopeModifier to slow our movement up the slope
                    var slopeModifier = slopeSpeedMultiplier.Evaluate(angle);
                    deltaMovement.x *= slopeModifier;

                    // we dont set collisions on the sides for this since a slope is not technically a side collision.
                    // smooth y movement when we climb. we make the y movement equivalent to the actual y location
                    // that corresponds to our new x location using our good friend Pythagoras
                    deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
                    var isGoingRight = deltaMovement.x > 0;

                    // safety check. we fire a ray in the direction of movement just in case the diagonal we calculated
                    // above ends u going through a wall. if the ray hits, we back off the horizontal movement to stay
                    // in bounds.
                    var ray = isGoingRight ? _raycastOrigins.BottomRight : _raycastOrigins.BottomLeft;
                    RaycastHit2D raycastHit;
                    if (CollisionState.WasGroundedLastFrame)
                    {
                        raycastHit = Physics2D.Raycast(
                            ray,
                            deltaMovement.normalized,
                            deltaMovement.magnitude,
                            platformMask
                        );
                    }
                    else
                    {
                        raycastHit = Physics2D.Raycast(
                            ray,
                            deltaMovement.normalized,
                            deltaMovement.magnitude,
                            platformMask & ~oneWayPlatformMask
                        );
                    }

                    if (raycastHit)
                    {
                        // we crossed an edge when using Pythagoras calculation,
                        // so we set the actual delta movement to the ray hit location
                        deltaMovement = (Vector3) raycastHit.point - ray;
                        if (isGoingRight)
                            deltaMovement.x -= skinWidth;
                        else
                            deltaMovement.x += skinWidth;
                    }

                    _isGoingUpSlope = true;
                    CollisionState.MovingUpSlope = true;
                    CollisionState.Below = true;
                    CollisionState.SlopeAngle = -angle;
                }
            }
            else // too steep
            {
                deltaMovement.x = 0;
            }

            return true;
        }

        private void MoveVertically(ref Vector3 deltaMovement)
        {
            var isGoingUp = deltaMovement.y > 0;
            var rayDistance = Mathf.Abs(deltaMovement.y) + skinWidth;
            var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
            var initialRayOrigin = isGoingUp ? _raycastOrigins.TopLeft : _raycastOrigins.BottomLeft;

            // apply our horizontal deltaMovement here so that we do our raycast from the actual position
            // we would be in if we had moved
            initialRayOrigin.x += deltaMovement.x;

            // if we are moving up, we should ignore the layers in oneWayPlatformMask
            var mask = platformMask;
            if (isGoingUp && !CollisionState.WasGroundedLastFrame || ignoreOneWayPlatformsThisFrame)
                mask &= ~oneWayPlatformMask;

            for (var i = 0; i < totalVerticalRays; i++)
            {
                var ray = new Vector2(initialRayOrigin.x + i * _horizontalDistanceBetweenRays, initialRayOrigin.y);
                DrawRay(ray, rayDirection * rayDistance, Color.red);
                
                _raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, mask);
                if (_raycastHit)
                {
                    // set our new deltaMovement and recalculate the rayDistance taking it into account
                    deltaMovement.y = _raycastHit.point.y - ray.y;
                    rayDistance = Mathf.Abs(deltaMovement.y);

                    // remember to remove the skinWidth from our deltaMovement
                    if (isGoingUp)
                    {
                        deltaMovement.y -= skinWidth;
                        CollisionState.Above = true;
                    }
                    else
                    {
                        deltaMovement.y += skinWidth;
                        CollisionState.Below = true;
                    }

                    _raycastHitsThisFrame.Add(_raycastHit);

                    // this is a hack to deal with the top of slopes. if we walk up a slope and reach
                    // the apex we can get in a situation where our ray gets a hit that is less then
                    // skinWidth causing us to be ungrounded the next frame due to residual velocity.
                    if (!isGoingUp && deltaMovement.y > 0.00001f)
                    {
                        CollisionState.MovingUpSlope = true;
                        _isGoingUpSlope = true;
                    }

                    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
                    // than the width + fudge bail out because we have a direct impact
                    if (rayDistance < skinWidth + KSkinWidthFloatFudgeFactor) break;
                }
            }
        }

        /// <summary>
        ///     checks the center point under the BoxCollider2D for a slope.
        ///     If it finds one then the deltaMovement is adjusted so that
        ///     the player stays grounded and the slopeSpeedModifier is taken
        ///     into account to speed up movement.
        /// </summary>
        /// <param name="deltaMovement">Delta movement.</param>
        private void HandleVerticalSlope(ref Vector3 deltaMovement)
        {
            // slope check from the center of our collider
            var centerOfCollider = (_raycastOrigins.BottomLeft.x + _raycastOrigins.BottomRight.x) * 0.5f;
            var rayDirection = -Vector2.up;

            // the ray distance is based on our slopeLimit
            var slopeCheckRayDistance = _slopeLimitTangent * (_raycastOrigins.BottomRight.x - centerOfCollider);

            var slopeRay = new Vector2(centerOfCollider, _raycastOrigins.BottomLeft.y);
            DrawRay(slopeRay, rayDirection * slopeCheckRayDistance, Color.yellow);
            _raycastHit = Physics2D.Raycast(slopeRay, rayDirection, slopeCheckRayDistance, platformMask);
            if (_raycastHit)
            {
                // bail out if we have no slope
                var angle = Vector2.Angle(_raycastHit.normal, Vector2.up);
                if (angle == 0)
                    return;

                // we are moving down the slope if our normal and movement direction are in the same x direction
                var isMovingDownSlope = 
                    Math.Abs(Mathf.Sign(_raycastHit.normal.x) - Mathf.Sign(deltaMovement.x)) < 0.00001f;
                if (isMovingDownSlope)
                {
                    // going down we want to speed up in most cases so the slopeSpeedMultiplier curve should be > 1
                    // for negative angles
                    var slopeModifier = slopeSpeedMultiplier.Evaluate(-angle);
                    // we add the extra downward movement here to ensure we "stick" to the surface below
                    deltaMovement.y += _raycastHit.point.y - slopeRay.y - SkinWidth;
                    deltaMovement = new Vector3(0, deltaMovement.y, 0) +
                                    Quaternion.AngleAxis(-angle, Vector3.forward) *
                                    new Vector3(deltaMovement.x * slopeModifier, 0, 0);
                    CollisionState.MovingDownSlope = true;
                    CollisionState.SlopeAngle = angle;
                }
            }
        }

        #endregion Movement Methods.
        
        [Conditional("DEBUG_CC2D_RAYS")]
        private static void DrawRay(Vector3 start, Vector3 dir, Color color) => Debug.DrawRay(start, dir, color);
    }
}