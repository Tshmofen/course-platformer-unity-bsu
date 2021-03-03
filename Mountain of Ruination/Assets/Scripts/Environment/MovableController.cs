using Entity.Movement;
using Entity.Player;
using UnityEngine;
using Util;

namespace Environment
{
    public class MovableController : MonoBehaviour
    {
        #region Fields and properties

        #region Unity assigns

        public float gravity = 20;
        public PlayerController player;
        public MovementController movement;

        #endregion

        private Vector2 _velocity;

        public float VelocityX
        {
            get => _velocity.x;
            set => _velocity.x = value;
        }
        public float VelocityY
        {
            get => _velocity.y;
            set => _velocity.y = value;
        }
        
        #endregion

        #region Unity calls

        private void OnTriggerStay2D(Collider2D other)
        {
            if (InputUtil.GetInteract())
            {
                player.CurrentMovable = this;
                player.IsInteractWithMovable = true;
            }
            else
            {
                player.IsInteractWithMovable = false;
                _velocity.x = 0;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            player.IsInteractWithMovable = false;
            _velocity.x = 0;
        }

        private void Update()
        {
            _velocity.y -= gravity * Time.deltaTime;
            movement.Move(_velocity * Time.deltaTime);
            _velocity = movement.Velocity;
        }

        #endregion
    }
}