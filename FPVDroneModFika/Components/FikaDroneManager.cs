using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Models;
using FPVDroneModFika.Data;
using FPVDroneModFika.Packets;
using System.Collections.Generic;
using UnityEngine;

namespace FPVDroneModFika.Components
{
    public class FikaDroneManager : MonoBehaviour
    {
        public static FikaDroneManager Instance;
        
        public Dictionary<int, DroneSyncComponent> SyncComponents = [];

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            
            Instance = this;
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void OnDroneSpawned(BaseDroneController droneController)
        {
            DroneSyncComponent syncComponent = droneController.GetComponent<DroneSyncComponent>();
            
            // if drone is not being synced then give it a syncer
            if (syncComponent == null)
            {
                DroneSyncComponent c = droneController.gameObject.AddComponent<DroneSyncComponent>();
                c.NetId = SyncComponents.Count;
                SyncComponents.Add(SyncComponents.Count, c);
            }
        }

        public void OnReceivedPositionPacket(DronePositionPacket packet)
        {
            // Plugin.Logger.LogInfo($"received drone pos packet for droneNetId: {packet.Data.DroneNetId}");
            
            SyncComponents.TryGetValue(packet.Data.DroneNetId, out DroneSyncComponent syncComponent);

            if (syncComponent != null)
            {
                syncComponent.SyncDronePosition(packet);
            }
        }

        public void OnReceivedControlPacket(DroneControlPacket packet)
        {
            Plugin.Logger.LogInfo($"received drone control packet for droneNetId: {packet.Data.DroneNetId}");
            
            SyncComponents.TryGetValue(packet.Data.DroneNetId, out DroneSyncComponent syncComponent);

            if (syncComponent != null)
            {
                syncComponent.SyncDroneControl(packet);
            }
        }

        public void OnReceivedExplosionPacket(DroneExplosionPacket packet)
        {
            Plugin.Logger.LogInfo($"received drone explosion packet at position {packet.Data.Position.ToString()}");

            DroneExplosionData data = packet.Data;

            if (SyncComponents.TryGetValue(data.DroneNetId, out DroneSyncComponent sync) && sync != null)
            {
                IPlayerOwner playerOwner = Singleton<GameWorld>.Instance.GetAlivePlayerBridgeByProfileID(data.OwnerProfileId);
                
                ExplosionHelper.CreateExplosion(new ExplosionData
                {
                    Position = data.Position,
                    MaxDistance = data.MaxDistance,
                    Damage = data.Damage,
                    FractureDelta = data.FractureDelta,
                    HeavyBleedDelta = data.HeavyBleedDelta,
                    LightBleedDelta = data.LightBleedDelta,
                    StaminaBurnRate = data.StaminaBurnRate,
                    PlayerOwner = playerOwner,
                    Weapon = sync.DroneController.Item,
                });
            }
        }

        public void OnReceivedDestroyPacket(DroneDestroyPacket packet)
        {
            Plugin.Logger.LogInfo($"received drone destroy packet for droneNetId: {packet.Data.DroneNetId}");

            DroneDestroyData data = packet.Data;

            SyncComponents.TryGetValue(data.DroneNetId, out DroneSyncComponent syncComponent);

            if (syncComponent != null)
            {
                syncComponent.DroneController.DestroyDrone();
            }
        }

        public void OnReceivedTankDestroyPacket(TankDestroyPacket packet)
        {
            Plugin.Logger.LogInfo($"received tank destroy packet");
        }
    }
}
