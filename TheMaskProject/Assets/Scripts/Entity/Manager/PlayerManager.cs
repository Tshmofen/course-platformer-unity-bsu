using Entity.Controller;

namespace Entity.Manager
{
    public class PlayerManager : EntityManager
    {
        public PlayerController player;

        public void PlaySound(string soundName) => audioController.PlaySound(soundName);
    }
}