using Comfort.Common;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Helpers;
using FPVDroneModFika.Components;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace FPVDroneModFika.Patches
{
    public class DroneDroppedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.ThrowItem), [
                typeof(Item),
                typeof(IPlayer),
                typeof(Vector3?)
            ]);
        }

        [PatchPostfix]
        private static void PatchPostfix(IPlayer player, LootItem __result)
        {
            BaseDroneController droneController = __result.GetComponentInChildren<BaseDroneController>();

            if (droneController != null)
            {
                DebugLogger.LogInfo("drone spawned");
                FikaDroneManager.Instance.OnDroneSpawned(droneController);
            }
        }
    }
}
