using Assets.Scripts.Damage;
using UnityEngine;

namespace Assets.Scripts.Entity.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("Attached objects")]
        public DamageDeliver weapon;
        public DamageReceiver health;
        public EnemyController enemy;
    }
}