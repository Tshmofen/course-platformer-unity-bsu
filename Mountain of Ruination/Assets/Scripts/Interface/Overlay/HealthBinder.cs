using Damage;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Overlay
{
    [RequireComponent(typeof(Image))]
    public class HealthBinder : MonoBehaviour
    {
        public HealthStats health;
        private Image _image;

        private void Start()
        {
            _image = GetComponent<Image>();
        }

        private void FixedUpdate()
        {
            _image.fillAmount = health.CurrentHealth / health.maxHealth;
        }
    }
}