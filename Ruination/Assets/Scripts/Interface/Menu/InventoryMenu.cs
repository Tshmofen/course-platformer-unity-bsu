using System.Collections.Generic;
using DataStore.Collectibles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Interface.Menu
{
    public class InventoryMenu : AbstractMenu
    {
        #region Fields & properties

        private GameObject _contentObject;
        private List<InventoryItem> _items;
        private Button _lastFilterButton;
        private Button _lastItemButton;

        [Header("Inventory objects")] 
        public GameObject itemButtonPrefab;
        public GameObject menuObject;
        public GameObject useMenuObject;
        public TextMeshProUGUI descriptionMesh;
        public Image fullItemImage;
        [Header("Initial buttons")]
        public Button filterAllButton;
        public Button firstItemButton;
        [Header("Visuals")] 
        public Color filterColor;

        #endregion

        #region Unity calls

        private void Start()
        {
            _contentObject = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
            
            _lastFilterButton = filterAllButton;
            SetFilterButton(filterAllButton);
            _lastItemButton = firstItemButton;
            SetItemButton(firstItemButton);
            
            EnableMenu(IsMenuEnabled, WasMenuEnabled);
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
            IsMenuEnabled = false;
            EnableMenu(IsMenuEnabled, WasMenuEnabled);
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

        #endregion

        #region Public

        public void EnableUseMenu()
        {
            var useMenuTransform = useMenuObject.GetComponent<RectTransform>();
            var useMenuRect = useMenuTransform.rect;
            var newPosition = Input.mousePosition;
            
            newPosition += new Vector3(useMenuRect.width / 2, -useMenuRect.height / 2);
            useMenuTransform.position = newPosition;
            useMenuObject.SetActive(true);
        }
        
        public override void EnableMenu(bool enable, bool wasEnabled)
        {
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

        #endregion
    }
}