using System;
using UnityEngine;

namespace Entity.Damage
{
    public class HealthStats : MonoBehaviour
    {
        public int maxHealth;
        [Range(0, 1)] public float lightArmor;
        [Range(0, 1)] public float pierceArmor;
        [Range(0, 1)] public float heavyArmor;

        public int CurrentHealth { get; set; }
        
        private void Start()
        {
            CurrentHealth = maxHealth;
        }
    }
}