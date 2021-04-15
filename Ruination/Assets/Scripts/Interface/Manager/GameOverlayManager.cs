using Interface.Menu;
using UnityEngine;

namespace Interface.Manager
{
    public class GameOverlayManager : MonoBehaviour
    {
        #region Fields & properties

        [Header("External Menus")]
        public AbstractMenu[] menus;

        #endregion

        #region Unity calls

        private void Update()
        {
            foreach (var menu in menus)
            {
                menu.WasMenuEnabled = menu.IsMenuEnabled;
                menu.IsMenuEnabled ^= menu.GetMenuControls();
                if (menu.WasMenuEnabled && !menu.IsMenuEnabled 
                    || !menu.WasMenuEnabled && menu.IsMenuEnabled) 
                    menu.EnableMenu(menu.IsMenuEnabled, menu.WasMenuEnabled);
            }
        }

        #endregion
    }
}