using Interface.Fading;
using UnityEngine;

namespace Entity.Player
{
    public class PlayerDestroyer : Destroyer
    {
        [Header("Visuals")]
        public DeathScreen deathScreen;

        public override void EnableDeathMaterial()
        {
            base.EnableDeathMaterial();
            deathScreen.StartFade();
        }

        public override void DestroyEntity()
        {
            gameObject.SetActive(false);
        }
    }
}