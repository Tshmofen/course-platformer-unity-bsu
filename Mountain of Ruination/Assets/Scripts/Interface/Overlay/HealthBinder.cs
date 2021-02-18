using Assets.Scripts.Damage;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Interface.Overlay
{
    [RequireComponent(typeof(Image))]
    public class HealthBinder : MonoBehaviour
    {
        private Image image;

        public HealthStats health;

        private void Start()
        {
            image = GetComponent<Image>();   
        }

        void FixedUpdate()
        {
             image.fillAmount = health.CurrentHealth / health.maxHealth;
        }
    }
}