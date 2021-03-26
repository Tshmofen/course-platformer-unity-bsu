using System;
using UnityEngine;
using Util;

namespace Interface.World
{
    public class InteractPointer : MonoBehaviour
    {
        #region Fields & properties

        private Vector2 _position;
        
        [Header("Position")]
        public float radius = 3;
        [Header("Movement")]
        public float speed = 7;

        #endregion

        #region Unity calls

        private void Start()
        {
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            var move = InputUtil.GetMousePositionDelta();
            if (move != Vector2.zero)
            {
                _position += move * (speed * Time.deltaTime);
                var distance = _position.magnitude;
                if (distance > radius)
                    _position = _position.normalized * radius;

                transform.localPosition = _position;
            }
        }

        #endregion
    }
}