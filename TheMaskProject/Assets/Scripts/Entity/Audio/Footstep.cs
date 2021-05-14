using System;
using UnityEngine;

namespace Entity.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class Footstep : MonoBehaviour
    {
        private AudioSource _audio;
        
        [Header("Naming")]
        public StepType stepType;
        public int stepNumber;

        private void Start()
        {
            _audio = GetComponent<AudioSource>();
        }

        public void Play() => _audio.Play();

        public static StepType ConvertNameToType(string name)
        {
            return name.ToLower() switch
            {
                { } type when type.Contains("grass") => StepType.Grass,
                { } type when type.Contains("wood") => StepType.Wood,
                { } type when type.Contains("fabric") => StepType.Fabric,
                { } type when type.Contains("mountain") => StepType.Stone,
                { } type when type.Contains("rock") => StepType.Stone,
                { } type when type.Contains("brick") => StepType.Stone,
                
                _ => StepType.Stone
            };
        }
    }

    public enum StepType
    {
        Grass,
        Wood,
        Stone,
        Fabric
    }
}