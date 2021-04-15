using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Interface.Menu
{
    public class InventoryMenu : AbstractMenu
    {
        #region Fields & properties

        private List<Item> _items;

        [Header("Inventory menu")]
        public GameObject menuObject;
        //public GameObject mainButton;
        public TextMeshProUGUI descriptionMesh;
        public Image fullItemImage;

        #endregion

        #region Unity calls

        private void Start()
        {
            EnableMenu(IsMenuEnabled, WasMenuEnabled);
            _items = new List<Item>(GetComponentsInChildren<Item>());
            if (_items.Count != 0)
                SetCurrentItem(_items[0]);
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