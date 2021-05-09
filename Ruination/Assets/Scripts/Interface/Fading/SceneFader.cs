using UnityEngine;
using UnityEngine.UI;

namespace Interface.Fading
{
    public class SceneFader : MonoBehaviour
    {
        #region Fields & properties

        [Header("Visuals")] 
        public Image background;
        public float fadeTime = 0.7f;
        public float waitTime = 0.3f;

        private float _currentWait;
        private float _currentFade;
        private bool _initialized;

        private float _maxDelta;
        
        #endregion

        #region Unity calls

        private void Start()
        {
            background.enabled = true;
            _currentFade = 0;
            _currentWait = 0;
        }

        private void Update()
        {
            if (Time.unscaledDeltaTime > 0.1) return;

            if (_currentFade >= fadeTime)
            {
                background.enabled = false;
                Destroy(this);
            }

            if (_currentWait < waitTime) 
                _currentWait += Time.unscaledDeltaTime;

            if (_currentWait >= waitTime)
            {
                _currentFade += Time.unscaledDeltaTime;
                if (_currentFade > fadeTime) _currentFade = fadeTime;
                
                var color = background.color;
                color.a = 1 - _currentFade / fadeTime;
                background.color = color;
            }
        }

        #endregion
    }
}