using System;
using Interface.Fading;
using UnityEngine;
using Util;

namespace Environment.Interactive
{
    [RequireComponent(typeof(Animator))]
    public class InteractiveEnter : MonoBehaviour, IInteractive
    {
        private Animator _animator;
        
        [Header("Transportation")]
        public GameObject destinationObject;
        public Vector2 offset;
        [Header("Visuals")] public string transportParameter = "toTransport";
        public bool toFadeScene = true;
        public SceneFader fader;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void Interact()
        {
            
        }

        public void StartFading(bool invert = false)
        {
            
        }
        
        public void TransportPlayer()
        {
            var player = GameObject.FindWithTag(TagStorage.PlayerTag);
            player.transform.position = destinationObject.transform.position + (Vector3)offset;
        }
    }
}