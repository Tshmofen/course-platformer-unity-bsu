using UnityEngine;

namespace Assets.Scripts.Entity.Animation
{
    public class NoDamageOnState : StateMachineBehaviour
    {
        #region Unity calls

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log(animator.gameObject.name);
            animator.GetComponent<EntityManager>().health.IsReceiveDamage = false;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<EntityManager>().health.IsReceiveDamage = true;
        }

        #endregion
    }
}