using Entity.Player;
using UnityEngine;
using Util;

namespace Animation.Entity.Player
{
    public class UnlockPlayerAtEnd : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var playerObject = GameObject.FindWithTag(TagStorage.PlayerTag);
            if (playerObject != null)
            {
                playerObject.GetComponent<PlayerController>().Unlock();
            }
        }
    }
}