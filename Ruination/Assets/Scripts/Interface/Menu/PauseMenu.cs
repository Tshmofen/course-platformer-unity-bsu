using Interface.Fading;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Util;

namespace Interface.Menu
{
    public class PauseMenu : BaseMenu
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
        
        private GameObject _lastButton;
        private bool _wasEnabled;

        #endregion

        #region Unity calls
        
        private void Start()
        {
            _lastButton = mainButton;
            IsEnabled = true;
            EnableMenu(false);
        }

        // called by a button
        public void HandleContinue()
        {
            EnableMenu(false);
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

        #region Public

        public override void EnableMenu(bool enable)
        {
            _wasEnabled = IsEnabled;
            IsEnabled = enable;
            
            if (_wasEnabled && enable || !_wasEnabled && !enable)
                return;

            Time.timeScale = enable ? 0 : 1;
            menuObject.SetActive(enable);
            
            Cursor.lockState = (enable) ? CursorLockMode.None : CursorLockMode.Locked;
            if (_wasEnabled && !enable) _lastButton = eventSystem.currentSelectedGameObject;
            if (!_wasEnabled && enable) eventSystem.SetSelectedGameObject(_lastButton);
        }
        
        public override bool GetMenuControls() => InputUtil.GetPauseMenu();

        #endregion
    }
}