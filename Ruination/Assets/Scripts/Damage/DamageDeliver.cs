using UnityEngine;

namespace Damage
{
    [RequireComponent(typeof(Collider2D))]
    public class DamageDeliver : MonoBehaviour
    {
        public DamageType type = DamageType.LightDamage;
        public DamageStats stats;

        private Collider2D _damageCollider;

        private void Start()
        {
            _damageCollider = GetComponent<Collider2D>();
            _damageCollider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D otherCollider)
        {
            var receiver = otherCollider.GetComponent<DamageReceiver>();
            if (receiver != null) receiver.ReceiveDamage(stats, type);
        }
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