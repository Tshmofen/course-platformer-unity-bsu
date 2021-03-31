using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Interface.Overlay
{
    [RequireComponent(typeof(Canvas))]
    public class OverlayManager : MonoBehaviour
    {
        #region Fields and properties

        private Camera _camera;
        
        public GameObject damagePopUp;
        public float popUpHeight = 0.5f;

        #endregion

        #region Unity Calls

        private void Awake()
        {
            InterfaceUtil.OverlayManager = this;
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        #endregion

        #region Public
        
        // shows popup over the target that flies away and disappears
        public void ShowPopUp(Vector2 worldPosition, string text, float lifeTime)
        {
            var newPopUp = Instantiate(damagePopUp, transform);
            newPopUp.GetComponent<Text>().text = text;
            var tempText = newPopUp.GetComponent<TemporaryText>();
            tempText.lifeTime = lifeTime;
            tempText.start = _camera.WorldToScreenPoint(worldPosition);
            worldPosition.y += popUpHeight;
            tempText.end = _camera.WorldToScreenPoint(worldPosition);
        }

        #endregion
    }
}