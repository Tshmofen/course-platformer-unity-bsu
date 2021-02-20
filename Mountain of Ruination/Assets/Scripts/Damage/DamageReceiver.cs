using System.Globalization;
using Entity;
using UnityEngine;
using Util;

namespace Damage
{
    [RequireComponent(typeof(HealthStats), typeof(BoxCollider2D))]
    public class DamageReceiver : MonoBehaviour
    {
        #region Public

        public void ReceiveDamage(DamageStats damage, DamageType type)
        {
            if (IsReceiveDamage)
            {
                var damageAmount = CalculateDamage(damage, type);
                _health.CurrentHealth -= damageAmount;

                var abovePosition = GetComponent<BoxCollider2D>().bounds.size;
                abovePosition.x /= 2;
                abovePosition.y /= 2;
                InterfaceUtil
                    .OverlayManager
                    .ShowPopUp(
                        transform.position + abovePosition,
                        damageAmount.ToString(CultureInfo.InvariantCulture),
                        1
                    );

                if (_health.CurrentHealth <= 0)
                    StartDieState();
                else
                    StartInjureState();
            }
        }

        public void DestroyReceiver()
        {
            manager.destroyer.DestroyEntity();
        }

        #endregion
        
        #region Unity calls

        private void Start()
        {
            _health = GetComponent<HealthStats>();
            IsReceiveDamage = true;
        }

        #endregion

        #region Fields and properties

        private HealthStats _health;

        public Animator animator;
        public EntityManager manager;
        
        // animation hashed strings
        private static readonly int ToDie = Animator.StringToHash("toDie");
        private static readonly int ToInjure = Animator.StringToHash("toInjure");

        public bool IsReceiveDamage { get; set; }

        #endregion

        #region Support methods
        
        private float CalculateDamage(DamageStats damage, DamageType type)
        {
            var damageAmount = type switch
            {
                DamageType.LightDamage => damage.damageLight * (1 - _health.lightArmor),
                DamageType.HeavyDamage => damage.damageHeavy * (1 - _health.heavyArmor),
                DamageType.PierceDamage => damage.damagePierce * (1 - _health.pierceArmor),
                _ => 1
            };

            return damageAmount < 1 ? 1 : damageAmount;
        }

        // toDie state should have corresponding behaviour
        private void StartDieState()
        {
            animator.SetTrigger(ToDie);
        }

        // toInjure state should have corresponding behaviour
        private void StartInjureState()
        {
            animator.SetTrigger(ToInjure);
        }

        #endregion
    }
}