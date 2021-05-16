using Entity.Player;
using UnityEngine;
using Util;

namespace Animation.Entity.Player
{
    public class LockPlayer : StateMachineBehaviour
    {
        private PlayerController _player;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var playerObject = GameObject.FindWithTag(TagStorage.PlayerTag);
            if (playerObject != null)
            {
                _player = playerObject.GetComponent<PlayerController>();
                _player.Lock();
                return;
            }
            
            Debug.Log("Found scene without player!");
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_player != null)
                _player.Lock();
        }
    }
}