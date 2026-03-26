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
using FPVDroneModClient.Items;

namespace FPVDroneModClient.Patches
{
    public class InteractionPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GetActionsClass), nameof(GetActionsClass.GetAvailableActions), [
                typeof(GamePlayerOwner),
                typeof(GInterface177)
            ]);
        }

        [PatchPrefix]
        public static bool PatchPrefix(ref ActionsReturnClass __result, GamePlayerOwner owner, GInterface177 interactive)
        {
            if (interactive == null) return true;
            
            LootItem lootItem = interactive as LootItem;
            if (lootItem != null && lootItem.Item is DroneItem)
            {
                ActionsReturnClass returnClass = GetActionsClass.smethod_8(owner, lootItem);
                if (returnClass != null)
                {
                    ActionsTypesClass takeAction = null;
                    foreach (ActionsTypesClass action in returnClass.Actions)
                    {
                        if (action.Name == "Take")
                        {
                            takeAction = action;
                        }
                    }

                    if (takeAction == null) return true;
                    
                    BaseDroneController droneController = lootItem.GetComponentInChildren<BaseDroneController>();
                    if (droneController == null) return true;

                    if (droneController.IsBeingControlled)
                    {
                        takeAction.Action = () =>
                        {
                            NotificationManagerClass.DisplayMessageNotification(
                                "IS BEING PILOTED".Localized(),
                                ENotificationDurationType.Default,
                                ENotificationIconType.Alert
                            );
                        };
                    }
                    
                    returnClass.CreateAction("Use", () => DroneHelper.UseDrone(droneController));
                    returnClass.CreateAction("Flip", () => DroneHelper.FlipDrone(droneController));

                    __result = returnClass;
                    return false;
                }
            }

            return true;
        }
    }
}
#endif
