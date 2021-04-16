using Entity.Player;
using UnityEngine;

namespace Animation.Entity.Player.Aim
{
    public class AimOnEnterRestorer : StateMachineBehaviour
    {
        #region Unity calls

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var player = animator.GetComponent<PlayerManager>().player;
            player.RestoreAimPosition();
        }

        #endregion
    }
}