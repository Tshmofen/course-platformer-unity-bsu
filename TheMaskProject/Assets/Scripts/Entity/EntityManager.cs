using Entity.Audio;
using Entity.Damage;
using UnityEngine;

// All methods below marked as unused cause they are called only in animation events
// ReSharper disable UnusedMember.Global

namespace Entity
{
    public class EntityManager : MonoBehaviour
    {
        [Header("Attached objects")] 
        public GameObject weapon;
        public DamageReceiver health;
        public Animator animator;
        public BaseEntityController entity;
        public EntityAudioController entityAudioController;

        #region For animation events
        
        public void PlaySound(string soundName) => entityAudioController.PlaySound(soundName);

        public void PlayFootstep(int footNumber) => entityAudioController.PlayFootstep(footNumber);

        public void StopSound(string soundName) => entityAudioController.StopSound(soundName);

        public void NotReceiveDamageFor(float duration) => health.NotReceiveDamageFor(duration);

        public void Lock() => entity.Lock();
        public void Unlock() => entity.Unlock();

        public void EnableAttackState() => entity.EnableAttackState();
        public void DisableAttackState() => entity.DisableAttackState();

        #endregion
    }
}