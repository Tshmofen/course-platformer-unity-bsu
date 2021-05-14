using System.Collections.Generic;
using UnityEngine;

namespace Environment.Follower
{
    public class Attacher : MonoBehaviour
    {
        private List<GameObject> _attached;
        private Vector3 _previousPosition;

        [Header("Attachment")] 
        public LayerMask attachLayer;

        private void Start()
        {
            _attached = new List<GameObject>();
            _previousPosition = transform.position;
        }

        private void Update() => UpdateAttachedPositions();

        // reposition attached objects to follow the elevator
        private void UpdateAttachedPositions()
        {
            var difference = transform.position - _previousPosition;
            
            foreach (var obj in _attached)
                obj.transform.position += difference;
            
            _previousPosition = transform.position;
        }
        
        // manages anything on the elevator that should move with it
        private void OnTriggerEnter2D(Collider2D other)
        {
            var isOtherInLayerMask = attachLayer == (attachLayer | (1 << other.gameObject.layer));
            if (isOtherInLayerMask) _attached.Add(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var isOtherInLayerMask = attachLayer == (attachLayer | (1 << other.gameObject.layer));
            if (isOtherInLayerMask) _attached.Remove(other.gameObject);
        }
    }
}