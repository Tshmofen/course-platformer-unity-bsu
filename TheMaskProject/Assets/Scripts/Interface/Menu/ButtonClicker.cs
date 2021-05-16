using UnityEngine;
using UnityEngine.EventSystems;

namespace Interface.Menu
{
    public class ButtonClicker : MonoBehaviour, IPointerEnterHandler
    {
        [Header("Audio")]
        public AudioSource clickSound;

        public void OnPointerEnter(PointerEventData eventData) => clickSound.Play();
    }
}