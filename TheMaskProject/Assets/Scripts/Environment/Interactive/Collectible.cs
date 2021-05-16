using Interface.Menu;
using UnityEngine;

namespace Environment.Interactive
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class Collectible : MonoBehaviour, IInteractive
    {
        [Header("Inventory")]
        public int itemID = 1;
        public InventoryMenu inventory;
        [Header("Visuals")] 
        public GameObject particles;
        [Header("Audio")] 
        public AudioSource pickupSound;

        protected virtual void Collect()
        {
            inventory.AddItem(itemID);
            pickupSound.Play();
            DisableItem();
        }

        protected void DisableItem()
        {
            particles.SetActive(false);
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }

        public void Interact() => Collect();
    }
}