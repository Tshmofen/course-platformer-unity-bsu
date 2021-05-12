using UnityEngine;
using Util;

namespace Interface.World
{
    public class InteractPointer : MonoBehaviour
    {
        #region Fields & properties

        [Header("Position")]
        public float radius = 3;
        [Header("Movement")]
        public float speed = 7;

        #endregion
        
        private void Start()
        {
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            var move = InputUtil.GetMousePositionDelta();
            if (move != Vector2.zero)
            {
                var pointerTransform = transform;
                var position = (Vector2)pointerTransform.position + move * (speed * Time.deltaTime);
                var parentPosition = (Vector2) pointerTransform.parent.position;
                var distance = position - parentPosition;
                if (distance.magnitude > radius) position = parentPosition + distance.normalized * radius;

                pointerTransform.position = position;
            }
        }
    }
}