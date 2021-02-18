using Assets.Scripts.Entity;
using UnityEngine;

namespace Assets.Scripts.Damage
{
    [RequireComponent(typeof(BoxCollider2D), typeof(DamageStats))]
    public class DamageDeliver : MonoBehaviour
    {
        #region Fields and properties

        [Header("Combat")]
        public bool isInAttack;
        [Header("External")]
        public EntityManager manager;

        public DamageType Type { get; set; }
        public BoxCollider2D DamageCollider { get; set; }
        public DamageStats Stats { get; set; }

        #endregion

        #region Unity calls

        private void Start()
        {
            DamageCollider = GetComponent<BoxCollider2D>();
            DamageCollider.enabled = false;
            Stats = GetComponent<DamageStats>();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            DamageReceiver receiver = collider.GetComponent<DamageReceiver>();
            if (receiver != null && isInAttack)
            {
                receiver.ReceiveDamage(Stats, Type);
            }
        }

        #endregion

        #region Public

        public void ToAttack()
        {
            if (!isInAttack)
            {
                manager.animator.SetTrigger("toAttack");
            }
            manager.animator.SetBool("isInAttack", isInAttack);
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