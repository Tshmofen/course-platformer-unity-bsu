using UnityEngine;

namespace Interface.World
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class InteractButton : MonoBehaviour
    {
        public SpriteRenderer Sprite { get; private set; }
        
        private void Start()
        {
            Sprite = GetComponent<SpriteRenderer>();
            Sprite.enabled = false;
        }
    }
}