using Entity.Controller;
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

        public void Interact()
        {
            fader.fadeTime = fadeTime;
            fader.waitTime = waitTime;

            interaction.isLocked = true;
            fader.OnFadeAddingEnd += HandleTransportation;
            fader.StartFade(false);
        }

        private void HandleTransportation()
        {
            fader.OnFadeAddingEnd -= HandleTransportation;
            
            fader.StartFade(true);
            doorSound.Play();
            interaction.isLocked = false;
            _player.transform.position = destinationObject.transform.position + (Vector3)offset;
            globalLight.intensity = newLightIntensity;
        }
    }
}