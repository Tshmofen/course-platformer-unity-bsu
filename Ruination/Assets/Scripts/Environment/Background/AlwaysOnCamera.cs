using UnityEngine;

namespace Environment.Background
{
    public class AlwaysOnCamera : MonoBehaviour
    {
        private Camera _camera;

        private Renderer _rendererPaired;
        private Renderer _renderer;

        public GameObject pairedObject;
        
        private void Start()
        {
            _camera = Camera.main;
            _renderer = GetComponent<Renderer>();
            _rendererPaired = pairedObject.GetComponent<Renderer>();
        }

        private void OnBecameInvisible() => MoveInView();

        private void OnBecameVisible() => MoveInView();

        private void MoveInView()
        {
            if (_camera == null) return;
            
            var transformObject = transform;
            var transformCamera = _camera.transform;
            var positionObject = transformObject.position;
            var positionCamera = transformCamera.position;
            
            var objectHalfWidth = _renderer.bounds.extents.x;
            var pairedHalfWidth = _rendererPaired.bounds.extents.x;
            var cameraHalfWidth = _camera.orthographicSize * _camera.aspect;
            
            var objectRightEdgeInView =
                positionObject.x + objectHalfWidth > positionCamera.x - cameraHalfWidth &&
                positionObject.x + objectHalfWidth < positionCamera.x + cameraHalfWidth;
            var objectLeftEdgeInView =
                positionObject.x - objectHalfWidth > positionCamera.x - cameraHalfWidth &&
                positionObject.x - objectHalfWidth < positionCamera.x + cameraHalfWidth;
            
            if (!objectLeftEdgeInView && !objectRightEdgeInView)
            {
                // if object right to camera add minus
                var minus = (positionObject.x - positionCamera.x > 0) ? -1 : 1;
                var newPosition = pairedObject.transform.position;
                newPosition.x += minus * pairedHalfWidth * 0.99f;
                newPosition.x += minus * objectHalfWidth;
                transformObject.position = newPosition;
            }
        }
    }
}