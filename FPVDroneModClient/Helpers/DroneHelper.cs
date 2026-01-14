#if !UNITY_EDITOR
using BSG.CameraEffects;
using Comfort.Common;
using EFT;
using EFT.Communications;
using EFT.Interactive;
using EFT.InventoryLogic;
using EFT.UI;
using FPVDroneModClient.Components;
using FPVDroneModClient.Config;
using FPVDroneModClient.Enum;
using FPVDroneModClient.Globals;
using FPVDroneModClient.Items;
using System.Collections.Generic;
using FPVDroneModClient.Components.Base;
using UnityEngine;

namespace FPVDroneModClient.Helpers
{
    public static class DroneHelper
    {
        public static List<BaseDroneController> SelectedControllers = [];
        public static BaseDroneController CurrentController;
        public static bool IsControllingDrone;
        public static float LastFov = 0f;
        public static float LastNearClip = 0f;

        private static int _maskId = Shader.PropertyToID("_Mask");

        public static void ControlDrone(bool newState)
        {
            if (!CanPilotDrone(out EDronePilotFailReason failReason) && newState)
            {
                DebugLogger.LogWarning($"Can't pilot drone due to: ${failReason}");

                string failReasonString = GetFailReasonString(failReason);

                if (failReasonString != null)
                {
                    NotificationManagerClass.DisplayMessageNotification(
                        GetFailReasonString(failReason),
                        ENotificationDurationType.Default,
                        ENotificationIconType.Alert
                    );
                }

                return;
            }

            IsControllingDrone = newState;

            InstanceHelper.HudCamera.enabled = newState;
            InstanceHelper.PostProcessCamera.enabled = newState;
            InstanceHelper.StaticEffect.enabled = newState;
            InstanceHelper.LocalPlayer.PointOfView = newState ? EPointOfView.ThirdPerson : EPointOfView.FirstPerson;
            Singleton<CommonUI>.Instance.EftBattleUIScreen.CanvasGroup.gameObject.SetActive(!newState);
            EFTPhysicsClass.SyncTransformsClass.UpdateMode = newState ? EFTPhysicsClass.SyncTransformsClass.UpdateModeType.FixedUpdate : EFTPhysicsClass.SyncTransformsClass.UpdateModeType.SmoothSimulate;

            EftGamePlayerOwner playerOwner = InstanceHelper.LocalPlayer.GetComponent<EftGamePlayerOwner>();
            playerOwner.enabled = !newState;

            if (CurrentController)
            {
                LootItem lootItem = CurrentController.GetComponent<LootItem>();
                DroneItem item = (DroneItem)lootItem.Item;
                
                NightVision nightVision = CameraClass.Instance.NightVision;
                ThermalVision thermalVision = CameraClass.Instance.ThermalVision;
                
                if (newState)
                {
                    CurrentController.OnPilotEnter();
                    
                    if (item.HasThermalModule())
                    {
                        thermalVision.On = true;
                        thermalVision.IsFpsStuck = true;
                        thermalVision.StuckFpsUtilities.MinFramerate = 60;
                        thermalVision.StuckFpsUtilities.MaxFramerate = 60;
                        thermalVision.IsPixelated = false;
                        thermalVision.IsNoisy = false;
                        thermalVision.IsGlitch = false;
                    }
                    
                    if (item.HasNightVisionModule())
                    {
                        nightVision.On = true;
                        nightVision.Mask = AssetHelper.DroneNightVisionMask;
                        nightVision.Material_0.SetTexture(_maskId, AssetHelper.DroneNightVisionLens); // but for why
                        nightVision.Color = new Color(60, 235, 100) / 255f;
                        nightVision.NoiseIntensity = 0.25f;
                        nightVision.NoiseScale = 0.15f;
                    }
                }
                else
                {
                    CurrentController.OnPilotExit();
                    
                    CameraClass.Instance.ThermalVision.On = false;
                    CameraClass.Instance.NightVision.On = false;
                }
            }

            if (GeneralConfig.DisableCulling.Value && DroneCullingManager.Instance)
            {
                DroneCullingManager.Instance.SetCullingState(!newState);
            }

            Camera camera = CameraClass.Instance.Camera;
            if (newState)
            {
                LastFov = camera.fieldOfView;
                LastNearClip = camera.nearClipPlane;
                
                camera.nearClipPlane = Plugin.CameraNearClip.Value;
                
                playerOwner.ClearInteractionState();
            }
            else
            {
                camera.fieldOfView = LastFov;
                camera.nearClipPlane = LastNearClip;
            }
        }

        public static bool CanPilotDrone(out EDronePilotFailReason failReason)
        {
            failReason = EDronePilotFailReason.None;

            Item currentHelmet = PlayerHelper.GetEquipmentItemOfId(ItemIds.HeadsetTemplateId)?.ContainedItem;
            Weapon currentWeapon = PlayerHelper.GetEquippedWeapon();

            if (currentWeapon == null || currentWeapon.StringTemplateId != ItemIds.ControllerTemplateId)
            {
                failReason = EDronePilotFailReason.NoController;
                return false;
            }

            if (currentHelmet == null || currentHelmet.StringTemplateId != ItemIds.HeadsetTemplateId)
            {
                failReason = EDronePilotFailReason.NoHelmet;
                return false;
            }

            if (!CurrentController)
            {
                failReason = EDronePilotFailReason.NoDrone;
                return false;
            }

            return true;
        }

        public static string GetFailReasonString(EDronePilotFailReason failReason)
        {
            return failReason switch
            {
                EDronePilotFailReason.NoDrone => "No drone selected",
                EDronePilotFailReason.NoHelmet => "No headset equipped",
                EDronePilotFailReason.NoDroneNearby => "No drone selected and no drone nearby", // TODO: add this
                EDronePilotFailReason.NoController => null, // shouldn't happen
                _ => null // shouldn't happen
            };
        }

        public static void UseDrone(LootItem lootItem)
        {
            BaseDroneController controller = lootItem.GetComponentInChildren<BaseDroneController>(true);

            if (controller != null)
            {
                NotificationManagerClass.DisplayMessageNotification(
                    "Successfully selected drone"
                );

                CurrentController = controller;
                SelectedControllers.Add(controller);
            }
        }

        public static void FlipDrone(LootItem lootItem)
        {
            BaseDroneController controller = lootItem.GetComponentInChildren<BaseDroneController>(true);

            if (controller != null)
            {
                NotificationManagerClass.DisplayMessageNotification(
                    "Flipped drone"
                );

                Vector3 current = controller.gameObject.transform.eulerAngles;
                current.z = 0;
                controller.gameObject.transform.eulerAngles = current;
            }
        }

        public static void ShowSelectedDrones()
        {
            if (SelectedControllers.Count <= 0)
            {
                PlayerHelper.ShowNotification("No selected drones", ENotificationDurationType.Default, ENotificationIconType.Alert);
                return;
            }
            
            ActionsReturnClass actions = new ActionsReturnClass();

            foreach (BaseDroneController controller in SelectedControllers)
            {
                if (controller == null || !controller.gameObject.activeInHierarchy)
                {
                    continue;
                }
                
                LootItem lootItem = controller.GetComponent<LootItem>();

                if (lootItem)
                {
                    actions.CreateAction(lootItem.Item.ShortName, () => OnDroneSelectedAction(controller));
                }
            }
            
            EftGamePlayerOwner playerOwner = InstanceHelper.LocalPlayer.GetComponent<EftGamePlayerOwner>();
            playerOwner.AvailableInteractionState.Value = actions;
            playerOwner.AvailableInteractionState.Value.SelectAction(actions.Actions[0]);
        }

        private static void OnDroneSelectedAction(BaseDroneController controller)
        {
            CurrentController = controller;
            
            NotificationManagerClass.DisplayMessageNotification(
                "Successfully selected drone"
            );
            
            EftGamePlayerOwner playerOwner = InstanceHelper.LocalPlayer.GetComponent<EftGamePlayerOwner>();
            playerOwner.ClearInteractionState();
        }
    }
}
#endif
