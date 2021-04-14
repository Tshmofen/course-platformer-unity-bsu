using UnityEngine;
using UnityEngine.EventSystems;

namespace Interface.Menu
{
    public class InventoryMenu : MonoBehaviour
    {
        #region Fields & properties

        [Header("Inventory menu")]
        public GameObject menuObject;
        public GameObject mainButton;
        public EventSystem eventSystem;
        
        public bool IsMenuEnabled { get; set; }
        public bool WasMenuEnabled { get; set; }

        #endregion

        #region Unity calls

        public void HandleExit()
        {
            IsMenuEnabled = false;
            EnableMenu(IsMenuEnabled, WasMenuEnabled);
        }

        #endregion

        #region Public

        public void EnableMenu(bool enable, bool wasEnabled)
        {
            menuObject.SetActive(enable);
            Cursor.lockState = (enable) ? CursorLockMode.None : CursorLockMode.Locked;
            /*if (wasEnabled && !enable) _lastButton = eventSystem.currentSelectedGameObject;
            if (!wasEnabled && enable) eventSystem.SetSelectedGameObject(_lastButton);*/
        }

        #endregion
    }
}