using UnityEngine;
using Util;

namespace Environment.Interactive
{
    public class Movable : MonoBehaviour, IInteractive
    {
        private bool _isConnected;
        private bool _isJointDestroyed;
        private Rigidbody2D _playerBody;
        
        [Header("Joint")] 
        [SerializeField] private float maxTorque;
        [SerializeField] private float maxForce;
        [SerializeField] private float correction;
        [SerializeField] private float breakTorque;
        [SerializeField] private float breakForce;
        [SerializeField] private Vector2 offset;
        [Header("External")] 
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private JointSignalizer signalizer;

        private void Start()
        {
            _isConnected = false;
            _isJointDestroyed = true;
            
            _playerBody = GameObject
                .FindWithTag(TagStorage.PlayerTag)
                .GetComponentInChildren<Rigidbody2D>();
            
            signalizer.OnJointBreak += () =>
            {
                _isConnected = false;
                _isJointDestroyed = true;
            };
        }

        public void Interact()
        {
            _isConnected = !_isConnected;
            Debug.Log(_isConnected);
            if (_isConnected)
            {
                if (_isJointDestroyed) AddJointToBody();
                var xSign = Mathf.Sign(transform.position.x - _playerBody.transform.position.x);
                var signedOffset = new Vector2(offset.x * xSign, offset.y);
                ((RelativeJoint2D)signalizer.Joint).linearOffset = signedOffset;
            }
            signalizer.Joint.enabled = _isConnected;
        }

        private void AddJointToBody()
        {
            var joint = body.gameObject.AddComponent<RelativeJoint2D>();
            signalizer.UpdateJoint();

            joint.autoConfigureOffset = false;
            joint.enableCollision = true;
            joint.connectedBody = _playerBody;
            joint.maxForce = maxForce;
            joint.maxTorque = maxTorque;
            joint.correctionScale = correction;
            joint.breakForce = breakForce;
            joint.breakTorque = breakTorque;
            joint.linearOffset = offset;

            _isJointDestroyed = false;
        }
    }
}