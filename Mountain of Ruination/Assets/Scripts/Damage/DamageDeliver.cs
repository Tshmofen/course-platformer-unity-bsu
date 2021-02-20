using Entity;
using UnityEngine;

namespace Damage
{
    [RequireComponent(typeof(BoxCollider2D), typeof(DamageStats))]
    public class DamageDeliver : MonoBehaviour
    {
        #region Public

        // toggle animation, that should handle collider enabling and isInAttack bool
        public void ToAttack()
        {
            if (!isInAttack) manager.animator.SetTrigger(Attack);
            manager.animator.SetBool(IsInAttack, isInAttack);
        }

        #endregion

        #region Fields and properties

        [Header("Combat")] 
        public bool isInAttack;
        [Header("External")] 
        public EntityManager manager;

        // animation hashed strings
        private static readonly int IsInAttack = Animator.StringToHash("isInAttack");
        private static readonly int Attack = Animator.StringToHash("toAttack");

        public DamageType Type { get; set; }
        private BoxCollider2D DamageCollider { get; set; }
        private DamageStats Stats { get; set; }

        #endregion

        #region Unity calls

        private void Start()
        {
            DamageCollider = GetComponent<BoxCollider2D>();
            DamageCollider.enabled = false;
            Stats = GetComponent<DamageStats>();
        }

        private void OnTriggerEnter2D(Collider2D otherCollider)
        {
            var receiver = otherCollider.GetComponent<DamageReceiver>();
            if (receiver != null && isInAttack) receiver.ReceiveDamage(Stats, Type);
        }

        #endregion
    }

    #region Support enum

    public enum DamageType
    {
        LightDamage,
        HeavyDamage,
        PierceDamage
    }

    #endregion
}