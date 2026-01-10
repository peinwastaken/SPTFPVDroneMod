using UnityEngine;

#if !UNITY_EDITOR
namespace FPVDroneMod.Helpers
{
    public static class VectorHelper
    {
        public static bool VisCheck(Vector3 vector1, Vector3 vector2, LayerMask mask, out RaycastHit hitResult)
        {
            bool hitSomething = Physics.Raycast(vector1, vector2 - vector1, out RaycastHit hit, (vector1 - vector2).magnitude, mask);
            hitResult = hit;
            return !hitSomething;
        }

        public static bool VisCheck(Vector3 vector1, Vector3 vector2, LayerMask mask)
        {
            return VisCheck(vector1, vector2, mask, out RaycastHit _);
        }

        public static bool HitCheck(Vector3 vector1, Vector3 vector2, LayerMask mask, out RaycastHit hitResult)
        {
            bool hitSomething = Physics.Raycast(vector1, vector2 - vector1, out RaycastHit hit, (vector1 - vector2).magnitude, mask);
            hitResult = hit;
            return hitSomething;
        }

        public static bool HitCheck(Vector3 vector1, Vector3 vector2, LayerMask mask)
        {
            return HitCheck(vector1, vector2, mask, out RaycastHit _);
        }
    }
}
#endif
