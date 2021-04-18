using UnityEngine;

namespace Environment.Background
{
    public class CloudController : MonoBehaviour
    {
        #region Fields & properties

        private Camera _camera;
        
        public float speedX = 0.1f;

        #endregion

        #region Unity calls

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
            
            var transformCloud = transform;
            var transformCamera = _camera.transform;
            var positionCamera = transformCamera.position;
            var positionCloud = transformCloud.position;
            
            var minus = (positionCloud.x - positionCamera.x > 0) ? -1 : 1;
            var newX = positionCamera.x + minus * 1.2f * _camera.orthographicSize * _camera.aspect;
            transformCloud.position = new Vector3(newX, positionCloud.y);
        }

        #endregion
        
    }
}