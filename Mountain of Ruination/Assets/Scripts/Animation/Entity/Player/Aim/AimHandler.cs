using Entity.Player;
using UnityEngine;

namespace Animation.Entity.Player.Aim
{
    public class AimHandler : StateMachineBehaviour
    {
        #region Unity calls

        // return aim to default position in the middle of animation to not interrupt its end or start
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var state = stateInfo.normalizedTime % 1; // this fraction displays percent of execution
            if (state > 0.45 || state > 0.55)
            {
                var player = animator.GetComponent<PlayerManager>().player;
                player.RestoreAimPosition();
            }
        }

        #endregion
    }
}