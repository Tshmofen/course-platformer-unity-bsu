using System.Collections.Generic;
using System.Linq;
using Entity.Audio;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Entity.Controller
{
    public class EntityAudioController : MonoBehaviour
    {
        private List<AudioSource> _sources;
        private List<Footstep> _steps;

        [Header("Footsteps")]
        public Tilemap groundMap;
        [Header("Raycasting")]
        public LayerMask groundMask;
        public float rayThreshold = 1;
        public float offset = 0.3f;

        private void Start()
        {
            _sources = new List<AudioSource>(GetComponentsInChildren<AudioSource>());
            _steps = new List<Footstep>(GetComponentsInChildren<Footstep>());
        }
        
        // should be used whenever it's needed to play sound
        public void PlaySound(string soundName)
        {
            var sound = _sources.AsEnumerable()
                .First(source => source.name == soundName);
            if (sound) sound.Play();
            else Debug.Log($"Can't find sound {soundName}");
        }

        public void PlayFootstep(int stepNumber)
        {
            var type = GetStepType();
            Debug.Log($"{type}, {stepNumber}");
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
                    groundName = groundMap.GetTile(position).name;
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
        
    }
}