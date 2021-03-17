using System.Collections.Generic;
using Entity.Player;
using Interface.World;
using UnityEngine;
using Util;

namespace Environment
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class MovableController : MonoBehaviour
    {
        #region Fields and properties

        public LayerMask movableLayer;
        public LayerMask interactLayer;
        public float radius = 3;
        [Header("External")]
        public InteractButton button;
        public PlayerManager manager;
        
        private List<Movable> _movables;
        private Movable _currentMovable;
        private bool _isCurrentMovableSet;
        
        public bool IsCurrentMovableLocked { get; set; }

        #endregion

        #region Unity calls

        private void Start()
        {
            if (movableLayer  == default) movableLayer = LayerMask.NameToLayer("Movable");
            if (interactLayer == default) interactLayer = LayerMask.NameToLayer("MovableInteract");
            GetComponent<CircleCollider2D>().radius = radius;
            _movables = new List<Movable>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Tag.Movable)) return;
            var movable = other.GetComponent<Movable>();
            if (movable != null) _movables.Add(movable);
        }

        private void FixedUpdate()
        {
            if (IsCurrentMovableLocked) return;
            
            if (_movables.Count == 0)
            {
                _isCurrentMovableSet = false;
                return;
            }

            var minMovable = _movables[0];
            var minDistance = (minMovable.transform.position - transform.position).magnitude;
            foreach (var movable in _movables)
            {
                var distance = (movable.transform.position - transform.position).magnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minMovable = movable;
                }
            }
            
            _currentMovable = minMovable;
            _isCurrentMovableSet = true;
        }

        private void Update()
        {
            if (!_isCurrentMovableSet)
            {
                SwitchButton(false);
                manager.player.IsMovableAvailable = false;
                return;
            }

            SwitchButton(true);
            button.transform.position = _currentMovable.transform.position;
            
            manager.player.CurrentMovable = _currentMovable;
            manager.player.IsMovableAvailable = true;
            _currentMovable.GravitationLocked = IsCurrentMovableLocked;
            _currentMovable.gameObject.layer = (IsCurrentMovableLocked) ? interactLayer.value : movableLayer.value;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(Tag.Movable)) return;
            var movable = other.GetComponent<Movable>();
            if (movable != null) _movables.Remove(movable);
        }

        #endregion

        #region Support Methods

        private void SwitchButton(bool enable)
        {
            button.Sprite.enabled = enable;
        }

        #endregion
    }
}