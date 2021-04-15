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
        public GameObject itemGroupObject;
        [Header("External")] 
        public GameObject itemButtonPrefab;
        
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

        public void FilterItems(TypeObject typeObject)
        {
            var type = typeObject.type;
            foreach (var item in _items)
                item.gameObject.SetActive(
                    type == ItemType.Default || item.types.Contains(type)
                    );
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