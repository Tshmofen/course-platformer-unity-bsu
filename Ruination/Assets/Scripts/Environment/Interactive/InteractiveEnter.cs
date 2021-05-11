using Entity.Controller;
using Interface.Fading;
using UnityEngine;
using Util;

namespace Environment.Interactive
{
    public class InteractiveEnter : MonoBehaviour, IInteractive
    {
        private PlayerController _player;
        
        [Header("Transportation")]
        public GameObject destinationObject;
        public Vector2 offset;
        [Header("Visuals")]
        public SceneFader fader;
        public float fadeTime = 0.5f;
        public float waitTime = 0.5f;

        private void Start()
        {
            _player = GameObject.FindWithTag(TagStorage.PlayerTag).GetComponent<PlayerController>();
        }

        public void Interact()
        {
            fader.fadeTime = fadeTime;
            fader.waitTime = waitTime;
            
            fader.OnFadeAddingEnd += HandleTransportation;

            fader.StartFade(false);
        }

        private void HandleTransportation()
        {
            fader.OnFadeAddingEnd -= HandleTransportation;
            fader.StartFade();
            
            _player.transform.position = destinationObject.transform.position + (Vector3)offset;
        }
    }
}