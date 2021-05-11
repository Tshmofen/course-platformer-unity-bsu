﻿using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Environment.Area
{
    [RequireComponent(typeof(Tilemap))]
    public class AreaHider : MonoBehaviour
    {
        private Tilemap _tiles;
        private bool _isFading;

        public float fadeSpeed = 0.3f;

        private void Start()
        {
            _tiles = GetComponent<Tilemap>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isFading) StartCoroutine(FadeColor());
            GetComponent<Collider2D>().enabled = false;
        }

        private IEnumerator FadeColor()
        {
            _isFading = true;
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