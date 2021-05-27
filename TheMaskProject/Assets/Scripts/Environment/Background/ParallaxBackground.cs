using System.Collections.Generic;
using Entity;
using UnityEngine;


namespace Environment.Background
{
    public class ParallaxBackground : MonoBehaviour
    {
        private Vector2 _oldPosition;
        private List<ParallaxLayer> _parallaxLayers;

        [Header("Cameras")]
        public GameObject cameraObject;
        public CameraUpdater cameraUpdater;

        private void Start()
        {
            _oldPosition = cameraObject.transform.position;
            _parallaxLayers = new List<ParallaxLayer>();
            SetLayers();
            
            cameraUpdater.OnCameraUpdateEnd += UpdatePosition;
        }

        private void UpdatePosition()
        {
            var newPosition = (Vector2)cameraObject.transform.position;
            transform.position = newPosition;
            var delta = _oldPosition - newPosition;
            _oldPosition = newPosition;
            
            if (delta.x != 0) MoveLayers(delta.x, true);
            if (delta.y != 0) MoveLayers(delta.y, false);
        }

        // should be called, when changed amount of layers
        private void SetLayers()
        {
            _parallaxLayers.Clear();
            for (var i = 0; i < transform.childCount; i++)
            {
                var layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

                if (layer != null)
                {
                    _parallaxLayers.Add(layer);
                }
            }
        }

        private void MoveLayers(float delta, bool xDelta)
        {
            foreach (var layer in _parallaxLayers)
                if (xDelta)
                    layer.MoveX(delta);
                else
                    layer.MoveY(delta);
        }
    }
}