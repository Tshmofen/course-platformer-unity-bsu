using UnityEngine;

namespace Environment.Interactive
{
    public abstract class AbstractInteractive : MonoBehaviour
    {
        public abstract void Collect();

        public abstract void Interact();
    }
}