using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Interface.Menu
{
    public class MainMenuManager : MonoBehaviour
    {
        #region Unity assign

        public Button initialButton;

        #endregion

        #region Unity call

        private void Start()
        {
            initialButton.Select();
        }

        #endregion

        #region Support methods
        
        public void HandleNewGame()
        {
            SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
        }

        public void HandleOptions()
        {
        }

        public void HandleExit()
        {
            EditorApplication.isPlaying = false;
            Application.Quit();
        }
        
        #endregion
    }
}