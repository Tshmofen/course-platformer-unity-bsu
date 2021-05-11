using Interface.Menu;
using UnityEngine;

namespace Environment.Interactive
{
    public class Collectible : MonoBehaviour, IInteractive
    {
        public int itemID = 1;
        public InventoryMenu inventory;

        private void Collect()
        {
            var collectible = gameObject;
            inventory.AddItem(itemID);
            collectible.SetActive(false);
            Destroy(collectible);
        }

        public void Interact() => Collect();
    }
}