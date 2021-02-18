using UnityEngine;

namespace Assets.Scripts.Entity
{
    [RequireComponent(typeof(EntityManager))]
    public class Destroyer : MonoBehaviour
    {
        #region Public

        public void DestroyEntity()
        {
            Destroy(this.gameObject);
        }

        #endregion
    }
}