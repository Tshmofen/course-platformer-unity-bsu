using UnityEngine;

namespace Assets.Scripts.Damage
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DamageDeliver : MonoBehaviour
    {
        #region Fields and properties

        private BoxCollider2D damageCollider;

        public DamageStats stats;
        
        public bool isInAttack;

        public DamageType Type { get; set; }

        #endregion

        #region Unity calls

        private void Start()
        {
            damageCollider = GetComponent<BoxCollider2D>();
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            DamageReceiver receiver = collision.GetComponent<DamageReceiver>();
            if (receiver != null)
            {
                receiver.ReceiveDamage(stats, Type);
            }
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