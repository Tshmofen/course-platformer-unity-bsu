using System;
using Damage;
using Entity.Movement;
using UnityEngine;
using Util;

namespace Entity.Player
{
    [RequireComponent(typeof(MovementController))]
    public class PlayerController : MonoBehaviour
    {
        #region Fields and properties

        #region Hashes

        // animation hashes
        private static readonly int HashVelocityScaleX = Animator.StringToHash("velocityScaleX");
        private static readonly int HashVelocityY = Animator.StringToHash("velocityY");
        private static readonly int HashInFall = Animator.StringToHash("inFall");
        private static readonly int HashToAttackLight = Animator.StringToHash("toAttackLight");
        private static readonly int HashToAttackHeavy = Animator.StringToHash("toAttackHeavy");
        private static readonly int HashToJump = Animator.StringToHash("toJump");
        private static readonly int HashInAttack = Animator.StringToHash("inAttack");

        #endregion

        #region Unity assigns

        [Header("Horizontal Movement")] public float moveSpeed;
        public float backwardsSpeed;
        [Header("Vertical Movement")] public float jumpHeight;
        public float jumpManualDumping;
        public float gravity;
        public float slopeMoveUpdateDelay = 0.1f;
        public int maxAttacksInFly = 1;
        [Header("External")] 
        public PlayerManager manager;
        public bool isInAttack;
        public bool isLocked;

        #endregion

        #region Input

        private float MoveX { get; set; }
        private bool ToJump { get; set; }
        private bool ToContinueJump { get; set; }
        private bool ToIgnorePlatform { get; set; }
        private bool ToInteract { get; set; }
        private bool ToAttack { get; set; }
        private bool ToParry { get; set; }
        private bool IsInParryMode { get; set; }

        #endregion
        
        private bool _wasMovingSlope;
        private bool _wasToContinueJump;
        private bool _isFacingRight;
        private bool _playJumpAnimation;

        private float _wasMovingSlopeTime;
        private Vector2 _velocity;
        private MovementController _movement;
        private int _attacksInFly;

        public bool IsInteracting
        {
            get => !isInAttack && ToInteract;
            set => ToInteract = value;
        }
        private bool IsMovingBackwards
        {
            get
            {
                var isBackwards = MoveX > 0 && !_isFacingRight || MoveX < 0 && _isFacingRight;
                return isInAttack && isBackwards;
            }
        }
        private bool IsGrounded
        {
            get
            {
                var grounded = _wasMovingSlope || _movement.IsGrounded;

                if (_movement.CollisionState.MovingDownSlope || _movement.CollisionState.MovingUpSlope)
                {
                    _wasMovingSlope = true;
                    _wasMovingSlopeTime = Time.time;
                }
                else if (Time.time - _wasMovingSlopeTime > slopeMoveUpdateDelay)
                {
                    _wasMovingSlope = false;
                }

                return grounded;
            }
        }

        #endregion

        #region Unity calls

        private void Start()
        {
            _movement = GetComponent<MovementController>();
            Cursor.lockState = CursorLockMode.Locked;
            _isFacingRight = true;
        }

        private void Update()
        {
            if (Time.deltaTime == 0) return;
            GetControls();
            UpdateMovement();
            if (isLocked) return;
            UpdateDirection();
            UpdateCombatState();
            UpdateAnimation();
        }

        #endregion

        #region Update parts

        private void GetControls()
        {
            MoveX = InputUtil.GetMove().x;
            ToJump = InputUtil.GetJump();
            ToContinueJump = InputUtil.GetContinuousJump();
            ToIgnorePlatform = InputUtil.GetIgnorePlatform() ;
            ToInteract ^= InputUtil.GetInteract();
            
            ToAttack = InputUtil.GetAttack();
            ToParry = InputUtil.GetParry();
            IsInParryMode ^= InputUtil.GetCombatMode();
        }

        // handles character movement and jumping using movement controller
        private void UpdateMovement()
        {
            if (!isLocked && !isInAttack)
            {
                _movement.ignoreOneWayPlatformsThisFrame = ToIgnorePlatform;

                if (ToJump && IsGrounded)
                {
                    _playJumpAnimation = true;
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

                var speed = (IsMovingBackwards)? backwardsSpeed : moveSpeed;
                _velocity.x = speed * MoveX;
            }
            _velocity.y -= gravity * Time.deltaTime; // (m/s^2)
            
            if (isLocked)
            {
                _velocity.x = 0;
            }
            else if (isInAttack)
            {
                _velocity = Vector2.zero;
                return;
            }

            var move = (Vector3)_velocity * Time.deltaTime;
            _movement.Move(move);
            _velocity = _movement.Velocity;
        }

        // flips character is it's necessary
        private void UpdateDirection()
        {
            var input = InputUtil.GetMove();
            if (!isInAttack && input.x > 0 && !_isFacingRight || input.x < 0 && _isFacingRight)
                FlipDirection();
        }

        // starts attack
        private void UpdateCombatState()
        {
            if (!_movement.IsGrounded && (ToAttack || ToParry))
                _attacksInFly++;
            if (_movement.IsGrounded)
                _attacksInFly = 0;
            if (_attacksInFly > maxAttacksInFly)
                return;
            
            if (ToParry)
                manager.animator.SetTrigger(HashToAttackHeavy);
            else if (ToAttack)
                manager.animator.SetTrigger(HashToAttackLight);
        }

        private void UpdateAnimation()
        {
            manager.animator.SetFloat(HashVelocityScaleX, GetHorizontalMoveScale());
            manager.animator.SetFloat(HashVelocityY, _velocity.y);
            manager.animator.SetBool(HashInFall, !IsGrounded);
            manager.animator.SetBool(HashInAttack, isInAttack);

            if (!_playJumpAnimation && manager.animator.GetBool(HashToJump)) 
                manager.animator.SetBool(HashToJump, false);
            if (_playJumpAnimation)
            {
                _playJumpAnimation = false; 
                manager.animator.SetTrigger(HashToJump);
            }
        }

        #endregion
        
        #region Support methods

        private void FlipDirection()
        {
            _isFacingRight = !_isFacingRight;
            transform.forward *= -1;
            manager.weapon.transform.forward *= -1;
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

        #endregion
    }
}