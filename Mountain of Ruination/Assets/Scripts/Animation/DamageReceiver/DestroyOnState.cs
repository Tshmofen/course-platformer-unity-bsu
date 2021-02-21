using Entity;
using UnityEngine;

namespace Animation.DamageReceiver
{
    public class DestroyOnState : StateMachineBehaviour
    {
        #region Fields

        [Range(0,1)] public float destroyTime = 0.85f;
        [Range(0,1)] public float timeTolerance = 0.05f;

        #endregion
        
        #region Unity calls

        // call health methods that will call general destroy entity method
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var state = stateInfo.normalizedTime % 1; // this fraction displays percent of execution
            if (state > destroyTime - timeTolerance || state > destroyTime + timeTolerance)
            {
                animator.GetComponent<EntityManager>().health.DestroyReceiver();
            }
        }

        #endregion
    }
}