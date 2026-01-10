using UnityEngine;

namespace FPVDroneMod.Components
{
    public class DronePropeller : MonoBehaviour
    {
        public void Rotate(Vector3 axis, float amount)
        {
            gameObject.transform.Rotate(axis, amount);
        }
    }
}
