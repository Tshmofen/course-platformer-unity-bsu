using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Entity.Controller
{
    public class AudioController : MonoBehaviour
    {
        private List<AudioSource> _sources;

        private void Start()
        {
            _sources = new List<AudioSource>(GetComponentsInChildren<AudioSource>());
        }

        // should be used whenever it's needed to play sound
        public void PlaySound(string soundName)
        {
            var sound = _sources.AsEnumerable()
                .First(source => source.name == soundName);
            if (sound) sound.Play();
            else Debug.Log($"Can't find sound {soundName}");
        }
    }
}