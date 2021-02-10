using Assets.Scripts.Interface;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class InterfaceUtil
    {
        #region Fields and properties

        public const string OverlayPath = "/Interface/Overlay";

        #endregion

        #region Public

        public static OverlayManager GetOverlayManager()
        {
            return GameObject.Find(OverlayPath).GetComponent<OverlayManager>();
        }

        #endregion
    }
}