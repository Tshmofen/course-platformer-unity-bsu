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
        public string initialSceneName;
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
            loader.sceneName = initialSceneName;
            
            fader.OnFadeAddingEnd += loader.LoadScene;
            fader.fadeTime = loadingFadeTime;
            fader.StartFade(false);
        }

        public void HandleOptions() => optionsMenu.EnableMenu(true);

        public void HandleExit()
        {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #endif
            
            Application.Quit();
        }
    }
}