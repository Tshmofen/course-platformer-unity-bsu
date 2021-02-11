using Assets.Scripts.Entity.Player;
using UnityEngine;

namespace Assets.Scripts.Animation.Actor.Attack
{
    public class AimHandler : StateMachineBehaviour
    {
        #region Unity calls

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController controller = animator.GetComponent<PlayerController>();
            controller.DisplayActualAim = false;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController controller = animator.GetComponent<PlayerController>();
            controller.DisplayActualAim = true;
            controller.actualAim.RestorePosition(controller.IsFacingRight);
            animator.SetTrigger("toAim");
        }

        #endregion
    }
}