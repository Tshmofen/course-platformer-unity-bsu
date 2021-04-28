using System;
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
        private static readonly int HashToEvade = Animator.StringToHash("toEvade");

        #endregion

        #region Unity assigns

        [Header("Horizontal Movement")] 
        public float moveSpeed = 4.5f;
        public float backwardsSpeed = 2.5f;
        public float evadeSpeed = 7;
        public float waitForEvadeTime = 0.3f;
        [Header("Vertical Movement")] 
        public float jumpHeight = 3;
        public float jumpManualDumping = 5;
        public float gravity = 20;
        public float slopeMoveUpdateDelay = 0.1f;
        public int maxAttacksInFly = 1;
        public int maxEvadesInFly;
        [Header("External")] 
        public PlayerManager manager;
        public bool isInAttack;
        public bool isLocked;
        public bool isEvading;

        #endregion

        #region Input

        private float MoveX { get; set; }
        private bool ToJump { get; set; }
        private bool ToContinueJump { get; set; }
        private bool ToEvade { get; set; }
        private bool ToIgnorePlatform { get; set; }
        private bool ToInteract { get; set; }
        private bool ToAttack { get; set; }
        private bool ToParry { get; set; }
        private bool IsInParryMode { get; set; }

        #endregion

        private bool _wasToContinueJump;
        private bool _isFacingRight;
        private bool _playJumpAnimation;
        private bool _notMoveThisFrame;
        
        private bool _wasMovingSlope;
        private float _wasMovingSlopeTime;
        
        private bool _wasEvading;
        private bool _isEvadingForbidden;
        private float _evadeForbidTime;
        private int _evadesInFly;

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
            // when game on pause
            if (Time.deltaTime == 0) return;
            
            GetControls();
            UpdateCounters();
            CorrectControls();
            UpdateMovement();
            if (isLocked) return;
            UpdateDirection();
            UpdateTimer();
            UpdateCombatState();
            UpdateAnimation();
        }

        #endregion
        
        #region Support methods
        
        private void GetControls()
        {
            MoveX = InputUtil.GetMove().x;
            ToJump = InputUtil.GetJump();
            ToContinueJump = InputUtil.GetContinuousJump();
            ToEvade = InputUtil.GetEvade();
            
            ToIgnorePlatform = InputUtil.GetIgnorePlatform() ;
            ToInteract ^= InputUtil.GetInteract();

            ToAttack = InputUtil.GetAttack();
            ToParry = InputUtil.GetParry();
            IsInParryMode ^= InputUtil.GetCombatMode();
        }

        private void UpdateCounters()
        {
            switch (_movement.IsGrounded)
            {
                case false when (ToAttack || ToParry):
                    _attacksInFly++;
                    break;
                case false when ToEvade:
                    _evadesInFly++;
                    break;
                case true:
                    _attacksInFly = 0;
                    _evadesInFly = 0;
                    break;
            }
        }

        private void CorrectControls()
        {
            if (isInAttack || _attacksInFly > maxAttacksInFly)
                ToAttack = ToParry = false;
            if (_isEvadingForbidden || isEvading || _evadesInFly > maxEvadesInFly)
                ToEvade = false;
        }

        // handles character movement and jumping using movement controller
        private void UpdateMovement()
        {
            _velocity.y -= gravity * Time.deltaTime; // (m/s^2)
            
            if (isLocked)
                UseLockedMovement();
            else if (isInAttack)
                UseAttackMovement();
            else if (isEvading)
                UseEvadeMovement();
            else
                UseUsualMovement();

            if (_notMoveThisFrame)
            {
                _notMoveThisFrame = false;
                return;
            }
            
            var move = (Vector3)_velocity * Time.deltaTime;
            _movement.Move(move);
            _velocity = _movement.Velocity;
        }

        // flips character is it's necessary
        private void UpdateDirection()
        {
            if (isInAttack || isEvading) return;
            var input = InputUtil.GetMove();
            if (input.x > 0 && !_isFacingRight || input.x < 0 && _isFacingRight)
                FlipDirection();
        }

        private void UpdateTimer()
        {
            if (_wasEvading && !isEvading)
            {
                _isEvadingForbidden = true;
                if (_evadeForbidTime < waitForEvadeTime)
                    _evadeForbidTime += Time.deltaTime;
                else
                {
                    _evadeForbidTime = 0;
                    _isEvadingForbidden = false;
                    _wasEvading = false;
                }
            }
        }

        // starts attack
        private void UpdateCombatState()
        {
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

            if (ToEvade && !isInAttack)
                manager.animator.SetTrigger(HashToEvade);
            
            // crutch: turn off jump trigger manually after animation start
            if (!_playJumpAnimation && manager.animator.GetBool(HashToJump)) 
                manager.animator.SetBool(HashToJump, false);
            if (_playJumpAnimation)
            {
                _playJumpAnimation = false; 
                manager.animator.SetTrigger(HashToJump);
            }
        }

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

        #region Movement behaviour
        
        private void UseUsualMovement()
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

        private void UseLockedMovement()
        {
            _velocity.x = 0;
        }

        private void UseAttackMovement()
        {
            _velocity = Vector2.zero;
            _notMoveThisFrame = true;
        }

        private void UseEvadeMovement()
        {
            var minus = (_isFacingRight) ? 1 : -1;
            _velocity = new Vector2(minus * evadeSpeed, 0);
            _wasEvading = true;
        }
        
        #endregion

        #endregion
    }
}