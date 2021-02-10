using Assets.Scripts.Damage;
using UnityEngine;

namespace Assets.Scripts.Entity.Animation
{
    public class NoDamageOnState : StateMachineBehaviour
    {
        #region Unity calls

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<DamageReceiver>().IsReceiveDamage = false;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<DamageReceiver>().IsReceiveDamage = true;
        }

        #endregion
    }
}