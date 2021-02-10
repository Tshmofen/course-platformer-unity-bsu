using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Interface
{
    [RequireComponent(typeof(Text))]
    public class TemporaryText : MonoBehaviour
    {
        #region Fields and properties

        private float currentTime;
        private Vector3 startPosition;
        private Vector3 targetPosition;

        private CanvasRenderer textRender;
        private RectTransform rect;

        public float lifeTime;

        #endregion

        #region Unity calls

        private void Start()
        {
            textRender = GetComponent<CanvasRenderer>();
            rect = GetComponent<RectTransform>();

            startPosition = rect.position;
            targetPosition = rect.position;
            targetPosition.y += rect.rect.height;
        }

        private void Update()
        {
            UpdateRenderState();   
        }

        #endregion

        #region Update parts

        private void UpdateRenderState()
        {
            currentTime += Time.deltaTime;

            float opacity = Mathf.Lerp(1, 0, currentTime / lifeTime);
            Vector3 position = Vector3.Lerp(rect.position, targetPosition, currentTime / lifeTime);

            textRender.SetAlpha(opacity);
            rect.position = position;

            if (opacity == 0)
            {
                Destroy(this.gameObject);
            }
        }

        #endregion
    }
}