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

        private bool _wasInteracts;
        private bool _isInteracts;
        private Vector2 _velocity;

        #endregion

        #region Unity calls

        private void OnTriggerStay2D(Collider2D other)
        {
            _isInteracts ^= InputUtil.GetInteract();
            if (_wasInteracts != _isInteracts)
                TakeControl(_isInteracts);
            _wasInteracts = _isInteracts;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            TakeControl(false);
        }

        private void Update()
        {
            UpdatePosition();
        }

        #endregion

        #region Update parts

        private void UpdatePosition()
        {
            _velocity.y -= gravity * Time.deltaTime;
            movement.Move(_velocity * Time.deltaTime);
            _velocity = movement.Velocity;
        }

        #endregion

        #region Support methods

        private void TakeControl(bool toTake)
        {
            player.IsControlTaken = toTake;
            _isInteracts = toTake;
            if (toTake)
            {
                var diff = transform.position.x - player.transform.position.x;
                if (diff < 0 && player.IsFacingRight) player.FlipDirection();
                player.ResetPlayer();
            }
            else
            {
                _velocity.x = 0;
            }
        }

        #endregion
    }
}