using System.Globalization;
using Entity;
using Interface.Overlay;
using UnityEngine;
using Util;

namespace Damage
{
    [RequireComponent(typeof(HealthStats), typeof(BoxCollider2D))]
    public class DamageReceiver : MonoBehaviour
    {
        #region Fields and properties

        private static readonly string BindersPath = "/Interactive/Interface/Overlay/Enemies";
        
        private HealthStats _health;
        private BoxCollider2D _collider;

        private GameObject _binder;
        private bool _isBinderSet;
        
        [Header("Visuals")]
        public bool showHealthBar = true;
        public GameObject healthBinderPrefab;
        [Header("External")] 
        public bool isReceiveDamage = true;
        public EntityManager manager;
        
        // animation hashed strings
        private static readonly int ToDie = Animator.StringToHash("toDie");
        private static readonly int ToInjure = Animator.StringToHash("toInjure");

        #endregion
        
        #region Unity calls

        private void Start()
        {
            _health = GetComponent<HealthStats>();
            _collider = GetComponent<BoxCollider2D>();
            isReceiveDamage = true;
        }

        // Also destroys created health bar
        private void OnDestroy()
        {
            if (_isBinderSet)
                Destroy(_binder);
        }

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

        private void ShowPopUp(float damage)
        {
            var abovePosition = _collider.bounds.size;
            abovePosition /= 2;
            InterfaceUtil
                .PopUpManager
                .ShowPopUp(
                    transform.position + abovePosition,
                    damage.ToString(CultureInfo.InvariantCulture),
                    1
                );
        }

        private void EnableHealthBar()
        {
            if (showHealthBar && !_isBinderSet)
            { 
                _isBinderSet = true;
                var binders = GameObject.Find(BindersPath);
                _binder = Instantiate(healthBinderPrefab, binders.transform);
                var binderScript = _binder.GetComponent<HealthBinder>();
                
                binderScript.health = _health;
                binderScript.target = transform;
                binderScript.isFollower = true;
                binderScript.offset = _collider.bounds.size / 2;
            }
        }

        // toDie state should have corresponding behaviour
        private void StartDieState()
        {
            manager.animator.SetTrigger(ToDie);
        }

        // toInjure state should have corresponding behaviour
        private void StartInjureState()
        {
            manager.animator.SetTrigger(ToInjure);
        }

        #endregion
        
        #region Public

        public void ReceiveDamage(DamageStats damage, DamageType type)
        {
            if (isReceiveDamage)
            {
                var damageAmount = CalculateDamage(damage, type);
                _health.CurrentHealth -= (int)damageAmount;
                if (_health.CurrentHealth < 0) _health.CurrentHealth = 0;

                ShowPopUp(damageAmount);
                EnableHealthBar();
                
                if (_health.CurrentHealth == 0)
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
    }
}