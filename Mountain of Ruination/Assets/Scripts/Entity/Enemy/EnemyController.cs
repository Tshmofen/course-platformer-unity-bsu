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
        private int nodeIndex;
        private bool isMoveToEnd;

        private MovementController movement;
        private PathFinder pathFinder;

        #endregion

        #region Public fields

        [Header("Movement")]
        public float speed = 4;
        public float gravity = 20;
        public int[] pathNodes;
        public float nodeRadius;
        [Header("External")]
        public Animator animator;

        #endregion 

        #region Unity Calls

        void Start()
        {
            movement = GetComponent<MovementController>();
            animator = GetComponent<Animator>();
            pathFinder = QPathFinder.PathFinder.Instance;
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
            moveByPath();
        }

        #endregion

        #region Update Parts 

        private void moveByPath()
        {
            Node node = pathFinder.graphData.GetNode(pathNodes[nodeIndex]);
            float difference = node.Position.x - transform.position.x;
            if (difference > nodeRadius)
                move(1);
            else if (difference < -nodeRadius)
                move(-1);
            else
                nodeIndex = getNextPathIndex();

        }

        #endregion

        #region Support Methods

        private void move(int direction)
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