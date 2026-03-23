#if !UNITY_EDITOR
using EFT;
using EFT.Interactive;
using FPVDroneModClient.Components;
using FPVDroneModClient.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Components.Drone;

namespace FPVDroneModClient.Patches
{
    public class InteractionPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GetActionsClass), nameof(GetActionsClass.smethod_8));
        }

        [PatchPostfix]
        public static void Postfix(ref ActionsReturnClass __result, GamePlayerOwner owner, LootItem lootItem)
        {
            BaseDroneController controller = lootItem.GetComponentInChildren<BaseDroneController>();

            DebugLogger.LogInfo($"Interacting with item: {lootItem.TemplateId}");

            if (controller)
            {
                DebugLogger.LogInfo("Interacting with drone - create actions");
                
                __result.Actions[0].Action += () => DroneHelper.PickUpDrone(controller);
                __result.CreateAction("Use", () => DroneHelper.UseDrone(controller));
                __result.CreateAction("Flip", () => DroneHelper.FlipDrone(controller));
            }
        }
    }
}
#endif
