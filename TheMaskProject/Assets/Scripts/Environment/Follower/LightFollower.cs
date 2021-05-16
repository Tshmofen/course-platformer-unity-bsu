using UnityEngine;

namespace Environment.Follower
{
    [ExecuteInEditMode]
    public class LightFollower : MonoBehaviour
    {
        public GameObject target;
        
        [Header("Position")]
        [Range(0, 360)] public float angleOffset = 180;
        public float radius = 10;

        private void Update()
        {
            var targetPosition = target.transform.position;
            var lightTransform = transform;
            lightTransform.position 
                = targetPosition + Quaternion.Euler(0, 0, angleOffset) * Vector3.right * radius;
        }
    }
}