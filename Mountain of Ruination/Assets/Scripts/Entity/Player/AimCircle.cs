using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entity.Player
{
    public class AimCircle : MonoBehaviour
    {
        #region Unity calls

        private void Start()
        {
            _position = new Vector2();
            _sprite = GetComponent<SpriteRenderer>();
        }

        #endregion

        #region Fields and properties
        
        #region Unity assign

        [Header("Target")] 
        public GameObject targetObject;

        [Header("Position")] 
        public float radius = 1.3f;
        public float offsetX;
        public float offsetY;

        [FormerlySerializedAs("CircleFollower")] 
        [Header("Type")]
        public bool circleFollower;

        #endregion

        private Vector2 _position;

        private SpriteRenderer _sprite;

        [HideInInspector] public float speedCenter = 3.5f;
        [HideInInspector] public float speedEdge = 1.5f;
        [HideInInspector] public AimCircle targetCircle;

        public Vector2 Position => _position;
        public bool IsLocked { get; set; }
        public bool SpriteEnabled
        {
            set => _sprite.enabled = value;
        }

        #endregion

        #region Public

        // sets position of the circle on arc over the target
        public void CalculatePosition()
        {
            transform.position = targetObject.transform.position;

            if (circleFollower && !IsLocked)
            {
                var targetX = targetCircle._position.x;
                var speed = Mathf.Lerp(speedCenter, speedEdge, Mathf.Abs(_position.x));
                if (targetX < _position.x)
                {
                    _position.x -= speed * Time.deltaTime;
                    _position.x = _position.x < targetX ? targetX : _position.x;
                }
                else
                {
                    _position.x += speed * Time.deltaTime;
                    _position.x = _position.x > targetX ? targetX : _position.x;
                }

                _position.y = Mathf.Sqrt(1 - _position.x * _position.x);
                _position.Normalize();
            }

            Vector3 newPosition = _position * radius + new Vector2(offsetX, offsetY);

            transform.position += newPosition;
        }
        
        public void ChangePosition(Vector2 delta)
        {
            _position += delta;
            if (_position.y < 0) _position.y = 0;
            _position.Normalize();
        }

        // sets position to right end of arc
        public void RestorePosition(bool isFacingRight)
        {
            _position.Set(1, 0);
            if (!isFacingRight)
                _position *= -1;
        }

        public void FlipX()
        {
            _position.x *= -1;
        }

        #endregion
    }

    #region Custom editor

#if UNITY_EDITOR

    // enables follower settings if corresponding bool is enabled
    [CustomEditor(typeof(AimCircle))]
    public class AimCircleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var script = (AimCircle) target;

            if (script.circleFollower)
            {
                script.targetCircle = EditorGUILayout.ObjectField(
                    "Circle",
                    script.targetCircle,
                    typeof(AimCircle),
                    true
                ) as AimCircle;
                script.speedCenter = EditorGUILayout.FloatField("Speed Center", script.speedCenter);
                script.speedEdge = EditorGUILayout.FloatField("Speed Edge", script.speedEdge);
            }
        }
    }

#endif

    #endregion
}