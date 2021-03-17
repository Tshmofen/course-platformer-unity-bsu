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

        // animation hashes
        private static readonly int HashVelocityScaleX = Animator.StringToHash("velocityScaleX");
        private static readonly int HashVelocityY = Animator.StringToHash("velocityY");
        private static readonly int HashInFall = Animator.StringToHash("inFall");
        private static readonly int HashAiming = Animator.StringToHash("isAiming");
        private static readonly int HashWeaponX = Animator.StringToHash("weaponX");
        private static readonly int HashToAim = Animator.StringToHash("toAim");
        private static readonly int HashToAttackPierce = Animator.StringToHash("toAttackPierce");
        private static readonly int HashToAttackLight = Animator.StringToHash("toAttackLight");
        private static readonly int HashToAttackHeavy = Animator.StringToHash("toAttackHeavy");
        
        #region Unity assigns

        [Header("Horizontal Movement")] 
        public float moveSpeed;
        public float backwardsSpeed;

        [Header("Vertical Movement")] 
        public float jumpHeight;
        public float jumpManualDumping;
        public float gravity;
        public float slopeMoveUpdateDelay = 0.1f;

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
        private bool _isFacingRight;

        private float _wasMovingSlopeTime;
        private Vector2 _velocity;
        private MovementController _movement;

        private float _weaponX;

        public bool IsLocked { get; set; }
        private bool IsMovingBackwards
        {
            get
            {
                var isBackwards = MoveX > 0 && !_isFacingRight || MoveX < 0 && _isFacingRight;
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
            Cursor.lockState = CursorLockMode.Locked;
            _isFacingRight = true;
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

            /* Todo enable on combat animation appearing
            if (!IsAiming && ToAttack)
            {
                IsAiming = true;
                ToAttack = false;
            }*/
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

                var speed = (IsMovingBackwards)? backwardsSpeed : moveSpeed;
                _velocity.x = speed * MoveX;
            }
            _velocity.y -= gravity * Time.deltaTime; // (m/s^2)

            var move = (Vector3)_velocity * Time.deltaTime;
            _movement.Move(move);
            _velocity = _movement.Velocity;
        }

        // flips character is it's necessary
        private void UpdateDirection()
        {
            if (!IsAiming)
            {
                var input = InputUtil.GetMove();
                if (input.x > 0 && !_isFacingRight || input.x < 0 && _isFacingRight)
                    FlipDirection();
            }

            if (IsAiming && InputUtil.GetSpecialAbility())
            {
                if (_weaponX > 0.1 && !_isFacingRight || _weaponX < -0.1 && _isFacingRight)
                {
                    FlipDirection();
                    _weaponX *= -1;
                }
            }
        }

        // starts attack and handles aim visibility
        private void UpdateCombatState()
        {
            if (IsAiming && ToAttack && !manager.weapon.isInAttack)
                StartAttack();
        }

        private void UpdateAimPositions()
        {
            _weaponX += MouseDelta.x;
            _weaponX = (_weaponX > 1) ? 1 : _weaponX;
            _weaponX = (_weaponX < -1) ? -1 : _weaponX;
        }

        private void UpdateAnimation()
        {
            manager.animator.SetFloat(HashVelocityScaleX, GetHorizontalMoveScale());
            manager.animator.SetFloat(HashVelocityY, _velocity.y);
            manager.animator.SetBool(HashInFall, !IsGrounded);

            manager.animator.SetBool(HashAiming, IsAiming);
            manager.animator.SetFloat(HashWeaponX, _weaponX * (_isFacingRight ? 1 : -1));
            if (IsAiming && !WasAiming)
                manager.animator.SetTrigger(HashToAim);
        }

        #endregion

        #region Public

        public void RestoreAimPosition()
        {
            _weaponX = (_isFacingRight) ? 1 : -1;
        }

        #endregion
        
        #region Support methods

        private void FlipDirection()
        {
            _isFacingRight = !_isFacingRight;
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
            if (_weaponX > 0.7071f && _isFacingRight
                || _weaponX < -0.7071f && !_isFacingRight)
            {
                manager.animator.SetTrigger(HashToAttackPierce);
                manager.weapon.Type = DamageType.PierceDamage;
            }
            else if (_weaponX > -0.7071f && _isFacingRight 
                     || _weaponX < 0.7071f && !_isFacingRight)
            {
                manager.animator.SetTrigger(HashToAttackLight);
                manager.weapon.Type = DamageType.LightDamage;
            }
            else
            {
                manager.animator.SetTrigger(HashToAttackHeavy);
                manager.weapon.Type = DamageType.HeavyDamage;
            }
        }

        #endregion
    }
}