using Entity.Player;
using UnityEngine;
using Util;

namespace Animation.Entity.Player
{
    public class LockPlayerOnStateGlobally : StateMachineBehaviour
    {
        private PlayerController _player;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var playerObject = GameObject.FindWithTag(TagStorage.PlayerTag);
            if (playerObject != null) _player = playerObject.GetComponent<PlayerController>();

            if (_player != null)
            {
                _player.Lock();
                return;
            }

            Debug.Log("Found scene without player!");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_player != null)
            {
                _player.Unlock();
            }
        }
    }
}