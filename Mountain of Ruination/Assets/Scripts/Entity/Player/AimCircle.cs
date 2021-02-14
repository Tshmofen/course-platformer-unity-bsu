using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Entity.Player
{
    public class AimCircle : MonoBehaviour
    {
        #region Fields and properties

        private Vector2 position;

        [Header("Target")]
        public GameObject targetObject;

        [Header("Position")]
        public float radius = 1.3f;
        public float offsetX;
        public float offsetY;

        [Header("Type")]
        public bool CircleFollower;
        [HideInInspector] public float speedCenter = 3.5f;
        [HideInInspector] public float speedEdge = 1.5f;
        [HideInInspector] public AimCircle targetCircle;

        public Vector2 Position { get => position; }
        public bool IsLocked { get; set; }

        public bool SpriteEnabled
        {
            get => GetComponent<SpriteRenderer>().enabled;
            set => GetComponent<SpriteRenderer>().enabled = value;
        }

        #endregion

        #region Unity calls

        private void Start()
        {
            position = new Vector2();
        }

        #endregion

        #region Public

        public void CalculatePosition()
        {
            transform.position = targetObject.transform.position;

            if (CircleFollower && !IsLocked)
            {
                float targetX = targetCircle.position.x;
                float speed = Mathf.Lerp(speedCenter, speedEdge, Mathf.Abs(position.x));
                if (targetX < position.x)
                {
                    position.x -= speed * Time.deltaTime;
                    position.x = (position.x < targetX) ? targetX : position.x;
                }
                else
                {
                    position.x += speed * Time.deltaTime;
                    position.x = (position.x > targetX) ? targetX : position.x;
                }
                position.y = Mathf.Sqrt(1 - position.x * position.x);
                position.Normalize();
            }
            Vector3 newPosition = position * radius + new Vector2(offsetX, offsetY); ;

            transform.position += newPosition;
        }

        public void ChangePosition(Vector2 delta)
        {
            position += delta;
            if (position.y < 0) position.y = 0;
            position.Normalize();
        }

        public void RestorePosition(bool isFacingRight)
        {
            position.Set(1, 0);
            if (!isFacingRight)
                position *= -1;
        }

        public void FlipX()
        {
            position.x *= -1;
        }

        #endregion
    }

    #region Custom editor
#if UNITY_EDITOR

    [CustomEditor(typeof(AimCircle))]
    public class RandomScript_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AimCircle script = (AimCircle)target;

            if (script.CircleFollower)
            {
                script.targetCircle = EditorGUILayout.ObjectField(
                    "Circle", script.targetCircle, typeof(AimCircle), true
                    ) as AimCircle;
                script.speedCenter = EditorGUILayout.FloatField("Speed Center", script.speedCenter);
                script.speedEdge = EditorGUILayout.FloatField("Speed Edge", script.speedEdge);
            }
        }
    }

#endif
    #endregion
}
