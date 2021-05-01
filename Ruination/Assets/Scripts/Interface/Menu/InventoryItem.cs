using System.IO;
using DataStore.Collectibles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Menu
{
    public class InventoryItem : MonoBehaviour
    {
        [Header("Menu objects")] 
        public TextMeshProUGUI nameMesh;
        public Image miniSpriteObject;
        [Header("Item")] 
        public int itemID = 1;
        
        public CollectibleItemData ItemData;
        [HideInInspector] public Sprite loadedSpriteFull;
        [HideInInspector] public Sprite loadedSpriteMini;

        private void Start()
        {
            SetItemId(itemID);
        }

        private static Sprite LoadSprite(string path, int pixelsPerUnit = 32)
        {
            var texture = Resources.Load(path) as Texture2D;
            if (texture == null) return null;
            
            return Sprite.Create(
                texture, 
                new Rect(0, 0, texture.width, texture.height), 
                new Vector2(0, 0), 
                pixelsPerUnit, 
                0,
                SpriteMeshType.Tight
            );
        }

        public void SetItemId(int id)
        {
            itemID = id;
            ItemData = CollectiblesContainer.GetItem(id);
            
            loadedSpriteFull = LoadSprite(ItemData.PathSpriteFull);
            loadedSpriteMini = LoadSprite(ItemData.PathSpriteMini);
            
            nameMesh.text = ItemData.Name;
            miniSpriteObject.sprite = loadedSpriteMini;
        }
    }
}