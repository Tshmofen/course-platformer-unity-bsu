using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Interface.Menu
{
    public class MainMenuManager : MonoBehaviour
    {
        public void StartNewGame()
        {
            Debug.Log("IsCalled??/");
            SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
        }
    }
}