using UnityEngine;

namespace Position
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraDuplicate : MonoBehaviour
    {
        private Camera _camera;
        
        public Camera mainCamera;

        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            _camera.orthographicSize = mainCamera.orthographicSize;
            _camera.transform.position = mainCamera.transform.position;
        }
    }
}