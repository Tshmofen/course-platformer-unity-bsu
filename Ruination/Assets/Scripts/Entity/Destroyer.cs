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

        #endregion

        private Material _currentMaterial;
        
        private static readonly int Fade = Shader.PropertyToID("_Fade");
        private static readonly int Scale = Shader.PropertyToID("_Scale");

        public float CurrentFade
        {
            set => _currentMaterial.SetFloat(Fade, value);
        }

        #endregion
        
        #region Public

        public void EnableDeathMaterial()
        {
            _currentMaterial = Instantiate(dyingMaterial);
            _currentMaterial.SetFloat(Scale, noiseScale);
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