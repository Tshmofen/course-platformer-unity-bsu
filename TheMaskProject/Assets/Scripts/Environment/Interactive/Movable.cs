using UnityEngine;
using Util;

namespace Environment.Interactive
{
    public class Movable : MonoBehaviour, IInteractive
    {
        private Rigidbody2D _player;

        [Header("Behaviour")] 
        [SerializeField] private float moveForce = 20;
        [Header("Visuals")] 
        [SerializeField] private string interactText = "Push";
        [Header("External")] 
        [SerializeField] private Rigidbody2D body;
        

        public string InteractText => interactText;

        private void Start()
        {
            _player = GameObject
                .FindWithTag(TagStorage.PlayerTag)
                .GetComponentInChildren<Rigidbody2D>();
        }

        public void Interact()
        {
            var direction = Mathf.Sign(body.position.x - _player.position.x);
            body.AddForce(direction * Vector2.right * moveForce, ForceMode2D.Force);
        }
    }
}