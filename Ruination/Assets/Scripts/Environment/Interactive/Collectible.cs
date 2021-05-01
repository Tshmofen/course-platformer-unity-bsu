using Interface.Menu;

namespace Environment.Interactive
{
    public class Collectible : AbstractInteractive
    {
        public int itemID = 1;
        public InventoryMenu inventory;
        
        public override void Interact()
        {
            var collectible = gameObject;
            inventory.AddItem(itemID);
            collectible.SetActive(false);
            Destroy(collectible);
        }
    }
}