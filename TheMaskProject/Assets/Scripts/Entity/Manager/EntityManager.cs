using Entity.Controller;
using Entity.Damage;
using UnityEngine;

namespace Entity.Manager
{
    public abstract class EntityManager : MonoBehaviour
    {
        [Header("Attached objects")] 
        public GameObject weapon;
        public DamageReceiver health;
        public Animator animator;
        public Destroyer destroyer;
        public EntityAudioController entityAudioController;
        
        public void PlaySound(string soundName) => entityAudioController.PlaySound(soundName);

        public void PlayFootstep(int footNumber) => entityAudioController.PlayFootstep(footNumber);

        public void StopSound(string soundName) => entityAudioController.StopSound(soundName);
    }
}