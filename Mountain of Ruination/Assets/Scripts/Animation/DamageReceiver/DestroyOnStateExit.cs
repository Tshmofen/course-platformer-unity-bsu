using Assets.Scripts.Damage;
using UnityEngine;

namespace Assets.Scripts.Entity.Animation
{
    public class DestroyOnStateExit : StateMachineBehaviour
    {
        #region Unity calls

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<DamageReceiver>().DestroyReceiver();
        }

        #endregion
    }
}