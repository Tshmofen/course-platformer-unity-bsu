using Cinemachine;
using Entity.Controller;
using UnityEngine;

namespace Environment.Area
{
    public class DeathArea : MonoBehaviour
    {
        public CinemachineVirtualCamera playerCamera;
            
        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                var cameraStop = new GameObject();
                cameraStop.transform.position = player.transform.position;
                playerCamera.Follow = cameraStop.transform;
                
                player.manager.health.KillReceiver();
            }
        }
    }
}