using UnityEngine;

namespace Environment.Furniture
{
    public class AnimationActivator : MonoBehaviour
    {
        private float _currentWaitTime;
        private int _hashBoolParameter;
        
        public float timeToActivate = 2;
        public string activatorTag = "Activator";
        public Animator animator;
        public string boolParameter = "isOpen";

        private void Start()
        {
            _hashBoolParameter = Animator.StringToHash(boolParameter);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag(activatorTag)) return;
            
            if (_currentWaitTime >= timeToActivate)
                animator.SetBool(_hashBoolParameter, true);
            else
                _currentWaitTime += Time.deltaTime;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(activatorTag)) return;
            
            _currentWaitTime = 0;
            animator.SetBool(_hashBoolParameter, false);
        }
    }
}