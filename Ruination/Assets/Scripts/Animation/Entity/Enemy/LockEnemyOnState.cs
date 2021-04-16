using Entity.Enemy;
using UnityEngine;

namespace Animation.Entity.Enemy
{
    public class LockEnemyOnState : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<EnemyManager>().enemy.IsLocked = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<EnemyManager>().enemy.IsLocked = false;
        }
    }
}