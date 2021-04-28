using System.Collections.Generic;
using UnityEngine;

namespace Environment.Furniture
{
    [RequireComponent(typeof(EdgeCollider2D))]
    public class Elevator : MonoBehaviour
    {
        #region Fields

        private Vector3 _previousPosition;
        private List<GameObject> _attached;
        
        private EdgeCollider2D _collider;
        private ContactFilter2D _filter;
        
        private float _time;
        private bool _movingUp = true;

        private Vector2 _acceleration;
        private Vector2 _maxVelocity;
        // position were acceleration are not used;
        private Vector2 _maxVelocityStart;
        private Vector2 _maxVelocityEnd;

        #endregion
        
        #region Unity assigns

        [Header("Movement")] 
        public Vector2 fromPoint;
        public Vector2 toPoint;
        public float oneWayTime = 4;
        public float waitTime = 1;
        [Range(0, 0.5f)]public float accelerationPart = 0.15f;
        [Header("Attachment")] 
        public LayerMask attachLayer;
        public float attachHeight;
        public int attachBufferSize = 16;

        #endregion

        #region Unity call

        private void Start()
        {
            _attached = new List<GameObject>();
            _collider = GetComponent<EdgeCollider2D>();
            _filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = attachLayer
            };
            // calculate how fast the velocity should be to move between points in the appointed time
            // also calculate acceleration if needed
            _maxVelocity = (toPoint - fromPoint) / (1 - accelerationPart) / oneWayTime;
            _acceleration = (accelerationPart > 0) ? _maxVelocity / (accelerationPart * oneWayTime) : Vector2.zero;

            // pre-calculate the starting points of our move function parts
            var start = accelerationPart * oneWayTime;
            _maxVelocityStart = fromPoint + _acceleration * (start * start) / 2;
            _maxVelocityEnd = toPoint - (_maxVelocityStart - fromPoint);
        }

        public void FixedUpdate()
        {
            ResetAttachments();
        }

        public void Update()
        { 
            UpdatePosition();
            UpdateAttachedPositions();
        }

        #endregion

        #region Update parts
        
        // memorize previous position and moves the elevator
        private void UpdatePosition()
        {
            _previousPosition = transform.position;
            
            var time = UpdateAndGetTime();
            transform.position = GetMoveFunction(time);
        }

        // reposition attached objects to follow the elevator
        private void UpdateAttachedPositions()
        {
            var difference = transform.position - _previousPosition;
            foreach (var obj in _attached)
            {
                obj.transform.position += difference;
            }
        }
        
        // checks if there is anything on the elevator that should move with it by casting it's collider
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

        #region Support Methods

        // update the internal time counter and return the time, excluding the wait time
        // time at first increases and the decreases to zero
        private float UpdateAndGetTime()
        {
            _time += (_movingUp) ? Time.deltaTime : -Time.deltaTime;
            
            if (_time > oneWayTime + waitTime / 2)
            {
                _time = oneWayTime + waitTime / 2;
                _movingUp = false;
            }
            
            if (_time < -waitTime / 2)
            {
                _time = -waitTime / 2;
                _movingUp = true;
            }

            if (_time > oneWayTime) return oneWayTime;
            if (_time < 0) return 0;
            return _time;
        }
        
        // this function returns position between fromPoint and toPoint
        // using three different math functions:
        // the first corresponds to accelerated motion,
        // the second to linear motion, and the third to decelerated motion.
        private Vector2 GetMoveFunction(float time)
        {
            if (time < accelerationPart * oneWayTime)
            {
                var uTime = time;
                return fromPoint + _acceleration * (uTime * uTime) / 2;
            }
            
            if (time > (1 - accelerationPart) * oneWayTime)
            {
                var dTime = time - (1 - accelerationPart) * oneWayTime;
                return _maxVelocityEnd + _maxVelocity * dTime - _acceleration * (dTime * dTime) / 2;
            }

            var sTime = time - accelerationPart * oneWayTime;
            return _maxVelocityStart + _maxVelocity * sTime;
        }

        #endregion
    }
}