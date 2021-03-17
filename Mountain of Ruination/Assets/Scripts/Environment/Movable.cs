using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movable : MonoBehaviour
    {
        #region Fields & properties

        public float rotationStopSpeed = 1;
        
        private Rigidbody2D _body;

        public bool RotationLocked
        {
            set => _body.freezeRotation = value;
        }
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

        public void Move(Vector2 velocity, Vector2 playerPosition, float radius, Vector2 playerDifference)
        {
            if (playerDifference != Vector2.zero)
            {
                transform.position += (Vector3)playerDifference;
            }

            var position = transform.position;
            var newPosition = (Vector2) position + velocity * Time.deltaTime;
            _body.velocity = ((newPosition - playerPosition).magnitude > radius) ? Vector2.zero : velocity;
            
            var distance = (Vector3)playerPosition - position;
            if (distance.magnitude > radius)
            {
                transform.position += distance * (distance.magnitude - radius);
            }

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