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
            return AccessTools.Method(typeof(LootItem), nameof(LootItem.OnRigidbodyStarted));
        }

        [PatchPrefix]
        private static bool PatchPrefix(LootItem __instance)
        {
            BaseDroneController controller = __instance.GetComponent<BaseDroneController>();

            if (controller)
            {
                Rigidbody rb = __instance.RigidBody;
                PhysicsExtensions.UpdateController.SupportRigidbody(rb);
                controller.RigidBody = rb;
                controller.FixColliderLayers();
                
                DebugLogger.LogInfo("found rb and did stuff w/ it");
                return false;
            }

            return true;
        }
    }
}
#endif
