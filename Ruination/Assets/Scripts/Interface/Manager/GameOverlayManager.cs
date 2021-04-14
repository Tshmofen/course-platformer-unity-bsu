using Interface.Menu;
using UnityEngine;
using Util;

namespace Interface.Manager
{
    public class GameOverlayManager : MonoBehaviour
    {
        #region Fields & properties

        [Header("External Menus")] 
        public PauseMenu pauseMenu;
        public InventoryMenu inventoryMenu;

        #endregion

        #region Unity calls

        private void Update()
        {
            UpdatePause();
            UpdateInventory();
        }
        

        #endregion

        #region Update Parts

        private void UpdatePause()
        {
            pauseMenu.WasMenuEnabled = pauseMenu.IsMenuEnabled;
            pauseMenu.IsMenuEnabled ^= InputUtil.GetPauseMenu();
            if (pauseMenu.WasMenuEnabled && !pauseMenu.IsMenuEnabled 
                || !pauseMenu.WasMenuEnabled && pauseMenu.IsMenuEnabled) 
                pauseMenu.EnableMenu(pauseMenu.IsMenuEnabled, pauseMenu.WasMenuEnabled);
        }

        private void UpdateInventory()
        {
            inventoryMenu.WasMenuEnabled = inventoryMenu.IsMenuEnabled;
            inventoryMenu.IsMenuEnabled ^= InputUtil.GetInventoryMenu();
            if (inventoryMenu.WasMenuEnabled && !inventoryMenu.IsMenuEnabled
                || !inventoryMenu.WasMenuEnabled && inventoryMenu.IsMenuEnabled)
                inventoryMenu.EnableMenu(inventoryMenu.IsMenuEnabled, inventoryMenu.WasMenuEnabled);
        }
        
        #endregion
    }
}