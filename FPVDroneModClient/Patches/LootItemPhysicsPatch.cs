#if !UNITY_EDITOR
using EFT.Ballistics;
using EFT.Interactive;
using FPVDroneModClient.Components;
using FPVDroneModClient.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using FPVDroneModClient.Components.Base;
using UnityEngine;

namespace FPVDroneModClient.Patches
{
    public class LootItemPhysicsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(LootItem), nameof(LootItem.method_3));
        }

        [PatchPrefix]
        private static bool PatchPrefix(LootItem __instance)
        {
            BaseDroneController controller = __instance.GetComponent<BaseDroneController>();

            if (controller)
            {
                GameObject droneBody = controller.transform.Find("DroneBody")?.gameObject;

                if (!droneBody)
                {
                    DebugLogger.LogWarning("DroneBody is null u idiot.");
                    return true;
                }

                droneBody.layer = LayerMask.NameToLayer("Deadbody");
                Rigidbody rb = __instance.RigidBody;
                EFTPhysicsClass.GClass745.SupportRigidbody(rb);
                controller.RigidBody = rb;

                DebugLogger.LogInfo("found rb and did stuff w/ it");
                return false;
            }

            return true;
        }
    }
}
#endif
