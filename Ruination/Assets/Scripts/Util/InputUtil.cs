using UnityEngine;

// ReSharper disable UnusedMember.Global
namespace Util
{
    public static class InputUtil
    {
        #region Character actions

        public static Vector2 GetMove()
        {
            var move = Vector2.zero;
            move.x = Input.GetAxis("Horizontal");
            move.y = Input.GetAxis("Vertical");
            return move;
        }

        public static bool GetJump() => Input.GetKeyDown(KeyCode.Space);

        public static bool GetContinuousJump() => Input.GetKey(KeyCode.Space);

        public static bool GetEvade() => Input.GetKeyDown(KeyCode.LeftShift);

        public static bool GetCombatMode() => Input.GetKeyDown(KeyCode.Tab);

        public static bool GetAttack() => Input.GetKeyDown(KeyCode.Mouse0);

        public static bool GetInteract() => Input.GetKeyDown(KeyCode.E);
        
        public static bool GetIgnorePlatform() => Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.S);

        public static bool GetParry() => Input.GetKeyDown(KeyCode.Mouse1);

        #endregion

        #region Mouse

        public static Vector2 GetMousePositionDelta()
        {
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        public static Vector2 GetMouseDeltaToCenter()
        {
            var center = new Vector2(Screen.width, Screen.height) / 2;
            return (Vector2) Input.mousePosition - center;
        }

        public static Vector2 GetMouseDeltaToObject(GameObject @object)
        {
            // ReSharper disable once PossibleNullReferenceException
            Vector2 objectPosition = Camera.main.WorldToScreenPoint(@object.transform.position);
            return (Vector2) Input.mousePosition - objectPosition;
        }
        
        #endregion

        #region Interface
        
        public static bool GetCloseAnyMenu() => Input.GetKeyDown(KeyCode.Escape);

        public static bool GetPauseMenu() => Input.GetKeyDown(KeyCode.Escape);

        public static bool GetInventoryMenu() => Input.GetKeyDown(KeyCode.I);

        #endregion
    }
}