using UnityEngine;

namespace Background
{
    [ExecuteInEditMode]
    public class ParallaxLayer : MonoBehaviour
    {
        public float parallaxFactorX;
        public float parallaxFactorY;

        public void MoveX(float delta)
        {
            transform.localPosition -= new Vector3(delta * parallaxFactorX, 0, 0);
        }

        public void MoveY(float delta)
        {
            transform.localPosition -= new Vector3(0, delta * parallaxFactorY, 0);
        }
    }
}