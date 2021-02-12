using Assets.Scripts.Entity;
using Assets.Scripts.Entity.Player;
using UnityEngine;

namespace Assets.Scripts.Animation.Actor.Attack
{
    public class AimOnEnterRestorer : StateMachineBehaviour
    {
        #region Unity calls

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController controller = animator.GetComponent<PlayerManager>().player;
            controller.actualAim.RestorePosition(controller.IsFacingRight);
        }

        #endregion
    }
}