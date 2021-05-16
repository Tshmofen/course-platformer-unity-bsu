using UnityEngine;

namespace Interface.Overlay
{
    // Used to preserve animation state of menu buttons on disable/enable
    [RequireComponent(typeof(Animator))]
    public class AnimatorKeeper : MonoBehaviour
    {
        private static readonly int HashObjectDisabled = Animator.StringToHash("ObjectWasDisabled");

        private void Start()
        {
            GetComponent<Animator>().keepAnimatorControllerStateOnDisable = true;
        }

        private void OnDisable()
        {
            GetComponent<Animator>().SetTrigger(HashObjectDisabled);
        }
    }
}