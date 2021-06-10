using System.Collections.Generic;
using Environment.Interactive;
using UnityEngine;
using Util;

namespace Entity.Player
{
    public class InteractionController : MonoBehaviour
    {
        private List<IInteractive> _interacts;
        private bool _skipFrame;

        [Header("External")]
        public GameObject interactButton;
        public bool isLocked;
        
        private void Start()
        {
            _interacts = new List<IInteractive>();
            interactButton.SetActive(false);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_skipFrame)
            {
                _skipFrame = false;
                return;
            }

            if (isLocked || _interacts.Count == 0)
            {
                interactButton.SetActive(false);
                return;
            }
            
            interactButton.SetActive(true);
            if (InputUtil.GetInteract())
            {
                _interacts[0].Interact();
                _skipFrame = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var interactive = other.GetComponent<IInteractive>();
            if (interactive != null) _interacts.Add(interactive);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var interactive = other.GetComponent<IInteractive>();
            if (interactive != null)
            {
                _interacts.Remove(interactive);
                if (_interacts.Count == 0) interactButton.SetActive(false);
            }
        }
    }
}