using System.Collections.Generic;
using Entity.Manager;
using Environment.Interactive;
using Interface.World;
using UnityEngine;
using Util;

namespace Entity.Player
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class MovableController : MonoBehaviour
    {
        #region Fields and properties

        [Header("Interacting")]
        public float radius = 3;
        public LayerMask hitLayers;
        public float interactBuffer = 1.5f;
        [Header("External")]
        public InteractButton button;
        public InteractPointer pointer;
        public PlayerManager manager;
        public bool isLocked;
        
        private List<Movable> _movables;
        private Movable _currentMovable;
        private Movable _minMovable;
        private bool _isCurrentSet;
        private bool _isMinSet;

        #endregion

        #region Unity behaviour

        private void Start()
        {
            GetComponent<CircleCollider2D>().radius = radius;
            _movables = new List<Movable>();
        }
        
        private void Update()
        {
            if (!_isMinSet && !_isCurrentSet || !IsPointCanReachPlayer(_minMovable.transform.position))
            {
                button.Sprite.enabled = false;
            }
            
            if (!_isCurrentSet && manager.player.IsInteracting && _isMinSet)
            {
                _currentMovable = _minMovable;
                _currentMovable.GravitationLocked = true;
                _currentMovable.ChangeLayer(true);
                _isCurrentSet = true;
                pointer.gameObject.SetActive(true);
            }
            
            if (_isCurrentSet && !manager.player.IsInteracting)
            {
                _isCurrentSet = false;
                _currentMovable.PlayerTransform = manager.player.transform;
                _currentMovable.GravitationLocked = false;
                _currentMovable.ChangeLayer(false);
                pointer.gameObject.SetActive(false);
            }
            
            if (_isCurrentSet)
            {
                var movablePosition = _currentMovable.transform.position;
                button.transform.position = movablePosition;
                _currentMovable.Move(pointer.transform.position);
                UpdatePlayerInteracting(movablePosition);
            }
            
            if (_isMinSet && !_isCurrentSet)
            {
                button.Sprite.enabled = true;
                button.transform.position = _minMovable.transform.position;
            }
        }
        
        private void FixedUpdate()
        {
            if (_movables.Count == 0)
            {
                _isMinSet = false;
                return;
            }

            var minMovable = _movables[0];
            var minDistance = (minMovable.transform.position - transform.position).magnitude;
            foreach (var movable in _movables)
            {
                var movablePosition = movable.transform.position;
                var distance = (movablePosition - transform.position).magnitude;
                if (distance < minDistance && IsPointCanReachPlayer(movablePosition))
                {
                    minDistance = distance;
                    minMovable = movable;
                }
            }

            _isMinSet = minMovable != _movables[0] || IsPointCanReachPlayer(minMovable.transform.position);
            _minMovable = minMovable;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(TagStorage.MovableTag)) return;
            var movable = other.GetComponent<Movable>();
            if (movable != null) _movables.Add(movable);
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(TagStorage.MovableTag)) return;
            var movable = other.GetComponent<Movable>();
            if (movable != null) _movables.Remove(movable);
        }

        private void UpdatePlayerInteracting(Vector2 movablePosition)
        {
            var distance = (movablePosition - (Vector2)manager.player.transform.position).magnitude;
            manager.player.IsInteracting =
                (distance < radius + interactBuffer) && IsPointCanReachPlayer(movablePosition);
        }

        private bool IsPointCanReachPlayer(Vector2 point)
        {
            var direction = (Vector2)manager.player.transform.position - point;
            var hit = Physics2D.Raycast(
                point,
                direction,
                direction.magnitude,
                hitLayers
            );
            return hit.collider == null;
        }

        #endregion
    }
}