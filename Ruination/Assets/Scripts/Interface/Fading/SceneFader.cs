using System.Collections;
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
        private float _currentFadeTime;
        private bool _isFading;

        #endregion

        private void Start()
        {
            StartFading();
        }

        // May be called by a transport animation
        public void StartFading()
        {
            background.enabled = true;
            _currentFadeTime = 0;
            _currentWait = 0;
            if (!_isFading)
            {
                _isFading = true;
                StartCoroutine(ContinueFading());
            }
        }

        private IEnumerator ContinueFading()
        {
            while (_currentFadeTime < fadeTime)
            {
                if (Time.unscaledDeltaTime > 0.1) 
                    yield return null;

                if (_currentWait >= waitTime)
                {
                    _currentFadeTime += Time.unscaledDeltaTime;
                    if (_currentFadeTime > fadeTime) _currentFadeTime = fadeTime;
                
                    var color = background.color;
                    color.a = 1 - _currentFadeTime / fadeTime;
                    background.color = color;
                }
                else
                {
                    _currentWait += Time.unscaledDeltaTime;
                }
                
                yield return null;
            }

            _isFading = false;
            background.enabled = false;
        }
        
    }
}