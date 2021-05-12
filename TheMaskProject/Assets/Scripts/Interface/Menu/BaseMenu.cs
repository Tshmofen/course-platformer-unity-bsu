using UnityEngine;
using Util;

namespace Interface.Menu
{
    public abstract class BaseMenu : MonoBehaviour
    {
        public bool IsEnabled { get; protected set; }

        public abstract void EnableMenu(bool enable);

        public abstract bool GetMenuControls();

        public static bool GetCloseAnyMenu() => InputUtil.GetCloseAnyMenu();
    }
}