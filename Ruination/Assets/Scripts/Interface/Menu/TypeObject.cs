using UnityEngine;

namespace Interface.Menu
{
    public class TypeObject : MonoBehaviour
    {
        public ItemType type;

        public TypeObject() => type = ItemType.Default;
    }
    
    public enum ItemType
    {
        Default,
        Weapon,
        Consumable
    }
}