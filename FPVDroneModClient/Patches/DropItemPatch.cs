#if !UNITY_EDITOR
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace FPVDroneModClient.Patches
{
    public class DropItemPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.ThrowItem), [
                typeof(Item),
                typeof(IPlayer),
                typeof(Vector3),
                typeof(Quaternion),
                typeof(Vector3),
                typeof(Vector3),
                typeof(bool),
                typeof(bool),
                typeof(float)
            ]);
        }

        [PatchPostfix]
        private static void PatchPostfix(IPlayer player, LootItem __result)
        {
            if (__result == null) return;
            
            BaseDroneController droneController = __result.GetComponentInChildren<BaseDroneController>();
            if (droneController != null)
            {
                DebugLogger.LogInfo($"player {player.Profile.Nickname} dropped drone, assigning owner");
                droneController.Owner = player;
                droneController.IsBeingControlled = false;
                droneController.IsInInventory = false;
                droneController.WasJustDropped = true;
            }
        }
    }
}
#endif