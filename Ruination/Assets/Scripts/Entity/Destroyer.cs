using Entity.Manager;
using UnityEngine;

namespace Entity
{
    [RequireComponent(typeof(EntityManager))]
    public class Destroyer : MonoBehaviour
    {
        #region Fields and properties
        
        #region Unity assigns

        public Material dyingMaterial;
        public float noiseScale = 20;
        public bool disableOnDeath;
        public Color edgeColor;
        public float colorIntensity = 1;

        #endregion

        private Material _currentMaterial;
        
        private static readonly int HashFade = Shader.PropertyToID("_Fade");
        private static readonly int HashScale = Shader.PropertyToID("_Scale");
        private static readonly int HashEdgeColor = Shader.PropertyToID("_EdgeColor");

        public float CurrentFade
        {
            set => _currentMaterial.SetFloat(HashFade, value);
        }

        #endregion
        
        #region Public

        public void EnableDeathMaterial()
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
        
        public void DestroyEntity()
        {
            if (disableOnDeath)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }

        #endregion
    }
}