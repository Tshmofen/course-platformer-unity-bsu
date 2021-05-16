using UnityEngine;

namespace Entity
{
    [RequireComponent(typeof(EntityManager))]
    public class Destroyer : MonoBehaviour
    {
        private static readonly int HashFade = Shader.PropertyToID("_Fade");
        private static readonly int HashScale = Shader.PropertyToID("_Scale");
        private static readonly int HashEdgeColor = Shader.PropertyToID("_EdgeColor");

        [Header("Visuals")]
        public Material dyingMaterial;
        public float noiseScale = 20;
        public Color edgeColor;
        public float colorIntensity = 1;

        private Material _currentMaterial;
        
        public float CurrentFade
        {
            set => _currentMaterial.SetFloat(HashFade, value);
        }

        public virtual void EnableDeathMaterial()
        {
            _currentMaterial = Instantiate(dyingMaterial);
            _currentMaterial.SetFloat(HashScale, noiseScale);
            var newColor = new Color(edgeColor.r * colorIntensity, 
                edgeColor.g * colorIntensity, 
                edgeColor.b * colorIntensity,
                edgeColor.a);
            _currentMaterial.SetColor(HashEdgeColor, newColor);
            foreach (var sprite in GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.material = _currentMaterial;
            }
        }

        public virtual void DestroyEntity() => Destroy(gameObject);
    }
}