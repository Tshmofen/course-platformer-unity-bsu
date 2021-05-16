using Entity.Audio;
using Entity.Damage;
using UnityEngine;

namespace Entity.Manager
{
    public abstract class BaseEntityManager : MonoBehaviour
    {
        [Header("Attached objects")] 
        public GameObject weapon;
        public DamageReceiver health;
        public Animator animator;
        public EntityAudioController entityAudioController;

        #region For animation events
        
        public void PlaySound(string soundName) => entityAudioController.PlaySound(soundName);

        public void PlayFootstep(int footNumber) => entityAudioController.PlayFootstep(footNumber);

        public void StopSound(string soundName) => entityAudioController.StopSound(soundName);

        public void NotReceiveDamageFor(float duration) => health.NotReceiveDamageFor(duration);
        
        #endregion
    }
}