using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Interface.Fading
{
    public class SceneLoader : MonoBehaviour
    {
        public SceneAsset scene;
        
        private bool _toLoad;

        public void LoadScene()
        {
            SceneManager.LoadScene(scene.name);
        }
    }
}