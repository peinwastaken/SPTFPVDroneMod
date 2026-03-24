using Comfort.Common;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib;
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Models;
using FPVDroneModFika.Components;
using FPVDroneModFika.Data;
using FPVDroneModFika.Packets;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneModFika.Patches
{
    public class PayloadExplodePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BasePayloadController), nameof(BasePayloadController.Detonate));
        }

        [PatchPostfix]
        private static void PatchPostfix(BasePayloadController __instance, ExplosionData __result)
        {
            DroneSyncComponent droneSync = __instance.DroneController.GetComponent<DroneSyncComponent>();
            
            DroneExplosionData data = new DroneExplosionData
            {
                DroneNetId = droneSync.NetId,
                OwnerProfileId = __result.PlayerOwner.iPlayer.ProfileId,
                Position = __result.Position,
                Damage = __result.Damage,
                MaxDistance = __result.MaxDistance,
                HeavyBleedDelta = __result.HeavyBleedDelta,
                LightBleedDelta = __result.LightBleedDelta,
                FractureDelta = __result.FractureDelta,
                StaminaBurnRate = __result.StaminaBurnRate
            };
            
            DroneExplosionPacket packet = new DroneExplosionPacket
            {
                Data = data
            };
            
            Singleton<IFikaNetworkManager>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered, true);
        }
    }
}
