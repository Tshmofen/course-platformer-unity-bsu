using UnityEngine;
using Util;

namespace Environment
{
    public class MovableController : MonoBehaviour
    {
        #region Fields and properties

        #region Unity assigns
        
        public GameObject playerInteract;
        public Rigidbody2D body;

        #endregion
        
        private Vector3 _lastPosition;

        #endregion

        #region Unity calls

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject != playerInteract)
            {
                _lastPosition = playerInteract.transform.position;
                return;
            }
            
            if (InputUtil.GetInteract())
            {
                var diff =  playerInteract.transform.position - _lastPosition;
                body.MovePosition(transform.position + diff);
            }

            _lastPosition = playerInteract.transform.position;

        }

        private void OnTriggerExit(Collider other)
        {
            
        }

        #endregion
        
    }
}