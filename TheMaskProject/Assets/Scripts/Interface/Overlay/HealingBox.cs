using System;
using Entity.Damage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Interface.Overlay
{
    public class HealingBox : MonoBehaviour
    {
        [Header("Stats")]
        public int healAmount = 70;
        [Header("Player")]
        public HealthStats health;
        [Header("Visuals")] 
        public TextMeshProUGUI text;
        public Image flaskImage;
        public Color colorAvailable;
        public Color colorUnavailable;
        
        public int HealsRemain { get; set; }

        private void Update()
        {
            text.text = HealsRemain.ToString();
            if (HealsRemain <= 0)
            {
                flaskImage.color = colorUnavailable;
                
            }
            else
            {
                flaskImage.color = colorAvailable;
                if (InputUtil.GetHeal() && health.CurrentHealth != health.maxHealth)
                {
                    HealsRemain--;
                    health.CurrentHealth += healAmount;
                    health.CurrentHealth = (health.CurrentHealth > health.maxHealth) ? 
                        health.maxHealth : health.CurrentHealth;
                }
            }
        }
    }
}