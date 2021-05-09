using Entity.Manager;
using UnityEngine;

namespace Animation.Entity.Enemy
{
    public class LockEnemyOnState : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<EnemyManager>().enemy.isLocked = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<EnemyManager>().enemy.isLocked = false;
        }
    }
}