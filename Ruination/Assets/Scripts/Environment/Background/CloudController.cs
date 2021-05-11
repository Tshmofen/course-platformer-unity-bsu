using UnityEngine;

namespace Environment.Background
{
    public class CloudController : MonoBehaviour
    {
        public float speedX = 0.1f;
        
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }
        
        private void Update()
        {
            transform.position += new Vector3(speedX * Time.deltaTime, 0);
        }
        
        private void OnBecameInvisible()
        {
            if (_camera == null) return;
            
            var transformCamera = _camera.transform;
            var positionObject = transform.position;
            var positionCamera = transformCamera.position;

            // if object right to camera add minus
            var minus = (positionObject.x - positionCamera.x > 0) ? -1 : 1;
            var newX = positionCamera.x;
            newX += minus * _camera.orthographicSize * _camera.aspect;
            newX += minus * GetComponent<Renderer>().bounds.extents.x;
            transform.position = new Vector3(newX, positionObject.y);
        }
    }
}