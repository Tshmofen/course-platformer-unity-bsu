using Interface.Scene;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Util;

namespace Interface.Menu
{
    public class PauseMenu : AbstractMenu
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

        #endregion

        #region Unity calls
        
        private void Start()
        {
            _lastButton = mainButton;
            EnableMenu(IsMenuEnabled, WasMenuEnabled);
        }
        
        
        // called by a button
        public void HandleContinue()
        {
            IsMenuEnabled = false;
            EnableMenu(IsMenuEnabled, WasMenuEnabled);
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

        public override void EnableMenu(bool enable, bool wasEnabled)
        {
            Time.timeScale = enable ? 0 : 1;
            menuObject.SetActive(enable);
            Cursor.lockState = (enable) ? CursorLockMode.None : CursorLockMode.Locked;
            if (wasEnabled && !enable) _lastButton = eventSystem.currentSelectedGameObject;
            if (!wasEnabled && enable) eventSystem.SetSelectedGameObject(_lastButton);
        }
        
        public override bool GetMenuControls() => InputUtil.GetPauseMenu();

        #endregion
    }
}