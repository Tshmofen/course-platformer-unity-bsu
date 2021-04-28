using UnityEngine;

namespace Environment.Furniture
{
    public class BreakingPlatform : MonoBehaviour
    {
        public float timeToStartBreak = 0.3f;
        public Animator animator;
        public string breakTrigger = "toBroke";

        private float _currentTime;
        private int _hashBreak;
        private int _enters;

        private void Start()
        {
            _hashBreak = Animator.StringToHash(breakTrigger);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            var otherBounds = other.bounds;
            var bounds = GetComponent<Collider2D>().bounds;
            
            var otherLowEdge = otherBounds.center.y - otherBounds.extents.y;
            var lowEdge = bounds.center.y - bounds.extents.y;
            if (otherLowEdge < lowEdge)
                return;

            if (_currentTime > timeToStartBreak)
            {
                animator.SetTrigger(_hashBreak);
                _currentTime = 0;
            }
            else 
            {
                _currentTime += Time.deltaTime;
            }
        }

        private void OnTriggerEnter2D(Collider2D _) => _enters++;

        private void OnTriggerExit2D(Collider2D _)
        {
            _enters--;
            if (_enters == 0) _currentTime = 0;
        }
    }
}