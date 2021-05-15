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

        private void Collect()
        {
            inventory.AddItem(itemID);
            pickupSound.Play();
            DisableItem();
        }

        private void DisableItem()
        {
            particles.SetActive(false);
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }

        public void Interact() => Collect();
    }
}