using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Interface.Fading
{
    public class SceneLoader : MonoBehaviour
    {
        public string sceneName;
        
        private bool _toLoad;

        public void LoadScene()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}