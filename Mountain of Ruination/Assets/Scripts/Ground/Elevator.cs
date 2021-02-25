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

        private Vector2 _moveDifference;
        private float _time;
        private bool _movingUp = true;

        #endregion
        
        #region Unity assigns

        [Header("Movement")] 
        public Vector2 fromPoint;
        public Vector2 toPoint;
        public float oneWayTime = 1;
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
            _moveDifference = transform.position;

            _time += (_movingUp) ? Time.deltaTime : -Time.deltaTime ;
            transform.position = Vector2.Lerp(fromPoint, toPoint, _time / oneWayTime);

            if (_time > oneWayTime || _time < 0)
            {
                _time = (_time > oneWayTime) ? oneWayTime : 0;
                _movingUp = !_movingUp;
            }

            _moveDifference -= (Vector2)transform.position;
            _moveDifference *= -1;
        }

        private void UpdateAttachedPosition()
        {
            foreach (var obj in _attached)
            {
                obj.transform.position += (Vector3)_moveDifference;
            }
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