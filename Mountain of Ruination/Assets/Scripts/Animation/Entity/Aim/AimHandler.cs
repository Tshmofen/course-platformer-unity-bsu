
using Assets.Scripts.Entity;
using Assets.Scripts.Entity.Player;
using UnityEngine;

namespace Assets.Scripts.Animation.Entity.Aim
{
    public class AimHandler : StateMachineBehaviour
    {
        #region Unity calls

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator
                .GetComponent<PlayerManager>()
                .player
                .DisplayActualAim = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float state = stateInfo.normalizedTime;
            state -= Mathf.Floor(state);
            if (state > 0.45 || state > 0.55)
            {
                PlayerController player = animator.GetComponent<PlayerManager>().player;
                player.actualAim.RestorePosition(player.IsFacingRight);
            }    
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController player = animator.GetComponent<PlayerManager>().player;
            player.DisplayActualAim = true;
            player.actualAim.RestorePosition(player.IsFacingRight);
        }

        #endregion
    }
}