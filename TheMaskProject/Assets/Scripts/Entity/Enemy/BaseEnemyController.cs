using System.Collections;
using ThirdParty.QPathFinder.Script;
using UnityEngine;

namespace Entity.Enemy
{
    public abstract class BaseEnemyController : BaseEntityController
    {
        #region Fields and properties

        #region Initial Fields
        
        [Header("Patrolling")] 
        public int[] pathNodes;
        [Header("Detection")] 
        public LayerMask enemyLayer;
        public LayerMask groundLayer;
        public float enemyDetectionRadius = 5;
        public float enemyCheckPeriod = 0.2f;

        #endregion
        
        private PathFinder _pathFinder;
        private bool _isMoveToEnd;
        private int _pathIndex;

        private bool _isEnemyNearby;
        private bool _canSeeEnemy;
        
        protected Vector2 EnemyPosition { get; private set; }
        protected bool CanReachEnemy { get; private set; }

        #endregion
        
        protected override void Start()
        {
            base.Start();
            
            _pathFinder = PathFinder.Instance;
            _pathFinder.graphData.ReGenerateIDs();
            _isMoveToEnd = true;
            _pathIndex = -1;
            
            // start every {playerCheckPeriod} seconds check of player position
            StartCoroutine(nameof(DoEnemyReachCheck));
        }

        #region Enemy reaching

        private IEnumerator DoEnemyReachCheck()
        {
            while (true)
            {
                UpdateEnemyPosition();
                UpdateEnemyVisibility();
                UpdateEnemyReach();
                yield return new WaitForSeconds(enemyCheckPeriod);
            }
            // ReSharper disable once IteratorNeverReturns
        }
        
        private void UpdateEnemyPosition()
        {
            Vector2 position = transform.position;
            var colliders = new Collider2D[5];
            var size = Physics2D.OverlapCircleNonAlloc(
                position,
                enemyDetectionRadius,
                colliders,
                enemyLayer
            );

            for (var i = 0; i < size; i++)
            {
                if (colliders[i] != null && colliders[i].gameObject.activeInHierarchy)
                {
                    _isEnemyNearby = true;
                    EnemyPosition = colliders[i].transform.position;
                    return;
                }
            }

            _isEnemyNearby = false;
        }

        private void UpdateEnemyVisibility()
        {
            if (!_isEnemyNearby)
            {
                _canSeeEnemy = false;
                return;
            }
            
            var position = (Vector2)transform.position;
            var direction = EnemyPosition - position;
            
            if (direction.magnitude > enemyDetectionRadius)
            {
                _canSeeEnemy = false;
            }
            else
            {
                var hit = Physics2D.Raycast(
                    position,
                    direction,
                    enemyDetectionRadius, 
                    groundLayer
                );
                
                _canSeeEnemy = hit.collider is null;
            }
        }

        // find nearest nodes to the player and to the current entity
        // and then check if there is a path between them
        // if the path is found, then on the next frame this entity
        // would chase the player
        private void UpdateEnemyReach()
        {
            if (!_canSeeEnemy)
            {
                CanReachEnemy = false;
                return;
            }

            var nearest = _pathFinder.FindNearestNode(EnemyPosition);
            var current = _pathFinder.FindNearestNode(transform.position);
            CanReachEnemy = false;
            _pathFinder.FindShortestPathOfNodes(
                current, 
                nearest,
                Execution.Synchronous, 
                nodes =>
                    {
                        if (nodes != null && nodes.Count != 0)
                        {
                            for (var i = 1; i < nodes.Count; i++)
                                if (nodes[i].pathDistance != 0)
                                    return;
                            CanReachEnemy = true;
                        }
                    }
                );
        }
        
        #endregion
        
        #region Patrolling nodes

        protected Vector2 GetNextNode()
        {
            _pathIndex = GetNextNodeIndex();
            var node = _pathFinder.graphData.GetNode(pathNodes[_pathIndex]);
            return node.Position;
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