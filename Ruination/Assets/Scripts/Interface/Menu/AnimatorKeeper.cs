using System;
using UnityEngine;

namespace Interface.Menu
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