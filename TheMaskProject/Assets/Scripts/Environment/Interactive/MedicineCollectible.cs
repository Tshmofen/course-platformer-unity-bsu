using Interface.Overlay;
using UnityEngine;

namespace Environment.Interactive
{
    public class MedicineCollectible : Collectible
    {
        [Header("External")]
        public HealingBox healingBox;
        
        protected override void Collect()
        {
            if (!inventory.HaveItem(itemID)) 
                inventory.AddItem(itemID);

            healingBox.HealsRemain++;
            pickupSound.Play();
            DisableItem();
        }
    }
}