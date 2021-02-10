using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Interface
{
    [RequireComponent(typeof(Canvas))]
    public class OverlayManager : MonoBehaviour
    {
        #region Fields and properties

        public GameObject damagePopUp;

        #endregion

        #region Public

        public void ShowPopUp(Vector2 worldPosition, string text, float lifeTime)
        {
            GameObject newPopUp = Instantiate(damagePopUp, gameObject.transform);
            newPopUp.GetComponent<Text>().text = text;
            newPopUp.GetComponent<TemporaryText>().lifeTime = lifeTime;
            newPopUp.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(worldPosition);
        }

        #endregion
    }
}