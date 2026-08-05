#if !UNITY_EDITOR
using EFT;
using EFT.Communications;
using EFT.Interactive;
using EFT.UI;
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
            return AccessTools.Method(typeof(InteractionContextHelper), nameof(InteractionContextHelper.GetAvailableActions), [
                typeof(GamePlayerOwner),
                typeof(IInteractive)
            ]);
        }

        [PatchPrefix]
        public static bool PatchPrefix(ref AvailableInteractionState __result, GamePlayerOwner owner, IInteractive interactive)
        {
            if (interactive == null) return true;
            
            LootItem lootItem = interactive as LootItem;
            if (lootItem != null && lootItem.Item is DroneItem)
            {
                AvailableInteractionState returnClass = InteractionContextHelper.GetAvailableActions(owner, lootItem);
                if (returnClass != null)
                {
                    InteractionAction takeAction = null;
                    foreach (InteractionAction action in returnClass.Actions)
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
                            NotificationManager.DisplayMessageNotification(
                                "IS BEING PILOTED".Localized(),
                                ENotificationDurationType.Default,
                                ENotificationIconType.Alert
                            );
                        };
                    }
                    
                    returnClass.CreateAction("USE DRONE".Localized(), () => DroneHelper.UseDrone(droneController));
                    returnClass.CreateAction("FLIP DRONE".Localized(), () => DroneHelper.FlipDrone(droneController));

                    __result = returnClass;
                    return false;
                }
            }

            return true;
        }
    }
}
#endif
