using UnityEngine;

namespace Entity.Enemy
{
    public class PriestController : BaseEnemyController
    {
        [Header("Combat")] 
        public float attackRadius = 1f;
        public float targetRadius = 0.2f;

        private Vector2 _velocity;
        private Vector2 _patrolNode;

        private bool _isLeftToTarget;
        private bool _isRightToTarget;
        
        private bool _isOnTarget;
        private bool _toAttackLight;

        protected override void Start()
        {
            base.Start();
            _patrolNode = GetNextNode();
        }

        private void Update()
        {
            if (!IsLocked)
            {
                UpdateTargeting();
                UpdateMovement();
                UpdateDirection();
            }
            UpdateAnimationState();
        }

        // set target as one of path nodes
        private void UpdateTargeting()
        {
            if (CanReachEnemy)
            {
                UpdateEnemyChase();
                return;
            }
            
            UpdatePatrolling();
        }
        
        private void ResetPositioning(Vector2 target)
        {
            var position = transform.position;
            _isLeftToTarget = target.x - position.x > targetRadius;
            _isRightToTarget = target.x - position.x < -targetRadius;
        }
         
        // set player position as target
        private void UpdateEnemyChase()
        {
            ResetPositioning(EnemyPosition);
            
            var distance = (EnemyPosition - (Vector2) transform.position).magnitude;
            _toAttackLight = !IsAttacking && (distance < attackRadius);
        }

        private void UpdatePatrolling()
        {
            ResetPositioning(_patrolNode);
            
            var wasOnTarget = _isOnTarget;
            _isOnTarget = !_isLeftToTarget && !_isRightToTarget;
            
            if (_isOnTarget && !wasOnTarget)
            {
                _patrolNode = GetNextNode();
            }
        }

        // changes direction if needed and move to target
        private void UpdateMovement()
        {
            _velocity.x = (_isLeftToTarget) ? moveSpeed : -moveSpeed;
            if (IsAttacking || !_isLeftToTarget && !_isRightToTarget)
            {
                _velocity.x = 0;
            }

            _velocity.y -= gravity * Time.deltaTime; // (m/s^2)
            Movement.Move(_velocity * Time.deltaTime);
            
            _velocity = Movement.Velocity;
        }

        private void UpdateDirection()
        {
            if (IsLocked || IsAttacking)
                return;
            if (_isLeftToTarget && !IsFacingRight || !_isLeftToTarget && IsFacingRight)
                FlipDirection();
        }

        // update animation
        private void UpdateAnimationState()
        {
            var velocityScaleX = GetMoveScale(_velocity.x, moveSpeed);
            var velocityScaleY = _velocity.y;
            var inFall = !IsGroundedAfterSlope;
            var inAttack = IsAttacking;
            var toAttackLight = !IsAttacking && _toAttackLight;

            SetAnimationState(
                velocityScaleX, velocityScaleY, inFall,
                inAttack, toAttackLight, false,
                false, false
            );
        }
    }
}