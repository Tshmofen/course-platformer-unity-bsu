using Entity;
using UnityEngine;

namespace Animation.DamageReceiver
{
    public class DestroyOnStateExit : StateMachineBehaviour
    {
        #region Unity calls

        // call health methods that will call general destroy entity method
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<EntityManager>().health.DestroyReceiver();
        }

        #endregion
    }
}