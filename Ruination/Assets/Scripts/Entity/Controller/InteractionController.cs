using System.Collections.Generic;
using Environment.Interactive;
using UnityEngine;
using Util;

namespace Entity.Controller
{
    public class InteractionController : MonoBehaviour
    {
        #region Fields & properties

        private List<AbstractInteractive> _interacts;

        public GameObject interactButton;
        
        #endregion


        #region Unity calls
        
        private void Start()
        {
            _interacts = new List<AbstractInteractive>();
            interactButton.SetActive(false);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_interacts.Count == 0) return;
            
            interactButton.SetActive(true);
            if (InputUtil.GetInteract())
                _interacts[0].Interact();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var interactive = other.GetComponent<AbstractInteractive>();
            if (interactive != null)
            {
                _interacts.Add(interactive);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var interactive = other.GetComponent<AbstractInteractive>();
            if (interactive != null)
            {
                _interacts.Remove(interactive);
                if (_interacts.Count == 0) interactButton.SetActive(false);
            }
        }
        
        #endregion
    }
}