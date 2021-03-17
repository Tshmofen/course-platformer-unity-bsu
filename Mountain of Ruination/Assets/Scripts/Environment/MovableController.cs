using System.Collections.Generic;
using Entity.Player;
using Interface.World;
using UnityEngine;
using Util;

namespace Environment
{
    public class MovableController : MonoBehaviour
    {
        #region Fields and properties
        
        [Header("External")]
        public InteractButton button;
        public PlayerManager manager;
        
        private List<Movable> _movables;
        private Movable _currentMovable;
        private bool _isCurrentMovableSet;
        
        public bool IsCurrentMovableLocked { get; set; }
        public bool MovableChanged { get; private set; }

        #endregion

        #region Unity calls

        private void Start()
        {
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
            
            MovableChanged = minMovable != _currentMovable;
            if (IsCurrentMovableLocked && _movables.Contains(_currentMovable)) return;
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