using System.Collections.Generic;
using System.Linq;
using DataStore.Collectibles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Interface.Menu
{
    public class InventoryMenu : BaseMenu
    {
        #region Fields & properties

        private GameObject _contentObject;
        private List<InventoryItem> _items;
        private Button _lastFilterButton;
        private Button _lastItemButton;

        [Header("Inventory objects")] 
        public GameObject itemButtonPrefab;
        public GameObject menuObject;
        public TextMeshProUGUI descriptionMesh;
        public Image fullItemImage;
        [Header("Initial buttons")]
        public Button filterAllButton;
        public Button firstItemButton;
        [Header("Visuals")] 
        public Color filterColor;

        #endregion
        
        private void Start()
        {
            EnableMenu(false);
            
            _lastFilterButton = filterAllButton;
            SetFilterButton(filterAllButton);
            _lastItemButton = firstItemButton;
            SetItemButton(firstItemButton);

            _contentObject = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
            _items = new List<InventoryItem>(GetComponentsInChildren<InventoryItem>());
            if (_items.Count != 0)
            {
                _items[0].SetItemId(_items[0].itemID);
                SetItem(_items[0]);
            }
        }

        // called by a button
        public void HandleExit()
        {
            IsEnabled = false;
            EnableMenu(IsEnabled);
        }
        
        // called by a button
        public void SetItem(InventoryItem inventoryItem)
        {
            descriptionMesh.text = inventoryItem.ItemData.Description;
            fullItemImage.sprite = inventoryItem.loadedSpriteFull;
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
        public void SetFilter(string inventoryItemType)
        {
            foreach (var item in _items)
                item.gameObject.SetActive(
                        inventoryItemType == CollectibleType.Default || 
                        item.ItemData.Types.Contains(inventoryItemType)
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

        public override void EnableMenu(bool enable)
        {
            IsEnabled = enable;
            menuObject.SetActive(enable);
            Cursor.lockState = (enable) ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public override bool GetMenuControls() => InputUtil.GetInventoryMenu();

        public void AddItem(int id)
        {
            var newItem = Instantiate(itemButtonPrefab, _contentObject.transform);
            var itemButton = newItem.GetComponent<Button>();
            var inventoryItem = newItem.GetComponent<InventoryItem>();
            
            _items.Add(inventoryItem);
            inventoryItem.SetItemId(id);
            itemButton.onClick.AddListener(() => SetItemButton(newItem.GetComponent<Button>()));
            itemButton.onClick.AddListener(() => SetItem(inventoryItem));
        }

        public bool HaveItem(int id) => _items.Any(item => item.itemID == id);
    }
}