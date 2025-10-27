#if !UNITY_EDITOR
using EFT.Interactive;
using FPVDroneMod.Components;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace FPVDroneMod.Patches
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
            DroneController controller = __instance.GetComponentInChildren<DroneController>();
            
            if (controller && __instance.RigidBody)
            {
                Plugin.Logger.LogInfo("method_3");
                
                EFTPhysicsClass.GClass723.SupportRigidbody(controller.gameObject.GetComponent<Rigidbody>(), 1f);

                return false;
            }

            return true;
        }
    }
    
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
                Rigidbody rb = __instance.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.detectCollisions = false;

                BoxCollider bc = __instance.GetComponent<BoxCollider>();
                bc.enabled = false;
            }
        }
    }
}
#endif