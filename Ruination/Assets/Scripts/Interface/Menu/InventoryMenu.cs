using UnityEngine;
using UnityEngine.EventSystems;
using Util;

namespace Interface.Menu
{
    public class InventoryMenu : AbstractMenu
    {
        #region Fields & properties

        [Header("Inventory menu")]
        public GameObject menuObject;
        public GameObject mainButton;
        public EventSystem eventSystem;

        #endregion

        #region Unity calls

        public void HandleExit()
        {
            IsMenuEnabled = false;
            EnableMenu(this.IsMenuEnabled, this.WasMenuEnabled);
        }

        #endregion

        #region Public

        public override void EnableMenu(bool enable, bool wasEnabled)
        {
            menuObject.SetActive(enable);
            Cursor.lockState = (enable) ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public override bool GetMenuControls() => InputUtil.GetInventoryMenu();

        #endregion
    }
}