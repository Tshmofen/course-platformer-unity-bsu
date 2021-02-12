using UnityEngine;

namespace Assets.Scripts.Damage
{
    [RequireComponent(typeof(BoxCollider2D), typeof(DamageStats))]
    public class DamageDeliver : MonoBehaviour
    {
        #region Fields and properties
        
        public bool isInAttack;

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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("asdasd");
            DamageReceiver receiver = collision.GetComponent<DamageReceiver>();
            if (receiver != null)
            {
                receiver.ReceiveDamage(Stats, Type);
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