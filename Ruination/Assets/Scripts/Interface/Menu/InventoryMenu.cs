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

        private List<InventoryItem> _items;
        private Button _lastFilterButton;
        private Button _lastItemButton;

        [Header("Inventory objects")]
        public GameObject menuObject;
        public TextMeshProUGUI descriptionMesh;
        public Image fullItemImage;
        [Header("Initial buttons")]
        public Button filterAllButton;
        public Button firstItemButton;
        [Header("Visuals")] 
        public Color filterColor;
        public Color itemColor;

        #endregion

        #region Unity calls

        private void Start()
        {
            _lastFilterButton = filterAllButton;
            SetFilterButton(filterAllButton);
            _lastItemButton = firstItemButton;
            SetItemButton(firstItemButton);
            
            EnableMenu(IsMenuEnabled, WasMenuEnabled);
            _items = new List<InventoryItem>(GetComponentsInChildren<InventoryItem>());
            if (_items.Count != 0)
                SetItem(_items[0]);
        }

        // called by a button
        public void HandleExit()
        {
            IsMenuEnabled = false;
            EnableMenu(this.IsMenuEnabled, this.WasMenuEnabled);
        }
        
        // called by a button
        public void SetItem(InventoryItem inventoryItem)
        {
            descriptionMesh.text = inventoryItem.description;
            fullItemImage.sprite = inventoryItem.spriteFull;
        }
        
        // called by a button
        public void SetItemButton(Button button)
        {
            var oldColors = _lastItemButton.colors;
            oldColors.normalColor = Color.white;
            _lastItemButton.colors = oldColors;

            _lastItemButton = button;
            var colors = button.colors;
            colors.normalColor = filterColor;
            button.colors = colors;
        }

        // called by a button
        public void SetFilter(TypeObject typeObject)
        {
            var type = typeObject.type;
            foreach (var item in _items)
                item.gameObject.SetActive(
                    type == ItemType.Default || item.types.Contains(type)
                    );
        }

        // called by a button
        public void SetFilterButton(Button button)
        {
            var oldColors = _lastFilterButton.colors;
            oldColors.normalColor = Color.white;
            _lastFilterButton.colors = oldColors;

            _lastFilterButton = button;
            var colors = button.colors;
            colors.normalColor = filterColor;
            button.colors = colors;
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