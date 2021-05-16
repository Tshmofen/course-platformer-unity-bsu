using System.Collections;
using UnityEngine;

namespace Environment.Interactive
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movable : MonoBehaviour
    {
        #region Fields & properties
        
        private Rigidbody2D _body;
        private int _movableLayer;
        private int _actionLayer;
        private bool _isChangingLayer;
        private int _currentSoundsAmount;
        
        [Header("Movement")]
        public float rotationStopSpeed = 15;
        public float moveTime = 0.25f;
        public LayerMask hitLayers;
        public float changeRadius = 1.7f;
        [Header("Audio")] 
        public AudioSource collisionAudio;
        public int maxSoundsTogether = 2;

        public Transform PlayerTransform { get; set; }
        
        public bool GravitationLocked
        {
            set => _body.gravityScale = (value) ? 0 : 1;
        }

        #endregion
        
        private void Start()
        {
            _body = GetComponent<Rigidbody2D>();
            _movableLayer = LayerMask.NameToLayer("Movable");
            _actionLayer = LayerMask.NameToLayer("MovableAction");
        }
        
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

        private IEnumerator ReturnLayerToUsual()
        {
            _isChangingLayer = true;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                var distance = PlayerTransform.position - transform.position;
                if (distance.magnitude > changeRadius && _body.velocity.magnitude == 0)
                {
                    _isChangingLayer = false;
                    gameObject.layer = _movableLayer;
                    yield break;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D _)
        {
            if (_currentSoundsAmount < maxSoundsTogether)
            {
                collisionAudio.PlayOneShot(collisionAudio.clip);
                StartCoroutine(AddOneShot());
            }
        }

        private IEnumerator AddOneShot()
        {
            _currentSoundsAmount++;
            yield return new WaitForSeconds(collisionAudio.clip.length);
            _currentSoundsAmount--;
        }
    }
}