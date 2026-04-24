using BepInEx;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using Fika.Core.Modding;
using Fika.Core.Modding.Events;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib.Utils;
using FPVDroneModClient.Helpers;
using FPVDroneModFika.Components;
using FPVDroneModFika.Packets;
using SPT.Reflection.Patching;

namespace FPVDroneModFika
{
    [BepInPlugin("com.pein.fpvdronemodfikasync", "FPVDroneModFikaSync", "0.1.1")]
    [BepInDependency("com.pein.fpvdronemod", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.fika.core", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;
            
            PatchManager patchManager = new PatchManager(this, true);
            patchManager.EnablePatches();
            
            FikaEventDispatcher.SubscribeEvent<FikaNetworkManagerCreatedEvent>(RegisterPackets);
        }

        private static void RegisterPackets(FikaNetworkManagerCreatedEvent fikaNetworkManagerCreatedEvent)
        {
            DebugLogger.LogInfo("registering drone mod fika sync packets");
            
            IFikaNetworkManager networkManager = Singleton<IFikaNetworkManager>.Instance;
            
            networkManager.RegisterPacket<DronePositionPacket>(packet => FikaDroneManager.Instance.OnReceivedPositionPacket(packet));
            networkManager.RegisterPacket<DroneControlPacket>(packet => FikaDroneManager.Instance.OnReceivedControlPacket(packet));
            networkManager.RegisterPacket<DroneExplosionPacket>(packet => FikaDroneManager.Instance.OnReceivedExplosionPacket(packet));
            networkManager.RegisterPacket<DroneDestroyPacket>(packet => FikaDroneManager.Instance.OnReceivedDestroyPacket(packet));
            networkManager.RegisterPacket<TankDestroyPacket>(packet => FikaDroneManager.Instance.OnReceivedTankDestroyPacket(packet));
        }
    }
}
