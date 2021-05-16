using System;
using UnityEngine;

namespace Environment.Background
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CameraFiller : MonoBehaviour
    {
        private Sprite _sprite;
        private float _previousSize;
        
        public Camera fillCamera;

        private void Start()
        {
            _sprite = GetComponent<SpriteRenderer>().sprite;
            _previousSize = fillCamera.orthographicSize;
            FillView();
        }

        private void FixedUpdate()
        {
            var size = fillCamera.orthographicSize;
            if (Math.Abs(size - _previousSize) > 0.01f) FillView(); 
            _previousSize = size;
        }
        
        private void FillView()
        {
            var width = _sprite.bounds.size.x;
            var height = _sprite.bounds.size.y;
     
            var worldScreenHeight = fillCamera.orthographicSize * 2.0f;
            var worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;

            var newScale = new Vector2(worldScreenWidth / width, worldScreenHeight / height) * 1.1f;
            transform.localScale = newScale;
        }
    }
}