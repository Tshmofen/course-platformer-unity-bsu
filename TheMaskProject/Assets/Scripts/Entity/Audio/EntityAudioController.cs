using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Entity.Audio
{
    public class EntityAudioController : MonoBehaviour
    {
        private List<AudioSource> _sources;
        private List<Footstep> _steps;

        [Header("General")] 
        public bool applyCustomRolloff;
        public float minDistance;
        public float maxDistance;
        [Header("Footsteps")]
        public Tilemap groundMap;
        [Header("Ray Casting")]
        public LayerMask groundMask;
        public float rayThreshold = 1;
        public float offset = 0.3f;

        private void Start()
        {
            _sources = new List<AudioSource>(GetComponentsInChildren<AudioSource>());
            _steps = new List<Footstep>(GetComponentsInChildren<Footstep>());

            if (applyCustomRolloff)
            {
                foreach (var source in _sources)
                {
                    source.spatialBlend = 1;
                    source.rolloffMode = AudioRolloffMode.Linear;
                    source.minDistance = minDistance;
                    source.maxDistance = maxDistance;
                }
            }
        }
        
        // should be used whenever it's needed to play sound
        public void PlaySound(string soundName)
        {
            var sound = FindSound(soundName);
            if (sound) sound.Play();
            else Debug.Log($"Can't find sound {soundName}");
        }
        
        public void StopSound(string soundName)
        {
            var sound = FindSound(soundName);
            if (sound) sound.Stop(); 
            else Debug.Log($"Can't find sound {soundName}");
        }

        public void PlayFootstep(int stepNumber)
        {
            var type = GetStepType();
            _steps
                .First(step => step.stepType == type && step.stepNumber == stepNumber)
                .Play();
        }

        private StepType GetStepType()
        {
            var groundName = "";
            var hit = Physics2D.Raycast(
                transform.position, 
                Vector2.down, 
                rayThreshold,
                groundMask
                );

            if (hit.collider)
            {
                if (hit.collider.name == groundMap.name)
                {
                    var tileOffset = new Vector2(0, offset);
                    var position = groundMap.WorldToCell(hit.point - tileOffset);
                    var tile = groundMap.GetTile(position);
                    if (tile) groundName = tile.name;
                }
                else
                {
                    var render = hit.collider.GetComponent<SpriteRenderer>();
                    if (render)
                        groundName = render.sprite.texture.name;
                }
            }
            
            return Footstep.ConvertNameToType(groundName);
        }
        
        private AudioSource FindSound(string sound) => _sources.AsEnumerable().First(source => source.name == sound);
    }
}