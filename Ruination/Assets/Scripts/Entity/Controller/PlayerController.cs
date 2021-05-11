using UnityEngine;
using Util;

namespace Entity.Controller
{
    public class PlayerController : BaseEntityController
    {
        #region Fields and properties

        #region Unity assigns
        
        [Header("Vertical Movement")] 
        public float jumpHeight = 3;
        public float jumpManualDumping = 5;
        public int maxAttacksInFly = 1;
        public int maxEvadesInFly;
        [Header("Evading")]
        public float evadeSpeed = 7;
        public float waitForEvadeTime = 0.3f;
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
        
        private Vector2 _velocity;
        
        private bool _wasToContinueJump;
        private bool _playJumpAnimation;
        private bool _notMoveThisFrame;

        private bool _wasEvading;
        private bool _isEvadingForbidden;
        private float _evadeForbidTime;
        private int _evadesInFly;

        private bool _toAttackLight;
        private bool _toAttackHeavy;
        private int _attacksInFly;

        public bool IsInteracting
        {
            get => !isInAttack && ToInteract;
            set => ToInteract = value;
        }

        #endregion

        #region Unity behaviour

        protected override void Start()
        {
            base.Start();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            // when game on pause
            if (Time.deltaTime == 0) return;
            
            GetControls();
            UpdateCounters();
            CorrectControls();
            UpdateMovement();
            if (!isLocked)
            {
                UpdateDirection();
                UpdateTimer();
                UpdateCombatState();
            }
            UpdateAnimation();
        }

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
            switch (Movement.IsGrounded)
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
            Movement.Move(move);
            _velocity = Movement.Velocity;
        }

        // flips character is it's necessary
        private void UpdateDirection()
        {
            if (isInAttack || isEvading) return;
            var input = InputUtil.GetMove();
            if (input.x > 0 && !IsFacingRight || input.x < 0 && IsFacingRight)
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
            _toAttackHeavy = ToParry;
            _toAttackLight = !ToParry && ToAttack;
            
            if (isInAttack || _attacksInFly > maxAttacksInFly)
            {
                _toAttackHeavy = false;
                _toAttackLight = false;
            }
        }

        private void UpdateAnimation()
        {
            var velocityScaleX = GetMoveScale(_velocity.x, moveSpeed);
            var velocityScaleY = _velocity.y;
            var inFall = !IsGrounded;
            var inAttack = isInAttack;
            var toEvade = ToEvade && !isInAttack;
            var toJump = _playJumpAnimation;
            var toAttackLight = _toAttackLight;
            var toAttackHeavy = _toAttackHeavy;

            SetAnimationState(
                velocityScaleX, velocityScaleY, inFall,
                inAttack, toAttackLight, toAttackHeavy,
                toJump, toEvade
                );
        }

        #region Movement behaviour
        
        private void UseUsualMovement()
        {
            Movement.ignoreOneWayPlatformsThisFrame = ToIgnorePlatform;

            _playJumpAnimation = ToJump && IsGrounded;
            if (_playJumpAnimation)
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
            
            _velocity.x = moveSpeed * MoveX;
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
            var minus = (IsFacingRight) ? 1 : -1;
            _velocity = new Vector2(minus * evadeSpeed, 0);
            _wasEvading = true;
        }
        
        #endregion

        #endregion
    }
}