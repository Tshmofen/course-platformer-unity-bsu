using System.Collections;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movable : MonoBehaviour
    {
        #region Fields & properties
        
        public float rotationStopSpeed = 15;
        public float moveTime = 0.25f;
        public LayerMask hitLayers;
        public float changeRadius = 1.7f;

        private Rigidbody2D _body;
        private int _movableLayer;
        private int _actionLayer;
        private bool _isChangingLayer;

        public Transform PlayerTransform { get; set; }
        
        public bool GravitationLocked
        {
            set => _body.gravityScale = (value) ? 0 : 1;
        }

        #endregion

        #region Unity calls

        private void Start()
        {
            _body = GetComponent<Rigidbody2D>();
            _movableLayer = LayerMask.NameToLayer("Movable");
            _actionLayer = LayerMask.NameToLayer("MovableAction");
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
                hitLayers
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

        public void ChangeLayer(bool inAction)
        {
            if (inAction)
            {
                gameObject.layer = _actionLayer;
            }
            else if (!_isChangingLayer)
            {
                StartCoroutine(ReturnLayerToUsual());
            }
        }

        #endregion

        #region Support methods

        private IEnumerator ReturnLayerToUsual()
        {
            _isChangingLayer = true;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                var distance = PlayerTransform.position - transform.position;
                if (distance.magnitude > changeRadius)
                {
                    _isChangingLayer = false;
                    gameObject.layer = _movableLayer;
                    yield break;
                }
            }
        }

        #endregion
    }
}