using UnityEngine;

namespace Interface.Menu
{
    public abstract class AbstractMenu : MonoBehaviour
    {
        public bool IsMenuEnabled { get; set; }
        public bool WasMenuEnabled { get; set; }

        public abstract void EnableMenu(bool enable, bool wasEnabled);
        public abstract bool GetMenuControls();
    }
}