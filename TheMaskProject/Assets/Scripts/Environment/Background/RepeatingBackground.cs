using UnityEngine;

namespace Environment.Background
{
    public class RepeatingBackground : MonoBehaviour
    {
        [Header("Positioning")]
        public Camera mainCamera;
        public Renderer leftRenderer;
        public Renderer mainRenderer;
        public Renderer rightRenderer;

        private void FixedUpdate()
        {
            var cameraPositionX = mainCamera.transform.position.x;
            var mainPosition = mainRenderer.transform.position;
            var mainExtents = mainRenderer.bounds.extents;

            var mainLeftEdgeX = mainPosition.x - mainExtents.x;
            var mainRightEdgeX = mainPosition.x + mainExtents.x;

            if (mainLeftEdgeX > cameraPositionX)
            {
                var offset = new Vector3(rightRenderer.bounds.size.x, 0, 0);
                rightRenderer.transform.position = leftRenderer.transform.position - offset;
                (leftRenderer, mainRenderer, rightRenderer) = (rightRenderer, leftRenderer, mainRenderer);
            }
            
            if (mainRightEdgeX < cameraPositionX)
            {
                var offset = new Vector3(leftRenderer.bounds.size.x, 0, 0);
                leftRenderer.transform.position = rightRenderer.transform.position + offset;
                (leftRenderer, mainRenderer, rightRenderer) = (mainRenderer, rightRenderer, leftRenderer);
            }
        }
    }
}