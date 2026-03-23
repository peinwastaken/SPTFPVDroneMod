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

namespace FPVDroneModFika.Patches
{
    public class DronePilotEnterPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BaseDroneController), nameof(BaseDroneController.OnPilotEnter));
        }

        [PatchPostfix]
        private static void PatchPostfix(BaseDroneController __instance)
        {
            if (__instance.Owner.IsYourPlayer)
            {
                DroneSyncComponent sync = __instance.GetComponent<DroneSyncComponent>();

                DroneControlData data = new DroneControlData
                {
                    DroneNetId = sync.NetId,
                    IsDroneControlled = true
                };

                DroneControlPacket packet = new DroneControlPacket
                {
                    Data = data
                };
                
                Singleton<IFikaNetworkManager>.Instance.SendData(ref packet, DeliveryMethod.ReliableUnordered, true);
            }
        }
    }
}
