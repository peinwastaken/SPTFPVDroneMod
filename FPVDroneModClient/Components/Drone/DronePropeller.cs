using UnityEngine;

namespace FPVDroneModClient.Components.Drone
{
    public class DronePropeller : MonoBehaviour
    {
        public void Rotate(Vector3 axis, float amount)
        {
            gameObject.transform.Rotate(axis, amount);
        }
    }
}
