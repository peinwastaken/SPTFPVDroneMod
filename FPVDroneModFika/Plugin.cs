using BepInEx;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using Fika.Core.Modding;
using Fika.Core.Modding.Events;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib.Utils;
using FPVDroneModFika.Components;
using FPVDroneModFika.Packets;
using SPT.Reflection.Patching;

namespace FPVDroneModFika
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.pein.fpvdronemod", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.fika.core", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            
            PatchManager patchManager = new PatchManager(this, true);
            patchManager.EnablePatches();
            
            FikaEventDispatcher.SubscribeEvent<FikaNetworkManagerCreatedEvent>(RegisterPackets);
        }

        private static void RegisterPackets(FikaNetworkManagerCreatedEvent fikaNetworkManagerCreatedEvent)
        {
            IFikaNetworkManager networkManager = Singleton<IFikaNetworkManager>.Instance;
            
            networkManager.RegisterPacket<DronePositionPacket>(OnPacketReceived);
            networkManager.RegisterPacket<DroneControlPacket>(OnPacketReceived);
            networkManager.RegisterPacket<DroneExplosionPacket>(OnPacketReceived);
            networkManager.RegisterPacket<DroneDestroyPacket>(OnPacketReceived);
            networkManager.RegisterPacket<TankDestroyPacket>(OnPacketReceived);
        }

        private static void OnPacketReceived(INetSerializable dronePacket)
        {
            FikaDroneManager droneManager = FikaDroneManager.Instance;
            
            switch (dronePacket)
            {
                case DronePositionPacket packet:
                    droneManager.OnReceivedPositionPacket(packet);
                    break;
                case DroneControlPacket packet:
                    droneManager.OnReceivedControlPacket(packet);
                    break;
                case DroneExplosionPacket packet:
                    droneManager.OnReceivedExplosionPacket(packet);
                    break;
                case DroneDestroyPacket packet:
                    droneManager.OnReceivedDestroyPacket(packet);
                    break;
                case TankDestroyPacket packet:
                    droneManager.OnReceivedTankDestroyPacket(packet);
                    break;
            }
        }
    }
}
