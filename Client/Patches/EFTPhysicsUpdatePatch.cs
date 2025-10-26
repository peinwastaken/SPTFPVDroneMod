#if !UNITY_EDITOR
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace FPVDroneMod.Patches
{
    public class EFTPhysicsUpdatePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(EFTPhysicsClass.GClass722), nameof(EFTPhysicsClass.GClass722.smethod_1));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ref float deltaTime)
        {
            deltaTime = Time.fixedDeltaTime;

            return true;
        }
    }
}
#endif