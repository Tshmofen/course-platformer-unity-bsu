using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Interface.Menu
{
    [ExecuteInEditMode]
    public class Item : MonoBehaviour
    {
        #region Fields & properties

        [Header("Menu objects")] 
        public TextMeshProUGUI nameMesh;
        public Image miniImageObject;
        [Header("Item parts")]
        public string itemName; 
        [TextArea(15,20)]
        public string description;
        public Sprite spriteFull;
        public Sprite spriteMini;

        #endregion

        #region Unity calls

        private void Start()
        {
            nameMesh.text = itemName;
            miniImageObject.sprite = spriteMini;
        }

        #if UNITY_EDITOR
        private void Update()
        {
            nameMesh.text = itemName;
            miniImageObject.sprite = spriteMini;
        }
        #endif

        #endregion
        
    }
}