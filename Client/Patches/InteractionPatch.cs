#if !UNITY_EDITOR
using EFT;
using EFT.Interactive;
using FPVDroneMod.Components;
using FPVDroneMod.Globals;
using FPVDroneMod.Helpers;
using FPVDroneMod.Patches;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;

namespace FPVDroneMod.Patches
{
    public class InteractionPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GetActionsClass), nameof(GetActionsClass.smethod_8));
        }

        private static void CreateAction(ActionsReturnClass __result, string name, Action action)
        {
            ActionsTypesClass newAction = new ActionsTypesClass()
            {
                Name = name,
                Disabled = false,
                Action = action,
            };

            __result.Actions.Add(newAction);
        }

        private static void OnPickupAction(LootItem lootItem, DroneController droneController)
        {
            Plugin.Logger.LogInfo(droneController);
            Plugin.Logger.LogInfo(lootItem);
            
            if (droneController == DroneHelper.CurrentController)
            {
                DroneHelper.CurrentController = null;
            }
        }

        [PatchPostfix]
        public static void Postfix(ref ActionsReturnClass __result, GamePlayerOwner owner, LootItem lootItem)
        {
            string itemId = lootItem.TemplateId;
            
            Plugin.Logger.LogInfo($"Interacting with item: {itemId}");

            if (itemId == ItemIds.DroneTemplateId)
            {
                DroneController controller = lootItem.GetComponentInChildren<DroneController>();
                Plugin.Logger.LogInfo(controller);
                Plugin.Logger.LogInfo(lootItem);
                
                Plugin.Logger.LogInfo("Interacting with drone - create actions");
                
                // Pick up
                __result.Actions[0].Action += () => OnPickupAction(lootItem, controller); 
                
                CreateAction(__result, "Use", () => DroneHelper.UseDrone(lootItem));
                CreateAction(__result, "Flip", () => DroneHelper.FlipDrone(lootItem));
            }
        }
    }
}
#endif