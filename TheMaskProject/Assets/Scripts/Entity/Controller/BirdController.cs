using UnityEngine;

namespace Entity.Controller
{
    public class BirdController : MonoBehaviour
    {
        private int _direction;
        
        [Header("Position")]
        public float startX;
        public float endX;
        public float speed = 1.5f;

        private void Start()
        {
            _direction = (int)Mathf.Sign(endX - startX);
        }

        private void Update()
        {
            var birdTransform = transform;
            var newPosition = birdTransform.localPosition;
            
            newPosition.x += _direction * speed * Time.deltaTime;
            if (newPosition.x > endX && _direction == 1 || newPosition.x < endX && _direction == -1)
            {
                newPosition.x = startX;
            }
            
            birdTransform.localPosition = newPosition;
        }
    }
}