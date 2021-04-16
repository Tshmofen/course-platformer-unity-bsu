using UnityEngine;

namespace Interface.Overlay
{
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