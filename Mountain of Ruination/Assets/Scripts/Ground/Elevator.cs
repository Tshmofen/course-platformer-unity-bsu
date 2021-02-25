using System.Collections.Generic;
using UnityEngine;

namespace Ground
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Elevator : MonoBehaviour
    {
        #region Fields

        private List<GameObject> _attached;
        
        private BoxCollider2D _collider;
        private ContactFilter2D _filter;

        private float _heightDifference;
        private float _currentHeight;
        private bool _movingUp = true;

        #endregion
        
        #region Unity assigns

        [Header("Movement")]
        public float moveHeight;
        public float speed = 1;
        [Header("Attachment")] 
        public LayerMask attachLayer;
        public float attachHeight;
        public int attachBufferSize = 16;

        #endregion

        #region Unity call

        private void Start()
        {
            _attached = new List<GameObject>();
            _collider = GetComponent<BoxCollider2D>();
            _filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = attachLayer
            };
        }

        public void FixedUpdate()
        {
            ResetAttachments();
        }

        public void Update()
        { 
            UpdatePosition();
            UpdateAttachedPosition();
        }

        #endregion

        #region Update parts

        private void UpdatePosition()
        {
            _heightDifference = _currentHeight;
            
            if (_movingUp && _currentHeight > moveHeight || !_movingUp && _currentHeight < 0)
                _movingUp = !_movingUp;

            if (_movingUp)
            {
                transform.position += Vector3.up * (speed * Time.deltaTime);
                _currentHeight += speed * Time.deltaTime;
            }
            else
            {
                transform.position += Vector3.down * (speed * Time.deltaTime);
                _currentHeight -= speed * Time.deltaTime;
            }
            
            _heightDifference -= _currentHeight;
            _heightDifference *= -1;
        }

        private void UpdateAttachedPosition()
        {
            foreach (var obj in _attached)
            {
                obj.transform.position += Vector3.up * _heightDifference;
            }
            Debug.Log(_attached.Count);
        }
        

        private void ResetAttachments()
        {
            _attached.Clear();
            var hits = new RaycastHit2D[attachBufferSize];
            var size = _collider.Cast(Vector2.up, _filter, hits, attachHeight);
            for (var i = 0; i < size; i++)
            {
                _attached.Add(hits[i].collider.gameObject);
            }
        }

        #endregion
    }
}