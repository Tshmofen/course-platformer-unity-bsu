using Entity.Player;
using Interface.Fading;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
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
        public string interactText = "Enter";
        public SceneFader fader;
        public float fadeTime = 0.5f;
        public float waitTime = 0.5f;
        public Light2D globalLight;
        public float newLightIntensity = 1;
        [Header("Audio")] 
        public AudioSource doorSound;
        [Header("External")] 
        public InteractionController interaction;

        private void Start()
        {
            _player = GameObject.FindWithTag(TagStorage.PlayerTag).GetComponent<PlayerController>();
        }

        public string InteractText => interactText;

        public void Interact()
        {
            interaction.isLocked = true;
            
            fader.fadeTime = fadeTime;
            fader.waitTime = waitTime;
            fader.StartFade(false);
            fader.OnFadeAddingEnd += HandleTransportation;
            
            doorSound.Play();
        }

        private void HandleTransportation()
        {
            _player.transform.position = destinationObject.transform.position + (Vector3)offset;
            globalLight.intensity = newLightIntensity;
            
            void Unlock()
            {
                fader.OnFadeRemovingEnd -= Unlock;
                interaction.isLocked = false;
            }
            
            fader.OnFadeAddingEnd -= HandleTransportation;
            fader.OnFadeRemovingEnd += Unlock;
            fader.StartFade(true);
        }
    }
}