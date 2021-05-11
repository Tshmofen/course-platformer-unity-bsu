using Entity.Controller;
using Entity.Manager;
using UnityEngine;

namespace Animation.Entity
{
    public class LockEntityOnState : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponentInChildren<BaseEntityController>().isLocked = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponentInChildren<BaseEntityController>().isLocked = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponentInChildren<BaseEntityController>().isLocked = false;
        }
    }
}