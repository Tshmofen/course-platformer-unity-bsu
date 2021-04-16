using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Interface.Scene
{
    public class SceneLoader : MonoBehaviour
    {
        #region Fields & properties

        [Header("Loading visuals")] 
        public float fadeTime = 0.3f;
        public Image background;

        private string _scene;
        private float _currentFade;
        private bool _toFade;
        private bool _toLoad;
        
        #endregion

        #region Unity calls

        private void Start()
        {
            _currentFade = 0;
        }

        private void Update()
        {
            if (_toFade)
            {
                background.enabled = true;
                _currentFade += Time.unscaledDeltaTime;
                if (_currentFade >= fadeTime)
                {
                    _currentFade = fadeTime;
                    _toFade = false;
                    _toLoad = true;
                }
                
                var color = background.color;
                color.a = _currentFade / fadeTime;
                background.color = color;
            }

            if (_toLoad)
            {
                SceneManager.LoadSceneAsync(_scene);
            }
        }

        #endregion

        #region Public

        public void Load(string scene)
        {
            _scene = scene;
            _toFade = true;
        }

        #endregion
    }
}