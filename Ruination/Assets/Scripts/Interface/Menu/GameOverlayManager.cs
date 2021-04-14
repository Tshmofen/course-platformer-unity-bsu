using UnityEngine;
using Util;

namespace Interface.Menu
{
    public class GameOverlayManager : MonoBehaviour
    {
        #region Fields & properties

        [Header("External Menus")] 
        public PauseMenu pauseMenu;

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
            // TODO inventory handling
        }
        
        #endregion
    }
}