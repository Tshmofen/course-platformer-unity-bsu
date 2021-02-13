using Assets.Scripts.Entity.Movement;
using QPathFinder;
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

        private int nodeIndex;
        private bool isMoveToEnd;

        private Vector2 velocity;
        private float direction;
       
        private MovementController movement;
        private PathFinder pathFinder;

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

            nodeIndex = 0;
            Node node = pathFinder.graphData.GetNode(pathNodes[nodeIndex]);
            target = node.Position;
        }

        void Update()
        {
            /*if (IsPlayerNearby)
            {
                int nearestNode = pathFinder.FindNearestNode(playerPosition);
                pathFinder.FindShortestPathOfNodes(CurrentNode, nearestNode, Execution.Synchronous, nodes =>
                {
                    if (nodes.Count != 0)
                    {

                    }
                });
            }*/
            UpdateTargetDistance();
            GoToPatroll();
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

        private void GoToPatroll()
        {
            if (!isRightToTarget && !isLeftToTarget)
            { 
                nodeIndex = GetNextNodeIndex();
                Node node = pathFinder.graphData.GetNode(pathNodes[nodeIndex]);
                target = node.Position;
            }
        }

        private void Move()
        {
            if (isLeftToTarget)
            {
                ChangeDirection(directionStep);
                ChangePosition();
            }
            if (isRightToTarget)
            {
                ChangeDirection(-directionStep);
                ChangePosition();
            }
        }

        private void UpdateAnimationState()
        {
            manager.animator.SetFloat("velocityX", velocity.x / speed);
        }

        #endregion

        #region Support Methods

        private void ChangeDirection(float delta)
        {
            direction += delta * Time.deltaTime;
            direction = (direction > 1) ? 1 : direction;
            direction = (direction < -1) ? -1 : direction;
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
            int index = nodeIndex;
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