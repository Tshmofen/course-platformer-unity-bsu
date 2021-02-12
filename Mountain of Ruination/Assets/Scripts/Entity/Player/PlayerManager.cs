using Assets.Scripts.Damage;
using Assets.Scripts.Entity.Player;
using UnityEngine;

namespace Assets.Scripts.Entity
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Attached objects")]
        public DamageDeliver weapon;
        public DamageReceiver health;
        public PlayerController player;
        public Animator animator;
    }
}