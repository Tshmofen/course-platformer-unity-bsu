using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Interface.Menu
{
    public class MainMenuManager : MonoBehaviour
    {
        public Button initialButton;

        private void Start()
        {
            initialButton.Select();
        }

        public void HandleNewGame()
        {
            SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
        }

        public void HandleOptions()
        {

        }

        public void HandleExit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif

            Application.Quit();
        }
    }
}