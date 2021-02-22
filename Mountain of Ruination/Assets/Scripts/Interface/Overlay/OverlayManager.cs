using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Interface.Overlay
{
    [RequireComponent(typeof(Canvas))]
    public class OverlayManager : MonoBehaviour
    {
        #region Fields and properties

        public GameObject damagePopUp;

        #endregion

        #region Unity Calls

        private void Awake()
        {
            InterfaceUtil.OverlayManager = this;
        }

        #endregion

        #region Public

        // ReSharper disable once PossibleNullReferenceException
        // shows popup over the target that flies away and disappears
        public void ShowPopUp(Vector2 worldPosition, string text, float lifeTime)
        {
            var newPopUp = Instantiate(damagePopUp, gameObject.transform);
            newPopUp.GetComponent<Text>().text = text;
            newPopUp.GetComponent<TemporaryText>().lifeTime = lifeTime;
            newPopUp.transform.position = worldPosition;
        }

        #endregion
    }
}