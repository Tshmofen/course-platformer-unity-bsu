using Entity.Player;
using UnityEngine;

namespace Animation.Entity.Player.Aim
{
    public class AimOnEnterRestorer : StateMachineBehaviour
    {
        #region Unity calls

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = animator.GetComponent<PlayerManager>().player;
            controller.actualAim.RestorePosition(controller.IsFacingRight);
        }

        #endregion
    }
}