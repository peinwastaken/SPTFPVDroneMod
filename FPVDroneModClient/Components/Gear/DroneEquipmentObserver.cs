using EFT;
using EFT.CameraControl;
using EFT.InventoryLogic;
using FPVDroneModClient.Helpers;
using System;
using UnityEngine;

namespace FPVDroneModClient.Components.Gear
{
    public class DroneEquipmentObserver : MonoBehaviour
    {
        public Player Player;
        
        public Player.GClass2059<FaceShieldComponent> EyeWearObserver;

        private void Awake()
        {
            Player = GetComponent<Player>();
            if (Player == null)
            {
                DebugLogger.LogWarning("Player is null! :^(");
                Destroy(this);
            }
            
            Slot eyeSlot = Player.Equipment.GetSlot(EquipmentSlot.Eyewear);
            EyeWearObserver = new Player.GClass2059<FaceShieldComponent>(eyeSlot, SubscribeToObserver);
        }

        private Action SubscribeToObserver(FaceShieldComponent faceShield, Action handler)
        {
            DebugLogger.LogInfo("observer subscribed to action");

            return handler;
        }

        public void OnEyeWearChanged()
        {
            DebugLogger.LogInfo("updated eyewear");
            
            FaceShieldComponent component = EyeWearObserver.Component;
            PlayerCameraController cameraController = InstanceHelper.LocalPlayer.gameObject.GetComponent<PlayerCameraController>();
            cameraController.method_3(component);
        }

        private void OnDestroy()
        {
            EyeWearObserver.Dispose();
        }
    }
}
