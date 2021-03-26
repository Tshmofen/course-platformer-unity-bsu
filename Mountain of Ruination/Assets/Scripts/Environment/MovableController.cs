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
        public InteractPointer pointer;
        public PlayerManager manager;
        
        private List<Movable> _movables;
        private Movable _currentMovable;
        private Movable _minMovable;
        private bool _isCurrentSet;
        private bool _isMinSet;

        #endregion

        #region Unity calls

        private void Start()
        {
            if (movableLayer  == default) movableLayer = LayerMask.NameToLayer("Movable");
            if (interactLayer == default) interactLayer = LayerMask.NameToLayer("MovableInteract");
            GetComponent<CircleCollider2D>().radius = radius;
            _movables = new List<Movable>();
        }
        
        private void Update()
        {
            if (!_isCurrentSet && manager.player.IsInteracting && _isMinSet)
            {
                _currentMovable = _minMovable;
                _isCurrentSet = true;
                pointer.gameObject.SetActive(true);
            }
            
            if (_isCurrentSet && !manager.player.IsInteracting)
            {
                _isCurrentSet = false;
                pointer.gameObject.SetActive(false);
            }
            
            if (_isCurrentSet)
            {
                button.transform.position = _currentMovable.transform.position;
            }
            
            if (_isMinSet && !_isCurrentSet)
            {
                button.Sprite.enabled = true;
                button.transform.position = _minMovable.transform.position;
            }
        }
        
        private void FixedUpdate()
        {
            if (_movables.Count == 0) return;

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
            
            _minMovable = minMovable;
            _isMinSet = true;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Tag.Movable)) return;
            var movable = other.GetComponent<Movable>();
            if (movable != null) _movables.Add(movable);
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(Tag.Movable)) return;
            var movable = other.GetComponent<Movable>();
            if (movable != null) _movables.Remove(movable);
        }

        #endregion
    }
}