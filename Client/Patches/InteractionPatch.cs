#if !UNITY_EDITOR
using EFT;
using EFT.Interactive;
using FPVDroneMod.Components;
using FPVDroneMod.Globals;
using FPVDroneMod.Patches;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;

namespace FPVDroneMod.Patches
{
    public class InteractionPatch : ModulePatch
    {
        private static Type _actionType;
        
        protected override MethodBase GetTargetMethod()
        {
            _actionType = typeof(ActionsTypesClass);
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

        private static void OnPickupAction(LootItem lootItem)
        {
            EFTPhysicsClass.GClass723.UnsupportRigidbody(DroneHelper.CurrentController.RigidBody);
            DroneHelper.ControlDrone(false);
            DroneHelper.CurrentController.ResetTransform();
            
            if (lootItem.GetComponentInChildren<DroneController>() == DroneHelper.CurrentController)
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
                Plugin.Logger.LogInfo("Interacting with drone - create actions");
                
                // Pick up
                __result.Actions[0].Action += () => OnPickupAction(lootItem); 
                
                CreateAction(__result, "Use", () => DroneHelper.UseDrone(lootItem));
                CreateAction(__result, "Flip", () => DroneHelper.FlipDrone(lootItem));
            }
        }
    }
}
#endif