using UnityEngine;

namespace Interface.World
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class InteractButton : MonoBehaviour
    {
        #region Fields & properties

        public SpriteRenderer Sprite { get; set; }

        #endregion

        #region Unity calls

        private void Start()
        {
            Sprite = GetComponent<SpriteRenderer>();
            Sprite.enabled = false;
        }

        #endregion
    }
}