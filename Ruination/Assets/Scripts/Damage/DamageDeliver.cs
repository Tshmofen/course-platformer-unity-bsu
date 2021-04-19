using Entity;
using UnityEngine;

namespace Damage
{
    [RequireComponent(typeof(BoxCollider2D), typeof(DamageStats))]
    public class DamageDeliver : MonoBehaviour
    {
        #region Fields and properties

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
            if (receiver != null) receiver.ReceiveDamage(Stats, Type);
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