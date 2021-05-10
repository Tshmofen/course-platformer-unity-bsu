using System.Collections;
using ThirdParty.QPathFinder.Script;
using UnityEngine;

namespace Entity.Controller
{
    public class EnemyController : AbstractEntityController
    {
        #region Fields and properties

        #region Initial Fields
        
        [Header("Patrolling")] 
        public int[] pathNodes;
        public float targetRadius = 0.2f;
        public float playerDistance = 0.6f;
        [Header("Detection")] 
        public LayerMask playerLayer;
        public LayerMask groundLayer;
        public float detectionRadius = 5;
        public float playerCheckPeriod = 0.2f;
        [Header("Combat")] 
        public float attackRadius = 1f;
        
        #endregion
        
        private Vector2 _playerPosition;
        private Vector2 _target;
        
        private Vector2 _velocity;
        private PathFinder _pathFinder;
        
        private bool _isOnTarget;
        private bool _wasOnTarget;
        
        private bool _isMovingRight;
        private bool _isMoveToEnd;
        private int _pathIndex;
        
        private bool _toAttackLight;

        // this property also set player position in vector player
        private bool IsPlayerNearby
        {
            get
            {
                Vector2 position = transform.position;
                var colliders = new Collider2D[1];
                var size = Physics2D.OverlapCircleNonAlloc(
                    position,
                    detectionRadius,
                    colliders,
                    playerLayer
                );

                if (size != 0)
                {
                    _playerPosition = colliders[0].transform.position;
                    var direction = _playerPosition - position;
                    var hit = Physics2D.Raycast(position, direction, direction.magnitude, groundLayer);
                    return hit.collider is null;
                }

                return false;
            }
        }
        private bool CanMoveToPlayer { get; set; }

        #endregion

        #region Unity Calls

        protected override void Start()
        {
            base.Start();
            
            _pathFinder = PathFinder.Instance;
            _pathFinder.graphData.ReGenerateIDs();
            _isMoveToEnd = true;

            _pathIndex = 0;
            var node = _pathFinder.graphData.GetNode(pathNodes[_pathIndex]);
            _target = node.Position;

            // start every {playerCheckPeriod} seconds check of player position
            StartCoroutine(nameof(DoPlayerCheck));
        }

        private void Update()
        {
            UpdatePositionToTarget();
            UpdatePatrolling();
            if (isLocked) return;
            UpdateMovement();
            UpdateDirection();
            UpdateAnimationState();
        }

        #endregion

        #region Update Parts

        private void UpdatePositionToTarget()
        {
            var position = transform.position;
            var isRightToTarget = _target.x - position.x < -targetRadius;
            var isLeftToTarget = _target.x - position.x > targetRadius;

            _isMovingRight = isLeftToTarget;
            _wasOnTarget = _isOnTarget;
            _isOnTarget = !isLeftToTarget && !isRightToTarget;
        }

        // set player position as target
        private void UpdatePlayerChase()
        {
            _target = _playerPosition;
            var distance = (_target - (Vector2) transform.position).magnitude;
            _toAttackLight = (distance < attackRadius);
        }

        // set target as one of path nodes
        private void UpdatePatrolling()
        {
            if (CanMoveToPlayer)
                UpdatePlayerChase();
            if (_isOnTarget && !_wasOnTarget)
            {
                _pathIndex = GetNextNodeIndex();
                var node = _pathFinder.graphData.GetNode(pathNodes[_pathIndex]);
                _target = node.Position;
            }
        }

        // changes direction if needed and move to target
        private void UpdateMovement()
        {
            var distance = (_playerPosition - (Vector2)transform.position).magnitude;
            
            if (isInAttack || distance < playerDistance) _velocity.x = 0;
            else _velocity.x = (_isMovingRight) ? moveSpeed : -moveSpeed;
            
            _velocity.y -= gravity * Time.deltaTime; // (m/s^2)

            Movement.Move(_velocity * Time.deltaTime);
            _velocity = Movement.Velocity;
        }

        private void UpdateDirection()
        {
            if (isLocked || isInAttack)
                return;
            if (_velocity.x > 0 && !IsFacingRight || _velocity.x < 0 && IsFacingRight)
                FlipDirection();
        }

        // update animation
        private void UpdateAnimationState()
        {
            var velocityScaleX = GetMoveScale(_velocity.x, moveSpeed);
            var velocityScaleY = _velocity.y;
            var inFall = !IsGrounded;
            var inAttack = isInAttack;
            var toAttackLight = _toAttackLight;

            SetAnimationState(
                velocityScaleX, velocityScaleY, inFall,
                inAttack, toAttackLight, false,
                false, false
            );
        }

        #endregion

        #region Support Methods
        
        private IEnumerator DoPlayerCheck()
        {
            while (true)
            {
                CheckIfCanGoToPlayer();
                yield return new WaitForSeconds(playerCheckPeriod);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        // find nearest nodes to the player and to the current entity
        // and then check if there is a path between them
        // if the path is found, then on the next frame this entity
        // would chase the player
        private void CheckIfCanGoToPlayer()
        {
            if (!IsPlayerNearby)
            {
                CanMoveToPlayer = false;
                return;
            }

            var nearest = _pathFinder.FindNearestNode(_playerPosition);
            var current = _pathFinder.FindNearestNode(transform.position);
            CanMoveToPlayer = false;
            _pathFinder.FindShortestPathOfNodes(current, nearest,
                Execution.Synchronous, nodes =>
            {
                if (nodes != null && nodes.Count != 0)
                {
                    for (var i = 1; i < nodes.Count; i++)
                        if (nodes[i].pathDistance != 0)
                            return;
                    CanMoveToPlayer = true;
                }
            });
        }

        // iterates over pathNodes from the start to the end, and then in backwards
        private int GetNextNodeIndex()
        {
            var index = _pathIndex;
            index += _isMoveToEnd ? 1 : -1;

            if (index == pathNodes.Length)
            {
                index -= 2;
                _isMoveToEnd = false;
            }

            if (index < 0)
            {
                index += 2;
                _isMoveToEnd = true;
            }

            return index;
        }

        #endregion
    }
}