using Entity;
using Entity.Manager;
using UnityEngine;

namespace Animation.DamageReceiver
{
    public class DestroyOnState : StateMachineBehaviour
    {
        #region Fields

        private EntityManager _manager;

        [Range(0,1)] public float destroyTime = 1;
        [Range(0,1)] public float timeTolerance = 0.05f;

        #endregion
        
        #region Unity calls

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _manager = animator.GetComponent<EntityManager>();
            var destroyer = animator.GetComponent<EntityManager>().destroyer;
            destroyer.EnableDeathMaterial();
        }

        // call health methods that will call general destroy entity method
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // this fraction displays percent of execution
            var state = stateInfo.normalizedTime % 1;
            
            if (state > destroyTime - timeTolerance || state > destroyTime + timeTolerance)
                _manager.health.DestroyReceiver();
            
            _manager.destroyer.CurrentFade = 1 - state;
        }

        #endregion
    }
}