using UnityEngine;

#if !UNITY_EDITOR
namespace FPVDroneMod.Helpers
{
    public static class VectorHelper
    {
        public static bool VisCheck(Vector3 vector1, Vector3 vector2, LayerMask mask)
        {
            return Physics.Raycast(vector1, vector2 - vector1, out RaycastHit hit, mask);
        }
    }
}
#endif