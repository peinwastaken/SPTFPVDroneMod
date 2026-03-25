#if !UNITY_EDITOR
using EFT;
using EFT.Communications;
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
                
                // if the drone is being piloted dont allow picking it up
                if (controller.IsBeingControlled)
                {
                    ActionsTypesClass actionsTypes = __result.Actions[0];
                    
                    if (actionsTypes != null)
                    {
                        actionsTypes.Action = () =>
                        {
                            NotificationManagerClass.DisplayMessageNotification(
                                "IS BEING PILOTED".Localized(),
                                ENotificationDurationType.Default,
                                ENotificationIconType.Alert
                            );
                        };
                    }
                }
                
                __result.CreateAction("Use", () => DroneHelper.UseDrone(controller));
                __result.CreateAction("Flip", () => DroneHelper.FlipDrone(controller));
            }
        }
    }
}
#endif
