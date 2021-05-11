using UnityEngine;
using Util;

namespace Environment.Interactive
{
    public class InteractiveEnter : MonoBehaviour, IInteractive
    {
        public GameObject destinationObject;
        public Vector2 offset;
    
        public void Interact()
        {
            var player = GameObject.FindWithTag(TagStorage.PlayerTag);
            player.transform.position = destinationObject.transform.position + (Vector3)offset;
        }
    }
}