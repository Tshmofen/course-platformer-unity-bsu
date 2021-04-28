using System;
using Damage;
using Entity;
using UnityEngine;

namespace Environment.Interactive
{
    [RequireComponent(typeof(DamageStats))]
    public class DeathArea : MonoBehaviour
    {
        private DamageStats _stats;

        public DamageType damageType = DamageType.HeavyDamage;

        private void Start()
        {
            _stats = GetComponent<DamageStats>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var manager = other.GetComponent<EntityManager>();
            if (manager != null)
                manager.health.ReceiveDamage(_stats, damageType);
        }
    }
}