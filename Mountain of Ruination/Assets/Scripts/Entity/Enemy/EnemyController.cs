using Assets.Scripts.Entity.Movement;
using QPathFinder;
using UnityEngine;

namespace Assets.Scripts.Entity.Enemy
{
    [RequireComponent(typeof(MovementController))]
    public class EnemyController : MonoBehaviour
    {

        #region Fields and properties

        private Vector2 velocity;
        private float direction;
        private int nodeIndex;

        private bool isMoveToEnd;

        private MovementController movement;
        private PathFinder pathFinder;

        #endregion

        #region Public fields

        [Header("Movement")]
        public float speed = 1;
        public float directionStep = 0.01f;
        public float gravity = 20;
        public int[] pathNodes;
        public float nodeRadius;
        [Header("External")]
        public EnemyManager manager;

        #endregion 

        #region Unity Calls

        void Start()
        {
            movement = GetComponent<MovementController>();
            pathFinder = PathFinder.Instance;
            pathFinder.graphData.ReGenerateIDs();

            nodeIndex = 0;
        }

        void Update()
        {
            /*
            if (isPlayerNearby && canMoveToPlayer) 
                moveToPlayer();
            else if (patrolPath.length != 0)
                moveByPath();
            */
            MoveByPath();
            UpdateAnimationState();
        }

        #endregion

        #region Update Parts 

        private void MoveByPath()
        {
            Node node = pathFinder.graphData.GetNode(pathNodes[nodeIndex]);
            float difference = node.Position.x - transform.position.x;
            if (difference > nodeRadius)
            {
                ChangeDirection(directionStep);
                move();
            }
            else if (difference < -nodeRadius)
            {
                ChangeDirection(-directionStep);
                move();
            }
            else
            {
                nodeIndex = getNextPathIndex();
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

        private void move()
        {
            velocity.y -= gravity * Time.deltaTime; // (m/s^2)
            velocity.x = speed * direction;

            movement.move(velocity * Time.deltaTime);
            velocity = movement.velocity;
        }

        private int getNextPathIndex()
        {
            int index = nodeIndex;
            index += (isMoveToEnd) ? 1 : -1;

            if (index == pathNodes.Length)
            {
                index -= 2;
                isMoveToEnd = false;
            }
            else if (index < 0)
            {
                index += 2;
                isMoveToEnd = true;
            }

            return index;
        }

        #endregion
    }
}