using Entity.Damage;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Overlay
{
    public class HealthBinder : MonoBehaviour
    {
        #region Fields
        
        #region Unity assigns
        
        [Header("Health")]
        public HealthStats health;
        public Image healthImage;
        public TMP_Text counter;
        [Header("Following")]
        public bool isFollower;
        [HideInInspector] public Transform target;
        [HideInInspector] public Vector2 offset;
        
        #endregion

        private Camera _camera;
        private int _previousHealth = -1;
        
        #endregion
        
        private void Start()
        {
            _camera = Camera.main;
        }

        private void FixedUpdate()
        {
            healthImage.fillAmount = health.CurrentHealth / health.maxHealth;
            if (health.CurrentHealth != _previousHealth)
            {
                counter.text = $"{health.CurrentHealth} / {health.maxHealth}";
                _previousHealth = health.CurrentHealth;
            }
        }

        private void Update()
        {
            if (isFollower)
            {
                transform.position = _camera.WorldToScreenPoint(target.position+ (Vector3) offset);
            }
        }
    }
    
    
    #if UNITY_EDITOR

    [CustomEditor(typeof(HealthBinder))]
    public class HealthBinderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var script = (HealthBinder) target;

            if (script.isFollower)
            {
                script.target = EditorGUILayout.ObjectField(
                    "Target transform",
                    script.target,
                    typeof(Transform),
                    true
                ) as Transform;
                script.offset = EditorGUILayout.Vector2Field("Offset", script.offset);
            }
        }
    }
    
    #endif
}