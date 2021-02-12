using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Damage
{
    [RequireComponent(typeof(HealthStats), typeof(BoxCollider2D))]
    public class DamageReceiver : MonoBehaviour
    {
        #region Fields and properties

        private HealthStats health;

        public Animator animator;

        public bool IsReceiveDamage { get; set; }

        #endregion

        #region Unity calls

        private void Start()
        {
            health = GetComponent<HealthStats>();
            IsReceiveDamage = true;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            Debug.Log("are you fucking receiver ???????????????");
        }


        /*private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("asdasd");
            DamageDeliver deliver = collision.GetComponent<DamageDeliver>();
            if (deliver != null)
            {
                ReceiveDamage(deliver.Stats, deliver.Type);
            }
        }*/

        #endregion

        #region Public

        public void ReceiveDamage(DamageStats damage, DamageType type)
        {
            if (IsReceiveDamage)
            {
                float damageAmount = CalculateDamage(damage, type);
                health.CurrentHealth -= damageAmount;

                Vector3 abovePosition = GetComponent<BoxCollider2D>().bounds.size;
                abovePosition.x /= 2;
                abovePosition.y /= 2;
                InterfaceUtil
                    .GetOverlayManager()
                    .ShowPopUp(transform.position + abovePosition, damageAmount.ToString(), 1);

                if (health.CurrentHealth <= 0)
                    StartDieState();
                else
                    StartInjureState();
            }
        }

        public float CalculateDamage(DamageStats damage, DamageType type)
        {
            float damageAmount = 1;
            switch (type)
            {
                case DamageType.LightDamage:
                    damageAmount = damage.damageLight * (1 - health.lightArmor);
                    break;

                case DamageType.HeavyDamage:
                    damageAmount = damage.damageHeavy * (1 - health.heavyArmor);
                    break;

                case DamageType.PierceDamage:
                    damageAmount = damage.damagePierce * (1 - health.pierceArmor);
                    break;
            }
            return (damageAmount < 1) ? 1 : damageAmount;
        }

        public void DestroyReceiver()
        {
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(this.gameObject);
        }

        #endregion

        #region Support methods

        // toDie state should have corresponding behaviour
        private void StartDieState()
        {
            animator.SetTrigger("toDie");
        }

        // toInjure state should have corresponding behaviour
        private void StartInjureState()
        {
            animator.SetTrigger("toInjure");
        }

        #endregion
    }
}