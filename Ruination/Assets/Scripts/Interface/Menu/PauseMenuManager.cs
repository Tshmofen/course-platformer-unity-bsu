using Interface.Scene;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Util;

namespace Interface.Menu
{
    public class PauseMenuManager : MonoBehaviour
    {
        #region Fields & properties

        [Header("Pause menu")]
        public GameObject menuObject;
        public GameObject mainButton;
        public EventSystem eventSystem;
        [Header("Menu returning")]
        public Image loadingBackground;
        public float loadingFadeTime = 0.7f;
        public SceneAsset mainMenuScene;

        private bool _isMenuEnabled;
        private bool _wasMenuEnabled;
        private GameObject _lastButton;

        #endregion

        #region Unity calls

        private void Start()
        {
            _lastButton = mainButton;
            EnableMenu(false, false);
        }

        private void Update()
        {
            _wasMenuEnabled = _isMenuEnabled;
            _isMenuEnabled ^= InputUtil.GetPauseMenu();
            if (_wasMenuEnabled && !_isMenuEnabled || !_wasMenuEnabled && _isMenuEnabled) 
                EnableMenu(_isMenuEnabled, _wasMenuEnabled);
        }
        
        // called by a button
        public void HandleContinue()
        {
            _isMenuEnabled = false;
            EnableMenu(_isMenuEnabled, _wasMenuEnabled);
        }

        // called by a button
        public void HandleMainMenu()
        {
            var loader = gameObject.AddComponent<SceneLoader>();
            loader.background = loadingBackground;
            loader.fadeTime = loadingFadeTime;
            loader.Load(mainMenuScene.name);
        }
        
        // called by a button
        public void HandleExit()
        {
            EditorApplication.isPlaying = false;
            Application.Quit();
        }

        #endregion

        #region Support methods

        private void EnableMenu(bool enable, bool wasEnabled)
        {
            Time.timeScale = enable ? 0 : 1;
            menuObject.SetActive(enable);
            Cursor.lockState = (enable) ? CursorLockMode.None : CursorLockMode.Locked;
            if (wasEnabled && !enable) _lastButton = eventSystem.currentSelectedGameObject;
            if (!wasEnabled && enable) eventSystem.SetSelectedGameObject(_lastButton);
        }
        
        #endregion
    }
}