using System;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movable : MonoBehaviour
    {
        #region Fields & properties

        private Rigidbody2D _body;

        #endregion

        #region Unity calls

        private void Start()
        {
            _body = GetComponent<Rigidbody2D>();
        }

        #endregion
        
    }
}