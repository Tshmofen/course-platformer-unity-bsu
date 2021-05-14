using UnityEngine;

namespace Environment.Follower
{
    [ExecuteInEditMode]
    public class ObjectFollower : MonoBehaviour
    {
        #region Fields

        [Header("Position")]
        public GameObject target;
        public bool customOffset;
        public Vector2 positionOffset;
        [Range(0, 360)] public float angleOffset;
        
        #endregion

        // follows another object
        // if necessary offsets and rotate it
        // around this object
        private void Update()
        {
            transform.position = target.transform.position;
            
            if (customOffset)
            {
                var rotation = target.transform.rotation;
                transform.position += rotation * positionOffset;
                transform.rotation = rotation * Quaternion.Euler(0, 0, angleOffset);
            }
        }
    }
}