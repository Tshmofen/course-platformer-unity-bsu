
using Assets.Scripts.Entity;
using Assets.Scripts.Entity.Player;
using UnityEngine;

namespace Assets.Scripts.Animation.Entity.Aim
{
    public class AimHandler : StateMachineBehaviour
    {
        #region Unity calls

        // aim hiding
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator
                .GetComponent<PlayerManager>()
                .player
                .DisplayActualAim = false;
        }

        // return aim to default position in the middle of animation to not interrupt its end or start
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float state = stateInfo.normalizedTime % 1; // its fraction displays procent of execution
            if (state > 0.45 || state > 0.55)
            {
                PlayerController player = animator.GetComponent<PlayerManager>().player;
                player.actualAim.RestorePosition(player.IsFacingRight);
            }    
        }

        // aim showing
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController player = animator.GetComponent<PlayerManager>().player;
            player.DisplayActualAim = true;
            player.actualAim.RestorePosition(player.IsFacingRight);
        }

        #endregion
    }
}