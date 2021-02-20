using UnityEngine;

namespace Entity
{
    [RequireComponent(typeof(EntityManager))]
    public class Destroyer : MonoBehaviour
    {
        #region Public

        public void DestroyEntity()
        {
            Destroy(gameObject);
        }

        #endregion
    }
}