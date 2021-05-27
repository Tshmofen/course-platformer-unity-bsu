using System;
using Cinemachine;
using UnityEngine;

namespace Entity
{
    [RequireComponent(typeof(Camera), typeof(CinemachineBrain))]
    public class CameraUpdater : MonoBehaviour
    {
        private CinemachineBrain _brain;

        public event Action OnCameraUpdateEnd;
        
        private void Start()
        {
            _brain = GetComponent<CinemachineBrain>();
        }
        
        private void Update() => LateUpdate();

        private void LateUpdate()
        {
            _brain.ManualUpdate();
            OnCameraUpdateEnd?.Invoke();
        }
    }
}