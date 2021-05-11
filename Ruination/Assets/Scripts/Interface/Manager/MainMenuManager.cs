using Interface.Fading;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Interface.Manager
{
    public class MainMenuManager : MonoBehaviour
    {
        #region Fields & properties
        
        public StandaloneInputModule inputModule;
        public EventSystem eventSystem;
        public Image loadingBackground;
        public float loadingFadeTime = 0.7f;
        public SceneAsset initialScene;
        public GameObject initialButton;

        #endregion

        private void Start()
        {
            Time.timeScale = 1;
            eventSystem.SetSelectedGameObject(initialButton);
        }

        public void HandleNewGame()
        {
            inputModule.enabled = false;

            var loader = gameObject.AddComponent<SceneLoader>();
            loader.background = loadingBackground;
            loader.fadeTime = loadingFadeTime;
            loader.Load(initialScene.name);
        }

        public void HandleOptions()
        {
        }

        public void HandleExit()
        {
            EditorApplication.isPlaying = false;
            Application.Quit();
        }
    }
}