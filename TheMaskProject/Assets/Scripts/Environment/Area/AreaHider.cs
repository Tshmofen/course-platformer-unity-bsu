using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Environment.Area
{
    [RequireComponent(typeof(Tilemap))]
    public class AreaHider : MonoBehaviour
    {
        private Tilemap _tiles;

        [Header("Visuals")]
        public float fadeSpeed = 0.3f;
        [Header("Audio")] 
        public AudioSource foundSound;

        private void Start()
        {
            _tiles = GetComponent<Tilemap>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            GetComponent<Collider2D>().enabled = false;
            foundSound.Play();
            StartCoroutine(FadeColor());
        }

        private IEnumerator FadeColor()
        {
            while (_tiles.color.a > 0)
            {
                var currentColor = _tiles.color;
                currentColor.a -= fadeSpeed * Time.deltaTime;
                if (currentColor.a < 0) currentColor.a = 0;
                _tiles.color = currentColor;
                yield return null;
            }
        }
    }
}