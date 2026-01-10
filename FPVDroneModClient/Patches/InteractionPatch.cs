#if !UNITY_EDITOR
using EFT;
using EFT.Interactive;
using FPVDroneModClient.Components;
using FPVDroneModClient.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;

namespace FPVDroneModClient.Patches
{
    public class InteractionPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GetActionsClass), nameof(GetActionsClass.smethod_8));
        }

        private static void CreateAction(ActionsReturnClass __result, string name, Action action)
        {
            ActionsTypesClass newAction = new ActionsTypesClass
            {
                Name = name,
                Disabled = false,
                Action = action
            };

            __result.Actions.Add(newAction);
        }

        private static void OnPickupAction(LootItem lootItem, BaseDroneController droneController)
        {
            if (droneController is FPVDroneController controller)
            {
                if (controller.DroneDetonator.Armed)
                {
                    controller.Detonate();
                }
            }

            if (droneController == DroneHelper.CurrentController)
            {
                DroneHelper.CurrentController = null;
            }
        }

        [PatchPostfix]
        public static void Postfix(ref ActionsReturnClass __result, GamePlayerOwner owner, LootItem lootItem)
        {
            BaseDroneController controller = lootItem.GetComponentInChildren<BaseDroneController>();

            DebugLogger.LogInfo($"Interacting with item: {lootItem.TemplateId}");

            if (controller)
            {
                DebugLogger.LogInfo("Interacting with drone - create actions");

                // Pick up
                __result.Actions[0].Action += () => OnPickupAction(lootItem, controller);

                CreateAction(__result, "Use", () => DroneHelper.UseDrone(lootItem));
                CreateAction(__result, "Flip", () => DroneHelper.FlipDrone(lootItem));
            }
        }
    }
}
#endif
