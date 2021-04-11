using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Util;

namespace Interface.Menu
{
    public class PauseMenuManager : MonoBehaviour
    {
        #region Fields & properties

        public GameObject menuObject;
        public GameObject mainButton;
        public EventSystem eventSystem;

        private bool _isMenuEnabled;
        private bool _wasMenuEnabled;

        #endregion

        #region Unity calls

        private void Start()
        {
            EnableMenu(false);
        }

        private void Update()
        {
            _wasMenuEnabled = _isMenuEnabled;
            _isMenuEnabled ^= InputUtil.GetPauseMenu();
            if (_wasMenuEnabled && !_isMenuEnabled || !_wasMenuEnabled && _isMenuEnabled) 
                EnableMenu(_isMenuEnabled);
        }

        #endregion

        #region Support methods

        private void EnableMenu(bool enable)
        {
            Time.timeScale = enable ? 0 : 1;
            menuObject.SetActive(enable);
            Cursor.lockState = (enable) ? CursorLockMode.None : CursorLockMode.Locked;
        }
        
        #endregion

        #region Public

        // called by a button
        public void HandleContinue()
        {
            _isMenuEnabled = false;
            EnableMenu(_isMenuEnabled);
        }

        // called by a button
        public void HandleExit()
        {
            EditorApplication.isPlaying = false;
            Application.Quit();
        }
        
        #endregion
    }
}