using UnityEngine;

namespace Assets.Scripts.Position
{
    public class ObjectAttacher : MonoBehaviour
    {
        public GameObject target;

        private void Start()
        {
            transform.parent = target.transform;
        }
    }
}