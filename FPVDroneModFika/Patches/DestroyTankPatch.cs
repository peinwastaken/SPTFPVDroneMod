using Comfort.Common;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib;
using FPVDroneModClient.Helpers;
using FPVDroneModFika.Data;
using FPVDroneModFika.Packets;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace FPVDroneModFika.Patches
{
    public class DestroyTankPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(InstanceHelper), nameof(InstanceHelper.CreateTankCorpse));
        }

        [PatchPostfix]
        private static void PatchPostfix(Vector3 pos, Vector3 euler, bool wasJustDestroyed, bool wasDestroyedLocally)
        {
            if (wasDestroyedLocally)
            {
                TankDestroyData data = new TankDestroyData
                {
                    Position = pos,
                    EulerAngles = euler
                };

                TankDestroyPacket packet = new TankDestroyPacket()
                {
                    Data = data
                };
                
                Singleton<IFikaNetworkManager>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered, true);
            }
        }
    }
}
