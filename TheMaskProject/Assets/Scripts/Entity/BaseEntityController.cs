using System;
using Entity.Movement;
using UnityEngine;

namespace Entity
{
    [RequireComponent(typeof(MovementController))]
    public abstract class BaseEntityController : MonoBehaviour
    {
        #region Fields

        #region Animation hashes
        
        // animation hashes
        private static readonly int HashVelocityScaleX = Animator.StringToHash("velocityScaleX");
        private static readonly int HashVelocityScaleY = Animator.StringToHash("velocityScaleY");
        private static readonly int HashInFall = Animator.StringToHash("inFall");
        private static readonly int HashInAttack = Animator.StringToHash("inAttack");
        private static readonly int HashToAttackLight = Animator.StringToHash("toAttackLight");
        private static readonly int HashToAttackHeavy = Animator.StringToHash("toAttackHeavy");
        private static readonly int HashToJump = Animator.StringToHash("toJump");
        private static readonly int HashToEvade = Animator.StringToHash("toEvade");
        
        #endregion
        
        [Header("Horizontal Movement")]
        public float moveSpeed = 4.5f;
        [Header("Evading")]
        public float evadeSpeed = 7;
        public float waitForEvadeTime = 0.3f;
        [Header("Other")]
        public float gravity = 20;
        public float slopeMoveUpdateDelay = 0.1f;
        [Header("External")]
        public EntityManager manager;
        

        private bool _wasMovingSlope;
        private float _wasMovingSlopeTime;
        
        protected MovementController Movement;

        protected bool IsLocked { get; private set; }
        protected bool IsAttacking { get; private set; }
        protected bool IsEvading { get; private set; }
        protected bool IsGroundedAfterSlope
        {
            get
            {
                var grounded = _wasMovingSlope || Movement.IsGrounded;

                if (Movement.CollisionState.MovingDownSlope || Movement.CollisionState.MovingUpSlope)
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
        protected bool IsFacingRight { get; private set; }
        
        #endregion

        #region Animation events (mostly)

        public void Lock() => IsLocked = true;
        public void Unlock() => IsLocked = false;
        
        public void StartAttackState() => IsAttacking = true;
        public void StopAttackState() => IsAttacking = false;

        public void StartEvadeState() => IsEvading = true;
        public void StopEvadeState() => IsEvading = false;

        #endregion

        protected virtual void Start()
        {
            Movement = GetComponent<MovementController>();
            IsFacingRight = true;
        }

        protected void FlipDirection()
        {
            IsFacingRight = !IsFacingRight;
            transform.forward *= -1;
            manager.weapon.transform.forward *= -1;
        }

        // temp crutch: turning off jump trigger manually after animation start
        protected void SetAnimationState(
            float velocityScaleX, float velocityScaleY, bool inFall,
            bool inAttack, bool toAttackLight, bool toAttackHeavy,
            bool toJump, bool toEvade)
        {
            manager.animator.SetFloat(HashVelocityScaleX, velocityScaleX);
            manager.animator.SetFloat(HashVelocityScaleY, velocityScaleY);
            manager.animator.SetBool(HashInFall, inFall);
            if(!IsLocked) 
            {
                manager.animator.SetBool(HashInAttack, inAttack);
                if (toAttackLight) manager.animator.SetTrigger(HashToAttackLight);
                if (toAttackHeavy) manager.animator.SetTrigger(HashToAttackHeavy);
                if (toEvade) manager.animator.SetTrigger(HashToEvade);
                
                if (!toJump && manager.animator.GetBool(HashToJump))
                    manager.animator.SetBool(HashToJump, false);
                if (toJump) manager.animator.SetTrigger(HashToJump);
            }
        }
        
        // returns movement scale from -1 to 1
        protected static float GetMoveScale(float speed, float maxSpeed) => Math.Abs(speed / maxSpeed);
    }
}