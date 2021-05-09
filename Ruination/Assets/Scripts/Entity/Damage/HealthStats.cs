using UnityEngine;

namespace Entity.Damage
{
    public class HealthStats : MonoBehaviour
    {
        #region Unity calls

        private void Start()
        {
            CurrentHealth = (int)maxHealth;
        }

        #endregion

        #region Fields and properties

        public float maxHealth;
        [Range(0, 1)] public float lightArmor;
        [Range(0, 1)] public float pierceArmor;
        [Range(0, 1)] public float heavyArmor;

        public int CurrentHealth { get; set; }

        #endregion
    }
}