using System.Collections;
using Entity.Movement;
using ThirdParty.QPathFinder.Script;
using UnityEngine;

namespace Entity.Enemy
{
    [RequireComponent(typeof(MovementController))]
    public class EnemyController : MonoBehaviour
    {
        #region Fields and properties

        private Vector2 _player;
        private Vector2 _target;
        private bool _isRightToTarget;
        private bool _isLeftToTarget;

        private int _pathIndex;
        private bool _isMoveToEnd;

        private Vector2 _velocity;
        private float _direction;

        private MovementController _movement;
        private PathFinder _pathFinder;

        // also set player position in vector player
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
                    _player = colliders[0].transform.position;
                    var direction = _player - position;
                    var hit = Physics2D.Raycast(position, direction, direction.magnitude, groundLayer);
                    return hit.collider is null;
                }

                return false;
            }
        }

        private bool CanMoveToPlayer { get; set; }

        #endregion

        #region Initial Fields

        [Header("Movement")] 
        public float speed = 1;
        public float directionStep = 0.01f;
        public float gravity = 20;
        
        [Header("Patrolling")] 
        public int[] pathNodes;
        public float targetRadius = 0.2f;
        
        [Header("Detection")] 
        public LayerMask playerLayer;
        public LayerMask groundLayer;
        public float detectionRadius = 5;
        public float playerCheckPeriod = 0.2f;
        
        [Header("Combat")] 
        public float attackRadius = 0.5f;

        [Header("External")] 
        public EnemyManager manager;
        private static readonly int VelocityX = Animator.StringToHash("velocityX");

        #endregion

        #region Unity Calls

        private void Start()
        {
            _movement = GetComponent<MovementController>();
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

            if (CanMoveToPlayer)
                UpdatePlayerChase();
            else
                UpdatePatrolling();

            Move();
            UpdateAnimationState();
        }

        #endregion

        #region Update Parts

        private void UpdatePositionToTarget()
        {
            var position = transform.position;
            _isRightToTarget = _target.x - position.x < -targetRadius;
            _isLeftToTarget = _target.x - position.x > targetRadius;
        }

        // set player position as target
        private void UpdatePlayerChase()
        {
            _target = _player;
            var distance = (_target - (Vector2) transform.position).magnitude;
            if (distance < attackRadius) manager.weapon.ToAttack();
        }

        // set target as one of path nodes
        private void UpdatePatrolling()
        {
            if (!_isRightToTarget && !_isLeftToTarget)
            {
                _pathIndex = GetNextNodeIndex();
                var node = _pathFinder.graphData.GetNode(pathNodes[_pathIndex]);
                _target = node.Position;
            }
        }

        // changes direction if needed and move to target
        private void Move()
        {
            if (_isLeftToTarget)
                ChangeDirection(directionStep);
            else if (_isRightToTarget)
                ChangeDirection(-directionStep);
            else
                ChangeDirection(directionStep, true);
            ChangePosition();
        }

        // update animation
        private void UpdateAnimationState()
        {
            manager.animator.SetFloat(VelocityX, _velocity.x / speed);
        }

        #endregion

        #region Support Methods

        // ReSharper disable Unity.PerformanceAnalysis
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

            var nearest = _pathFinder.FindNearestNode(_player);
            var current = _pathFinder.FindNearestNode(transform.position);
            CanMoveToPlayer = false;
            _pathFinder.FindShortestPathOfNodes(current, nearest, Execution.Synchronous, nodes =>
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

        // smoothly change direction according to time
        // when toZero is true, delta should be positive
        private void ChangeDirection(float delta, bool toZero = false)
        {
            delta *= Time.deltaTime;
            if (!toZero)
            {
                _direction += delta;
                _direction = _direction > 1 ? 1 : _direction;
                _direction = _direction < -1 ? -1 : _direction;
            }
            else
            {
                if (_direction > 0)
                {
                    _direction -= delta;
                    _direction = _direction < 0 ? 0 : _direction;
                }
                else if (_direction < 0)
                {
                    _direction += delta;
                    _direction = _direction > 0 ? 0 : _direction;
                }
            }
        }

        // interact with movement controller to move entity
        private void ChangePosition()
        {
            _velocity.y -= gravity * Time.deltaTime; // (m/s^2)
            _velocity.x = speed * _direction;

            _movement.Move(_velocity * Time.deltaTime);
            _velocity = _movement.Velocity;
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