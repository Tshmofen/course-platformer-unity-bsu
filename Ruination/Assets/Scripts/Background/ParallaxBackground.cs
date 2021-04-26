using System;
using System.Collections.Generic;
using UnityEngine;

namespace Background
{
    [ExecuteInEditMode]
    public class ParallaxBackground : MonoBehaviour
    {
        #region Fields and properties

        // float comparison epsilon
        private const float Tolerance = 0.00001f;

        private Vector2 _oldPosition;
        private List<ParallaxLayer> _parallaxLayers;

        public Camera parallaxCamera;
        public bool movingWithCamera;

        #endregion
        
        #region Unity calls

        private void Start()
        {
            _oldPosition = parallaxCamera.transform.position;
            _parallaxLayers = new List<ParallaxLayer>();
            SetLayers();
        }

        private void Update()
        {
            UpdatePosition();
        }

        #endregion
        
        #region Update parts

        private void UpdatePosition()
        {
            Vector2 newPosition = parallaxCamera.transform.position;
            
            // on position change move layers
            if (newPosition != _oldPosition)
            {
                if (movingWithCamera)
                    transform.position = newPosition;

                if (Math.Abs(newPosition.x - _oldPosition.x) > Tolerance)
                    MoveLayers(_oldPosition.x - newPosition.x, true);
                if (Math.Abs(newPosition.y - _oldPosition.y) > Tolerance)
                    MoveLayers(_oldPosition.y - newPosition.y, false);

                _oldPosition = newPosition;
            }
        }

        #endregion
        
        #region Support methods

        // should be called, when changed number of layers
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

        #endregion
    }
}