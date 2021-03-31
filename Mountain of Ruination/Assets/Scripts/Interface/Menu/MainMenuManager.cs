using Interface.Scene;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Interface.Menu
{
    public class MainMenuManager : MonoBehaviour
    {
        #region Fields & properties
        
        public StandaloneInputModule inputModule;
        public Image loadingBackground;
        public float loadingFadeTime = 0.7f;

        #endregion

        #region Support methods
        
        public void HandleNewGame()
        {
            inputModule.enabled = false;

            var loader = gameObject.AddComponent<SceneLoader>();
            loader.background = loadingBackground;
            loader.fadeTime = loadingFadeTime;
            loader.Load("PrologueScene");
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