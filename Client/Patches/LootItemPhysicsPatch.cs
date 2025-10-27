#if !UNITY_EDITOR
using EFT.Interactive;
using FPVDroneMod.Components;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace FPVDroneMod.Patches
{
    public class OnRigidBodyStartedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(LootItem), nameof(LootItem.OnRigidbodyStarted));
        }

        [PatchPostfix]
        private static void PatchPostfix(LootItem __instance)
        {
            DroneController controller = __instance.GetComponentInChildren<DroneController>();
            
            if (controller && __instance.RigidBody)
            {
                Plugin.Logger.LogInfo("support");
                Rigidbody rb = __instance.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.detectCollisions = false;

                BoxCollider bc = __instance.GetComponent<BoxCollider>();
                bc.enabled = false;
                
                EFTPhysicsClass.GClass723.SupportRigidbody(controller.RigidBody, 1f);
            }
        }
    }
    
    public class OnRigidBodyStoppedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(LootItem), nameof(LootItem.OnRigidbodyStopped));
        }

        [PatchPostfix]
        private static void PatchPostfix(LootItem __instance)
        {
            DroneController controller = __instance.GetComponentInChildren<DroneController>();
            
            if (controller && __instance.RigidBody)
            {
                Plugin.Logger.LogInfo("unsupport");
                EFTPhysicsClass.GClass723.UnsupportRigidbody(controller.RigidBody);
            }
        }
    }
}
#endif