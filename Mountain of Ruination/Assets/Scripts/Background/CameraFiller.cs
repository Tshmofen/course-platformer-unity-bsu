using System;
using UnityEngine;

namespace Background
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CameraFiller : MonoBehaviour
    {
        #region Fields

        #region Unity assigns

        public Camera fillCamera;

        #endregion

        private float _previousSize;

        #endregion

        #region Unity call

        private void Start()
        {
            FillView();
            _previousSize = fillCamera.orthographicSize;
        }

        private void FixedUpdate()
        {
            var size = fillCamera.orthographicSize;
            if (Math.Abs(size - _previousSize) > 0.01f) FillView(); 
            _previousSize = size;
        }

        #endregion

        #region Support Methods

        private void FillView()
        {
            var sprite = GetComponent<SpriteRenderer>().sprite;

            var width = sprite.bounds.size.x;
            var height = sprite.bounds.size.y;
     
            var worldScreenHeight = fillCamera.orthographicSize * 2.0f;
            var worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;

            transform.localScale = new Vector2(worldScreenWidth / width, worldScreenHeight / height) * 1.1f;
        }

        #endregion
    }
}