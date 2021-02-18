using Assets.Scripts.Damage;
using UnityEngine;

namespace Assets.Scripts.Entity
{
    public class EntityManager : MonoBehaviour
    {
        [Header("Attached objects")]
        public DamageDeliver weapon;
        public DamageReceiver health;
        public Animator animator;
        public Destroyer destroyer;
    }
}