using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Util;

namespace Interface.Menu
{
    public class InventoryMenu : AbstractMenu
    {
        #region Fields & properties

        [Header("Inventory menu")]
        public GameObject menuObject;
        //public GameObject mainButton;
        public TextMeshProUGUI descriptionMesh;
        public Image fullItemImage;
        public EventSystem eventSystem;

        #endregion

        #region Unity calls

        private void Start()
        {
            EnableMenu(IsMenuEnabled, WasMenuEnabled);
        }

        // called by a button
        public void HandleExit()
        {
            IsMenuEnabled = false;
            EnableMenu(this.IsMenuEnabled, this.WasMenuEnabled);
        }
        
        // called by a button
        public void SetCurrentItem(Item item)
        {
            descriptionMesh.text = item.description;
            fullItemImage.sprite = item.spriteFull;
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