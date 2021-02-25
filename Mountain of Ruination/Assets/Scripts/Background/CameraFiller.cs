using UnityEngine;

namespace Background
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CameraFiller : MonoBehaviour
    {
        #region Unity assigns

        public Camera fillCamera;

        #endregion
        
        #region Unity call

        private void Start()
        {
            FillView();
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