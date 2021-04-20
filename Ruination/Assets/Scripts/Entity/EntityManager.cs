using Damage;
using UnityEngine;

namespace Entity
{
    public class EntityManager : MonoBehaviour
    {
        [Header("Attached objects")] 
        public GameObject weapon;
        public DamageReceiver health;
        public Animator animator;
        public Destroyer destroyer;
    }
}