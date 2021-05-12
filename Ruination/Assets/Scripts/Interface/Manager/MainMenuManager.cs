using Interface.Fading;
using Interface.Menu;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Interface.Manager
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Menu")]
        public StandaloneInputModule inputModule;
        public EventSystem eventSystem;
        public OptionsMenu optionsMenu;
        public GameObject initialButton;
        [Header("Scene changer")]
        public SceneFader fader;
        public SceneAsset initialScene;
        public float loadingFadeTime = 0.7f;

        private void Start()
        {
            Time.timeScale = 1;
            eventSystem.SetSelectedGameObject(initialButton);
        }

        public void HandleNewGame()
        {
            inputModule.enabled = false;

            var loader = gameObject.AddComponent<SceneLoader>();
            loader.scene = initialScene;
            
            fader.OnFadeAddingEnd += loader.LoadScene;
            fader.fadeTime = loadingFadeTime;
            fader.StartFade(false);
        }

        public void HandleOptions() => optionsMenu.EnableMenu(true);

        public void HandleExit()
        {
            EditorApplication.isPlaying = false;
            Application.Quit();
        }
    }
}