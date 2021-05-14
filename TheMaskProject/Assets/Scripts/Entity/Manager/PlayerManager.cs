using Entity.Controller;

namespace Entity.Manager
{
    public class PlayerManager : EntityManager
    {
        public PlayerController player;

        public void PlaySound(string soundName) => entityAudioController.PlaySound(soundName);

        public void PlayFootstep(int footNumber) => entityAudioController.PlayFootstep(footNumber);
    }
}