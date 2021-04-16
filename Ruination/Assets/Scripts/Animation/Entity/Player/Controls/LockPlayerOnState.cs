using Entity.Player;
using UnityEngine;

namespace Animation.Entity.Player.Controls
{
    public class LockPlayerOnState : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<PlayerManager>().player.IsLocked = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<PlayerManager>().player.IsLocked = false;
        }
    }
}