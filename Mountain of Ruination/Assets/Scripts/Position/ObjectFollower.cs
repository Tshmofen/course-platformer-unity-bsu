using UnityEngine;

namespace Assets.Scripts.Position
{
    [ExecuteInEditMode]
    public class ObjectFollower : MonoBehaviour
    {
        public GameObject target;

        public bool customOffset;
        public Vector2 positionOffset;
        public float angleOffset;

        private void Update()
        {
            transform.position = target.transform.position;
            if (customOffset)
            {
                transform.position += target.transform.rotation * positionOffset;
                transform.rotation = target.transform.rotation * Quaternion.Euler(0, 0, angleOffset);
            }
        }
    }
}