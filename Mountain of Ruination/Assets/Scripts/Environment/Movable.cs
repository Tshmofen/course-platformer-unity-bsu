using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movable : MonoBehaviour
    {
        #region Fields & properties

        public float rotationStopSpeed = 15;
        public float moveTime = 0.25f;
        public LayerMask _hitLayers;
        
        private Rigidbody2D _body;

        public bool GravitationLocked
        {
            set => _body.gravityScale = (value) ? 0 : 1;
        }

        #endregion

        #region Unity calls

        private void Start()
        {
            _body = GetComponent<Rigidbody2D>();
        }

        #endregion

        #region Public

        public void Move(Vector2 toPosition)
        {
            var position = (Vector2)transform.position;
            var direction = (toPosition - position);
            var hit = Physics2D.Raycast(
                position,
                direction, 
                direction.magnitude,
                _hitLayers
                );
            if (hit.collider != null) direction = hit.point - position;
            _body.velocity = direction / moveTime;

            if (_body.angularVelocity > 0)
            {
                var rotation = _body.angularVelocity;
                rotation -= rotationStopSpeed * Time.deltaTime;
                rotation = (rotation <= 0) ? 0 : rotation;
                _body.angularVelocity = rotation;
            }
            if (_body.angularVelocity < 0)
            {
                var rotation = _body.angularVelocity;
                rotation += rotationStopSpeed * Time.deltaTime;
                rotation = (rotation >= 0) ? 0 : rotation;
                _body.angularVelocity = rotation;
            }
        }

        #endregion
    }
}