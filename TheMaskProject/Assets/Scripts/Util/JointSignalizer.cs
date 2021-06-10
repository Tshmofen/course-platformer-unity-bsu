using System;
using UnityEngine;

namespace Util
{
    public class JointSignalizer : MonoBehaviour
    {
        public event Action OnJointBreak;
        public Joint2D Joint { get; private set; }

        public void UpdateJoint() => Joint = GetComponent<Joint2D>();
        
        private void Start() => UpdateJoint();
        
        private void OnJointBreak2D(Joint2D _) => OnJointBreak?.Invoke();
    }
}