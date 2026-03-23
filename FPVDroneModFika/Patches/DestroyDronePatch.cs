using Comfort.Common;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib;
using FPVDroneModClient.Components.Base;
using FPVDroneModFika.Components;
using FPVDroneModFika.Data;
using FPVDroneModFika.Packets;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace FPVDroneModFika.Patches
{
    public class DestroyDronePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BaseDroneController), nameof(BaseDroneController.DestroyDrone));
        }

        [PatchPostfix]
        private static void PatchPostfix(BaseDroneController __instance)
        {
            if (__instance.Owner.IsYourPlayer)
            {
                DroneSyncComponent sync = __instance.GetComponent<DroneSyncComponent>();

                DroneDestroyData data = new DroneDestroyData
                {
                    DroneNetId = sync.NetId,
                };

                DroneDestroyPacket packet = new DroneDestroyPacket()
                {
                    Data = data
                };
                
                Singleton<IFikaNetworkManager>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered, true);
            }
        }
    }
}
