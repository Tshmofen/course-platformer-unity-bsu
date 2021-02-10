using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Background
{
    [ExecuteInEditMode]
    public class ParallaxBackground : MonoBehaviour
    {
        #region Fiels and properties

        private Vector2 oldPosition;
        private List<ParallaxLayer> parallaxLayers;

        public Camera parallaxCamera;
        public bool movingWithCamera;

        #endregion

        #region Unity calls

        private void Start()
        {
            oldPosition = parallaxCamera.transform.position;
            parallaxLayers = new List<ParallaxLayer>();
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

            if (newPosition != oldPosition)
            {
                if (movingWithCamera)
                    transform.position = newPosition;

                if (newPosition.x != oldPosition.x)
                    MoveLayers(oldPosition.x - newPosition.x, true);
                if (newPosition.y != oldPosition.y)
                    MoveLayers(oldPosition.y - newPosition.y, false);

                oldPosition = newPosition;
            }
        }

        #endregion

        #region Support methods

        private void SetLayers()
        {
            parallaxLayers.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

                if (layer != null)
                {
                    layer.name = "Layer-" + i;
                    parallaxLayers.Add(layer);
                }
            }
        }

        private void MoveLayers(float delta, bool xDelta)
        {
            foreach (ParallaxLayer layer in parallaxLayers)
            {
                if (xDelta)
                    layer.MoveX(delta);
                else
                    layer.MoveY(delta);
            }
        }

        #endregion
    }
}