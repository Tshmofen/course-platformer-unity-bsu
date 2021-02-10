using UnityEngine;

namespace Assets.Scripts.Background
{
    [ExecuteInEditMode]
    public class ParallaxLayer : MonoBehaviour
    {
        #region Fields and properties

        public float parallaxFactorX;
        public float parallaxFactorY;

        #endregion

        #region Public

        public void MoveX(float delta)
        {
            Vector3 newPos = transform.localPosition;
            newPos.x -= delta * parallaxFactorX;
            transform.localPosition = newPos;
        }

        public void MoveY(float delta)
        {
            Vector3 newPos = transform.localPosition;
            newPos.y -= delta * parallaxFactorY;
            transform.localPosition = newPos;
        }

        #endregion
    }
}