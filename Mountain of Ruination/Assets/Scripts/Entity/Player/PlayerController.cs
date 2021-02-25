using System;
using Damage;
using Entity.Movement;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Entity.Player
{
    [RequireComponent(typeof(MovementController))]
    public class PlayerController : MonoBehaviour
    {
        #region Fields and properties

        #region Unity assigns

        [Header("Horizontal Movement")] 
        public float moveSpeed;
        public float backwardsSpeed;

        [Header("Vertical Movement")] 
        public float jumpHeight;
        public float jumpManualDumping;
        public float gravity;
        public float slopeMoveUpdateDelay = 0.1f;

        [Header("Aim")] 
        public AimCircle actualAim;
        public AimCircle targetAim;
        [FormerlySerializedAs("CircleSpeedCenter")]
        public float circleSpeedCenter;
        [FormerlySerializedAs("CircleSpeedEdge")]
        public float circleSpeedEdge;

        [Header("External")] 
        public PlayerManager manager;

        #endregion
        
        #region Input

        private float MoveX { get; set; }
        private Vector2 MouseDelta { get; set; }
        private bool ToJump { get; set; }
        private bool ToContinueJump { get; set; }
        private bool IsAiming
        {
            get => _isAiming;
            set
            {
                WasAiming = _isAiming;
                _isAiming = value;
            }
        }
        private bool WasAiming { get; set; }
        private bool ToAttack { get; set; }

        #endregion

        private bool _isAiming;
        private bool _wasMovingSlope;
        private bool _wasToContinueJump;

        private float _wasMovingSlopeTime;
        private Vector2 _velocity;
        private MovementController _movement;
        
        // animation hashes
        private static readonly int VelocityScaleX = Animator.StringToHash("velocityScaleX");
        private static readonly int VelocityY = Animator.StringToHash("velocityY");
        private static readonly int InFall = Animator.StringToHash("inFall");
        private static readonly int Aiming = Animator.StringToHash("isAiming");
        private static readonly int WeaponX = Animator.StringToHash("weaponX");
        private static readonly int ToAim = Animator.StringToHash("toAim");
        private static readonly int ToAttackPierce = Animator.StringToHash("toAttackPierce");
        private static readonly int ToAttackLight = Animator.StringToHash("toAttackLight");
        private static readonly int ToAttackHeavy = Animator.StringToHash("toAttackHeavy");
        
        public bool IsFacingRight { get; private set; }
        public bool DisplayActualAim { get; set; }
        public bool IsLocked { get; set; }
        private bool IsMovingBackwards
        {
            get
            {
                var isBackwards = MoveX > 0 && !IsFacingRight || MoveX < 0 && IsFacingRight;
                return IsAiming && isBackwards;
            }
        }
        private bool IsGrounded
        {
            get
            {
                var grounded = _wasMovingSlope || _movement.IsGrounded;
                
                if (_wasMovingSlope == false || Time.time - _wasMovingSlopeTime > slopeMoveUpdateDelay)
                {
                    _wasMovingSlopeTime = Time.time;
                    _wasMovingSlope = _movement.CollisionState.MovingDownSlope 
                                      || _movement.CollisionState.MovingUpSlope;
                }

                return grounded;
            }
        }
        

        #endregion

        #region Unity calls

        private void Start()
        {
            _movement = GetComponent<MovementController>();

            targetAim.ChangePosition(new Vector2(1, 0));
            Cursor.lockState = CursorLockMode.Locked;
            DisplayActualAim = true;

            IsFacingRight = true;
        }

        private void Update()
        {
            GetControls();
            UpdateMovement();
            if (IsLocked) return;
            UpdateDirection();
            UpdateCombatState();
            UpdateAimPositions();
            UpdateAnimation();
        }

        #endregion

        #region Update parts

        private void GetControls()
        {
            MoveX = InputUtil.GetMove().x;
            MouseDelta = InputUtil.GetMousePositionDelta();
            ToJump = InputUtil.GetJump();
            ToContinueJump = InputUtil.GetContinuousJump();
            IsAiming ^= InputUtil.GetCombatMode();
            ToAttack = InputUtil.GetAttack();

            if (!IsAiming && ToAttack)
            {
                IsAiming = true;
                ToAttack = false;
            }
        }

        // handles character movement and jumping using movement controller
        private void UpdateMovement()
        {
            if (!IsLocked)
            {
                if (ToJump && IsGrounded)
                {
                    _velocity.y = Mathf.Sqrt(2f * jumpHeight * gravity);
                    _wasToContinueJump = true;
                }

                if (_wasToContinueJump && !ToContinueJump)
                {
                    if (_velocity.y > 0)
                    {
                        _velocity.y -= jumpManualDumping;
                        _velocity.y = _velocity.y < 0 ? 0 : _velocity.y;
                    }

                    _wasToContinueJump = false;
                }

                var speed = IsMovingBackwards ? backwardsSpeed : moveSpeed;
                _velocity.x = speed * MoveX;
            }
            _velocity.y -= gravity * Time.deltaTime; // (m/s^2)

            _movement.Move(_velocity * Time.deltaTime);
            _velocity = _movement.Velocity;
        }

        // flips character is it's necessary
        private void UpdateDirection()
        {
            if (!IsAiming)
            {
                var input = InputUtil.GetMove();
                if (input.x > 0 && !IsFacingRight || input.x < 0 && IsFacingRight)
                    FlipDirection();
            }

            if (IsAiming && InputUtil.GetSpecialAbility())
            {
                var targetX = targetAim.Position.x;
                if (targetX > 0.1 && !IsFacingRight || targetX < -0.1 && IsFacingRight)
                {
                    FlipDirection();
                    actualAim.FlipX();
                }
            }
        }

        // starts attack and handles aim visibility
        private void UpdateCombatState()
        {
            if (IsAiming && ToAttack && !manager.weapon.isInAttack)
                StartAttack();
            MoveTargetAim();
            SwitchActualAim(IsAiming);
        }

        private void UpdateAimPositions()
        {
            actualAim.CalculatePosition();
            targetAim.CalculatePosition();
        }

        private void UpdateAnimation()
        {
            manager.animator.SetFloat(VelocityScaleX, GetHorizontalMoveScale());
            manager.animator.SetFloat(VelocityY, _velocity.y);
            manager.animator.SetBool(InFall, !IsGrounded);

            manager.animator.SetBool(Aiming, IsAiming);
            manager.animator.SetFloat(WeaponX, actualAim.Position.x * (IsFacingRight ? 1 : -1));
            if (IsAiming && !WasAiming)
                manager.animator.SetTrigger(ToAim);
        }

        #endregion

        #region Support methods

        private void FlipDirection()
        {
            IsFacingRight = !IsFacingRight;
            transform.forward *= -1;
        }

        // returns movement scale from -1 to 1
        // depends on current speed and direction
        private float GetHorizontalMoveScale()
        {
            float velocityScale;

            if (IsMovingBackwards)
                velocityScale = -Math.Abs(_velocity.x / backwardsSpeed);
            else
                velocityScale = Math.Abs(_velocity.x / moveSpeed);

            return velocityScale;
        }

        // start different attack types
        // that depends on aim position
        private void StartAttack()
        {
            if (actualAim.Position.x > 0.7071f && IsFacingRight
                || actualAim.Position.x < -0.7071f && !IsFacingRight)
            {
                manager.animator.SetTrigger(ToAttackPierce);
                manager.weapon.Type = DamageType.PierceDamage;
            }
            else if (actualAim.Position.x > -0.7071f && IsFacingRight 
                     || actualAim.Position.x < 0.7071f && !IsFacingRight)
            {
                manager.animator.SetTrigger(ToAttackLight);
                manager.weapon.Type = DamageType.LightDamage;
            }
            else
            {
                manager.animator.SetTrigger(ToAttackHeavy);
                manager.weapon.Type = DamageType.HeavyDamage;
            }
        }

        private void MoveTargetAim()
        {
            var speed = Mathf.Lerp(circleSpeedCenter, circleSpeedEdge, Mathf.Abs(targetAim.Position.x));
            targetAim.ChangePosition(MouseDelta * (speed * Time.deltaTime));
        }

        // handles actual aim visibility
        private void SwitchActualAim(bool enable)
        {
            actualAim.SpriteEnabled = enable && DisplayActualAim;
            actualAim.IsLocked = !enable;
        }

        #endregion
    }
}