using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Interface.Menu
{
    public class MainMenuManager : MonoBehaviour
    {
        public void StartNewGame()
        {
            SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
        }

        public void ExitTheGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif

            Application.Quit();
        }
    }
}