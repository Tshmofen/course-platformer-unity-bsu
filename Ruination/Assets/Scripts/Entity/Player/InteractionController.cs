using System;
using System.Collections.Generic;
using Environment.Interactive;
using UnityEngine;

namespace Entity.Player
{
    public class InteractionController : MonoBehaviour
    {
        private List<InteractiveObject> _interactiveObjects;

        private void Start()
        {
            _interactiveObjects = new List<InteractiveObject>();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var interactive = other.GetComponent<InteractiveObject>();
            if (interactive != null)
            {
                _interactiveObjects.Add(interactive);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var interactive = other.GetComponent<InteractiveObject>();
            if (interactive != null)
            {
                _interactiveObjects.Remove(interactive);
            }
        }
    }
}