using Assets.Scripts.Entity.Movement;
using QPathFinder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Entity.Enemy
{
    [RequireComponent(typeof(MovementController))]
    public class EnemyController : MonoBehaviour
    {

        #region Fields and properties

        private Vector2 player;
        private Vector2 target;
        private bool isRightToTarget;
        private bool isLeftToTarget;

        private int pathIndex;
        private int nextNode;
        private bool isMoveToEnd;

        private Vector2 velocity;
        private float direction;
       
        private MovementController movement;
        private PathFinder pathFinder;

        // also set player position in vector player
        public bool IsPlayerNearby
        {
            get
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
                if (colliders.Length != 0)
                {
                    player = colliders[0].transform.position;
                    Vector2 direction = player - (Vector2)transform.position;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, direction.magnitude, groundLayer);
                    return hit.collider == null;
                }
                return false;
            }
        }
        public bool CanMoveToPlayer { get; set; }

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

        #endregion 

        #region Unity Calls

        void Start()
        {
            movement = GetComponent<MovementController>();
            pathFinder = PathFinder.Instance;
            pathFinder.graphData.ReGenerateIDs();
            isMoveToEnd = true;

            pathIndex = 0;
            Node node = pathFinder.graphData.GetNode(pathNodes[pathIndex]);
            target = node.Position;

            StartCoroutine("DoPlayerCheck");
        }

        void Update()
        {
            UpdateTargetDistance();

            if (CanMoveToPlayer)
                UpdatePlayerChase();
            else
                UpdatePatrolling();

            Move();
            UpdateAnimationState();
        }

        #endregion

        #region Update Parts 

        private void UpdateTargetDistance()
        {
            isRightToTarget = target.x - transform.position.x < -targetRadius;
            isLeftToTarget  = target.x - transform.position.x > targetRadius;
        }

        private void UpdatePlayerChase()
        {
            target = player;

            // this isInAttack bool is neccessary to handle first frame 
            // before animation when weapon isInAttack not set yet
            float distance = (target - (Vector2)transform.position).magnitude;
            if (distance < attackRadius)
            {
                manager.weapon.ToAttack();
            }
        }

        private void UpdatePatrolling()
        {
            if (!isRightToTarget && !isLeftToTarget)
            { 
                pathIndex = GetNextNodeIndex();
                Node node = pathFinder.graphData.GetNode(pathNodes[pathIndex]);
                target = node.Position;
            }
        }

        private void Move()
        {
            if (isLeftToTarget)
                ChangeDirection(directionStep);
            else if (isRightToTarget)
                ChangeDirection(-directionStep);
            else
                ChangeDirection(directionStep, true);
            ChangePosition();
        }

        private void UpdateAnimationState()
        {
            manager.animator.SetFloat("velocityX", velocity.x / speed);
        }

        #endregion

        #region Support Methods

        private IEnumerator DoPlayerCheck()
        {
            while(true)
            {
                CheckIfCanGoToPlayer();
                yield return new WaitForSeconds(playerCheckPeriod);
            }
        }

        private void CheckIfCanGoToPlayer()
        {
            if (!IsPlayerNearby)
            {
                CanMoveToPlayer = false;
                return;
            }

            int nearest = pathFinder.FindNearestNode(player);
            int current = pathFinder.FindNearestNode(transform.position);
            CanMoveToPlayer = false;
            pathFinder.FindShortestPathOfNodes(current, nearest, Execution.Synchronous, nodes =>
            {
                if (nodes != null && nodes.Count != 0)
                {
                    for (int i = 1; i < nodes.Count; i++)
                    {
                        if (nodes[i].pathDistance != 0)
                            return;
                    }
                    CanMoveToPlayer = true;
                }
            });
        }

        // when toZero is true, delta should be positive
        private void ChangeDirection(float delta, bool toZero = false)
        {
            delta *= Time.deltaTime;
            if (!toZero)
            {
                direction += delta;
                direction = (direction > 1) ? 1 : direction;
                direction = (direction < -1) ? -1 : direction;
            }
            else
            {
                if (direction > 0)
                {
                    direction -= delta;
                    direction = (direction < 0) ? 0 : direction;
                }
                else if (direction < 0)
                {
                    direction += delta;
                    direction = (direction > 0) ? 0 : direction;
                }  
            }
        }

        private void ChangePosition()
        {
            velocity.y -= gravity * Time.deltaTime; // (m/s^2)
            velocity.x = speed * direction;

            movement.move(velocity * Time.deltaTime);
            velocity = movement.velocity;
        }

        private int GetNextNodeIndex()
        {
            int index = pathIndex;
            index += (isMoveToEnd) ? 1 : -1;

            if (index == pathNodes.Length)
            {
                index -= 2;
                isMoveToEnd = false;
            }
            if (index < 0)
            {
                index += 2;
                isMoveToEnd = true;
            }

            return index;
        }

        #endregion
    }
}