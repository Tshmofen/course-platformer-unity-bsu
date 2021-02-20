using UnityEngine;
using UnityEngine.UI;

namespace Interface.Overlay
{
    [RequireComponent(typeof(Text))]
    public class TemporaryText : MonoBehaviour
    {
        #region Fields and properties

        private float _currentTime;
        private Vector3 _targetPosition;

        private CanvasRenderer _textRender;
        private RectTransform _rect;

        #region Unity assign

        // how long will the text exist and dissolve
        public float lifeTime;
        
        #endregion

        #endregion

        #region Unity calls

        private void Start()
        {
            _textRender = GetComponent<CanvasRenderer>();
            _rect = GetComponent<RectTransform>();

            _targetPosition = _rect.position;
            _targetPosition.y += _rect.rect.height;
        }

        private void Update()
        {
            UpdateRenderState();
        }

        #endregion
        
        #region Update parts

        // gradually increases sprite opacity and move it vertically 
        private void UpdateRenderState()
        {
            _currentTime += Time.deltaTime;

            var opacity = Mathf.Lerp(1, 0, _currentTime / lifeTime);
            var position = Vector3.Lerp(_rect.position, _targetPosition, _currentTime / lifeTime);

            _textRender.SetAlpha(opacity);
            _rect.position = position;

            if (opacity == 0) Destroy(gameObject);
        }

        #endregion
    }
}