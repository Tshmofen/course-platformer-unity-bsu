using Interface.Menu;

namespace Environment.Interactive
{
    public class Collectible : AbstractInteractive
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

        public override void Interact() => Collect();
    }
}