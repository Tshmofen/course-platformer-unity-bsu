using System.Collections;
using Entity.Controller;
using UnityEngine;
using Util;

namespace Animation.Entity.Player
{
    public class LockPlayerOnStateGlobally : StateMachineBehaviour
    {
        private PlayerController _player;
        private bool _isInState;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_player == null)
                _player = GameObject.FindWithTag(TagStorage.PlayerTag).GetComponent<PlayerController>();

            _isInState = true;
            _player.StartCoroutine(OverwriteTheField());
        }
        
        // there are some tricky place in unity system, i.e. we can't change field affected
        // by animator even so it's not used now by animator so we rewrite it in a late time
        // after animator already returned field in it default state
        private IEnumerator OverwriteTheField() {
            while (_isInState)
            {
                _player.isLocked = true;
                yield return new WaitForEndOfFrame();
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _isInState = false;
        }
    }
}