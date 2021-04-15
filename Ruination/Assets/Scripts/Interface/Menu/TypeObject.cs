using UnityEngine;

namespace Interface.Menu
{
    public class TypeObject : MonoBehaviour
    {
        public ItemType type;
    }
    
    public enum ItemType
    {
        Default,
        Weapon,
        Consumable
    }
}