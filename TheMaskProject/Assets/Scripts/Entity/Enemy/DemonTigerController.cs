using System.Collections.Generic;
using UnityEngine;

namespace Entity.Enemy
{
    public class DemonTigerController : BaseEnemyController
    {
        private static readonly int HashToAttackRush = Animator.StringToHash("toAttackRush");
        private enum Attack
        {
            Light,
            Heavy
        }

        [Header("Combat")] 
        public float attackRadius = 1f;
        public float targetRadius = 0.2f;
        public int lightAttacksBeforeHeavy = 2;
        [Header("Timings")] 
        public float attackLightWait = 0.5f;
        public float attackHeavyWait = 0.5f;

        private Vector2 _velocity;
        private Vector2 _patrolNode;

        private bool _isLeftToTarget;
        private bool _isRightToTarget;
        private bool _isOnTarget;
        
        private bool _toAttackLight;
        private bool _toAttackHeavy;
        private bool _toAttackRush;
        
        private int _lightAttacksCount;
        private Dictionary<Attack, (bool forbidden, float time)> _forbids;

        protected override void Start()
        {
            base.Start();
            _patrolNode = GetNextNode();
            
            _forbids = new Dictionary<Attack, (bool forbidden, float time)>
            {
                {Attack.Light, (false, 0)},
                {Attack.Heavy, (false, 0)},
            };
        }

        private void Update()
        {
            if (Time.deltaTime == 0) return;
            
            if (!IsLocked)
            {
                UpdateTimer();
                UpdateTargeting();
                UpdateMovement();
                UpdateDirection();
            }
            UpdateAnimationState();
        }

        private void UpdateTimer()
        {
            if (_forbids[Attack.Light].forbidden)
            {
                if (_forbids[Attack.Light].time < attackLightWait)
                    _forbids[Attack.Light] = (true, _forbids[Attack.Light].time + Time.deltaTime);
                else
                    _forbids[Attack.Light] = (false, attackLightWait);
            }
            
            if (_forbids[Attack.Heavy].forbidden)
            {
                if (_forbids[Attack.Heavy].time < attackHeavyWait)
                    _forbids[Attack.Heavy] = (true, _forbids[Attack.Heavy].time + Time.deltaTime);
                else
                    _forbids[Attack.Heavy] = (false, attackHeavyWait);
            }
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
         
        // Handles attacks
        private void UpdateEnemyChase()
        {
            ResetPositioning(EnemyPosition);
            
            _toAttackHeavy = false;
            _toAttackLight = false;
            _toAttackRush = false;
            var distance = (EnemyPosition - (Vector2) transform.position).magnitude;
            var isUsualAttackAvailable =
                !_forbids[Attack.Light].forbidden || !_forbids[Attack.Heavy].forbidden;
            
            if (!IsAttacking && isUsualAttackAvailable && distance <= attackRadius )
            {
                if (_lightAttacksCount < lightAttacksBeforeHeavy)
                {
                    _lightAttacksCount++;
                    _toAttackLight = true;
                    _forbids[Attack.Light] = (true, 0);
                }
                else
                {
                    _lightAttacksCount = 0;
                    _toAttackHeavy = true;
                    _forbids[Attack.Heavy] = (true, 0);
                }
            }
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
            _velocity.y -= gravity * Time.deltaTime; // (m/s^2)
            var isOnTarget = !_isLeftToTarget && !_isRightToTarget;
            
            if (IsEvading)
                UseEvadeMovement();
            else if (IsAttacking)
                UseAttackMovement();
            else if (isOnTarget)
                UseTargetMovement();
            else
                UseUsualMovement();
            
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
            var toAttackHeavy = !IsAttacking && _toAttackHeavy;
            var toAttackRush = !IsAttacking && _toAttackRush;

            SetAnimationState(
                velocityScaleX, velocityScaleY, inFall,
                inAttack, toAttackLight, toAttackHeavy,
                false, false
            );
            
            if (toAttackRush) manager.animator.SetTrigger(HashToAttackRush);
        }

        #region Movement Behaviour

        private void UseAttackMovement()
        {
            _velocity.x = 0;
        }

        private void UseTargetMovement()
        {
            _velocity.x = 0;
        }

        private void UseEvadeMovement()
        {
            var minus = (IsFacingRight) ? 1 : -1;
            _velocity = new Vector2(minus * evadeSpeed, 0);
        }

        private void UseUsualMovement()
        {
            _velocity.x = (_isLeftToTarget) ? moveSpeed : -moveSpeed;
        }

        #endregion
    }
}