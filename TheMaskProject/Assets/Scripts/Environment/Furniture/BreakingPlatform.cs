using UnityEngine;

namespace Environment.Furniture
{
    public class BreakingPlatform : MonoBehaviour
    {
        [Header("Animation")]
        public Animator animator;
        public string breakTrigger = "toBroke";
        [Header("Audio")] 
        public AudioSource breakingSound;
        
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

            if (otherLowEdge >= lowEdge)
            {
                animator.SetTrigger(_hashBreak);
                breakingSound.Play();
            }
        }
    }
}