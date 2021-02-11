using Assets.Scripts.Damage;
using Assets.Scripts.Util;
using System;
using UnityEngine;
using Assets.Scripts.Entity.Movement;
using Assets.Scripts.Entity.Player.Aim;

namespace Assets.Scripts.Entity.Player
{
    [RequireComponent(typeof(MovementController))]
    public class PlayerController : MonoBehaviour
    {
        #region Fields and properties

        #region Fields private

        private bool wasAiming;
        private bool isAiming;
        private Vector2 velocity;
        private bool wasToContinueJump;

        private MovementController character;
        private Animator animator;

        #endregion

        #region Initial fields

        [Header("Horizontal Movement")]
        public float moveSpeed;
        public float backwardsSpeed;

        [Header("Vertical Movement")]
        public float jumpHeight;
        public float jumpManualDumping;
        public float gravity;

        [Header("Aim")]
        public AimCircle actualAim;
        public AimCircle targetAim;
        public float CircleSpeedCenter;
        public float CircleSpeedEdge;

        [Header("Damage Delivering")]
        public DamageDeliver deliver;

        #endregion

        #region Public properties

        public bool IsFacingRight { get; set; }
        public bool DisplayActualAim { get; set; }
        public bool IsMovingBackwards
        {
            get
            {
                bool isBackwards = MoveX > 0 && !IsFacingRight || MoveX < 0 && IsFacingRight;
                return IsAiming && isBackwards;
            }
        }

        #endregion

        #region Input

        public float MoveX { get; set; }
        public Vector2 MouseDelta { get; set; }
        public bool ToJump { get; set; }
        public bool ToContinueJump { get; set; }
        public bool IsAiming
        {
            get => isAiming;
            set
            {
                wasAiming = isAiming;
                isAiming = value;
            }
        }
        public bool WasAiming { get => wasAiming; }
        public bool ToAttack { get; set; }

        #endregion

        #endregion

        #region Unity calls

        private void Start()
        {
            character = GetComponent<MovementController>();
            animator = GetComponent<Animator>();

            targetAim.ChangePosition(new Vector2(1, 0));
            Cursor.lockState = CursorLockMode.Locked;
            DisplayActualAim = true;

            IsFacingRight = true;
        }

        private void Update()
        {
            GetControls();
            UpdatePosition();
            UpdateDirection();
            UpdateCombatState();
            UpdateAimPositions();
            UpdateAnimation();
        }

        #endregion

        #region Update parts
       
        private void GetControls()
        {
            MoveX          = InputUtil.GetMove().x;
            MouseDelta     = InputUtil.GetMousePositionDelta();
            ToJump         = InputUtil.GetJump();
            ToContinueJump = InputUtil.GetContinuousJump();
            IsAiming      ^= InputUtil.GetCombatMode();
            ToAttack       = InputUtil.GetAttack();
        }

        private void UpdatePosition()
        {
            if (ToJump && character.isGrounded)
            {
                velocity.y = Mathf.Sqrt(2f * jumpHeight * gravity);
                wasToContinueJump = true;
            }
            if (wasToContinueJump && !ToContinueJump)
            {
                if (velocity.y > 0)
                {
                    velocity.y -= jumpManualDumping;
                    velocity.y = (velocity.y < 0) ? 0 : velocity.y;
                }
                wasToContinueJump = false;
            }

            float speed = (IsMovingBackwards) ? backwardsSpeed : moveSpeed;
            velocity.x = speed * MoveX;
            velocity.y -= gravity * Time.deltaTime;

            character.move(velocity * Time.deltaTime);
            velocity = character.velocity;
        }

        private void UpdateDirection()
        {
            if (!IsAiming)
            {
                Vector2 input = InputUtil.GetMove();
                if (input.x > 0 && !IsFacingRight || input.x < 0 && IsFacingRight)
                    FlipDirection();
            }
            if (IsAiming && InputUtil.GetSpecialAbility())
            {
                float targetX = targetAim.Position.x;
                if (targetX > 0.1 && !IsFacingRight || targetX < -0.1 && IsFacingRight)
                {
                    FlipDirection();
                    actualAim.FlipX();
                }
            }
        }

        private void UpdateCombatState()
        {
            if (IsAiming && ToAttack && !deliver.isInAttack)
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
            animator.SetFloat("velocityScaleX", GetHorizontalMoveScale());
            animator.SetFloat("velocityY", velocity.y);
            animator.SetBool("inFall", !character.isGrounded);

            animator.SetBool("isAiming", IsAiming);
            animator.SetFloat("weaponX", actualAim.Position.x * (IsFacingRight ? 1 : -1));
            if (IsAiming && !WasAiming)
                animator.SetTrigger("toAim");
        }

        #endregion

        #region Support methods

        private void FlipDirection()
        {
            IsFacingRight = !IsFacingRight;
            transform.forward *= -1;
        }

        private float GetHorizontalMoveScale()
        {
            float velocityScale;

            if (IsMovingBackwards)
            {
                velocityScale = -Math.Abs(velocity.x / backwardsSpeed);
            }
            else
            {
                velocityScale = Math.Abs(velocity.x / moveSpeed);
            }

            return velocityScale;
        }

        private void StartAttack()
        {
            if (actualAim.Position.x > 0.7071f && IsFacingRight
                || actualAim.Position.x < -0.7071f && !IsFacingRight)
            {
                animator.SetTrigger("toAttackPierce");
                deliver.Type = DamageType.PierceDamage;
            }
            else if (actualAim.Position.x > -0.7071f && IsFacingRight
                 || actualAim.Position.x < 0.7071f && !IsFacingRight)
            {
                animator.SetTrigger("toAttackLight");
                deliver.Type = DamageType.LightDamage;
            }
            else
            {
                animator.SetTrigger("toAttackHeavy");
                deliver.Type = DamageType.HeavyDamage;
            }
        }

        private void MoveTargetAim()
        {
            float speed = Mathf.Lerp(CircleSpeedCenter, CircleSpeedEdge, Mathf.Abs(targetAim.Position.x));
            targetAim.ChangePosition(MouseDelta * speed * Time.deltaTime);
        }

        private void SwitchActualAim(bool enable)
        {
            actualAim.SpriteEnabled = enable && DisplayActualAim;
            actualAim.IsLocked = !enable;
        }

        #endregion
    }
}