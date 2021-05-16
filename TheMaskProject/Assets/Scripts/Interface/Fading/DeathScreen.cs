using System;
using System.Collections;
using Interface.Manager;
using Interface.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Fading
{
    public class DeathScreen : MonoBehaviour
    {
        private float _currentFade;
        
        [Header("Visuals")]
        public TextMeshProUGUI text;
        public Image background;
        public float fadeTime;
        [Header("Behaviour")]
        public InterfaceManager interfaceManager;
        public PauseMenu pauseMenu;
        public float menuOffsetY = -5;
        public Button continueButton;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void StartFade()
        {
            gameObject.SetActive(true);
            
            var backColor = background.color;
            var textColor = text.color;
            backColor.a = textColor.a = 0;
            background.color = backColor;
            text.color = textColor;
            
            StartCoroutine(ApplyFade());
        }

        private IEnumerator ApplyFade()
        {
            while (_currentFade <= fadeTime)
            {
                _currentFade += Time.deltaTime;
                
                var backColor = background.color;
                var textColor = text.color;

                var alpha = _currentFade / fadeTime;
                backColor.a = textColor.a = (alpha <= 1)? alpha : 1;

                background.color = backColor;
                text.color = textColor;
                
                yield return null;
            }

            interfaceManager.isLocked = true;
            pauseMenu.EnableMenu(true);
            continueButton.interactable = false;
            
            var menuTransform = pauseMenu.transform;
            var position = menuTransform.position;
            position.y += menuOffsetY;
            menuTransform.position = position;
        }
    }
}