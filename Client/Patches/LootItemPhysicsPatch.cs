#if !UNITY_EDITOR
using EFT.Interactive;
using FPVDroneMod.Components;
using FPVDroneMod.Globals;
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
            string templateId = __instance.TemplateId;

            if (templateId == ItemIds.DroneTemplateId)
            {
                Rigidbody rb = __instance.RigidBody;
                EFTPhysicsClass.GClass745.SupportRigidbody(rb);
                return false;
            }

            return true;
        }
    }
}
#endif