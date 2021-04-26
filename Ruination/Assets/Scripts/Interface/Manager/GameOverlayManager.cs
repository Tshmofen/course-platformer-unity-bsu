using Interface.Menu;
using UnityEngine;
using Util;

namespace Interface.Manager
{
    public class GameOverlayManager : MonoBehaviour
    {
        #region Fields & properties

        private bool _isMenuEnabled;
        private AbstractMenu _enabledMenu;

        [Header("External Menus")]
        public AbstractMenu[] menus;

        #endregion

        #region Unity calls

        private void Update()
        {
            foreach (var menu in menus)
            {
                if (_isMenuEnabled && menu != _enabledMenu) continue;
                
                if (InputUtil.GetCloseAnyMenu()) 
                    _isMenuEnabled = false;
                    
                menu.WasMenuEnabled = menu.IsMenuEnabled;
                menu.IsMenuEnabled ^= menu.GetMenuControls();
                if (InputUtil.GetCloseAnyMenu() && menu.WasMenuEnabled) menu.IsMenuEnabled = false;
                if (menu.IsMenuEnabled)
                {
                    _enabledMenu = menu;
                    _isMenuEnabled = true;
                }
                if (menu.WasMenuEnabled && !menu.IsMenuEnabled 
                    || !menu.WasMenuEnabled && menu.IsMenuEnabled) 
                    menu.EnableMenu(menu.IsMenuEnabled, menu.WasMenuEnabled);
            }
        }

        #endregion
    }
}