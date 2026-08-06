using BSG.CameraEffects;
using Comfort.Common;
using EFT;
using EFT.CameraControl;
using EFT.Communications;
using EFT.Interactive;
using EFT.InventoryLogic;
using EFT.UI;
using FPVDroneModClient.Components;
using FPVDroneModClient.Enum;
using FPVDroneModClient.Globals;
using FPVDroneModClient.Items;
using System.Collections.Generic;
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Components.Drone;
using FPVDroneModClient.Components.Gear;
using UnityEngine;
using NightVision = BSG.CameraEffects.NightVision;

namespace FPVDroneModClient.Helpers
{
    public static class DroneHelper
    {
        public static List<BaseDroneController> SelectedControllers = [];
        public static BaseDroneController CurrentController;
        public static bool IsControllingDrone;
        public static float LastFov = 0f;
        public static float LastNearClip = 0f;
        
        public static bool LastNvgEnabled;
        public static Texture LastNvgMask;
        public static bool LastThermalEnabled;
        public static Texture LastVisorMask;

        private static int _maskId = Shader.PropertyToID("_Mask");

        public static void ControlDrone(bool newState)
        {
            if (!CanPilotDrone(out EDronePilotFailReason failReason) && newState)
            {
                DebugLogger.LogWarning($"Can't pilot drone due to: ${failReason}");

                string failReasonString = GetFailReasonString(failReason);

                if (failReasonString != null)
                {
                    NotificationManager.DisplayMessageNotification(
                        failReasonString,
                        ENotificationDurationType.Default,
                        ENotificationIconType.Alert
                    );
                }

                return;
            }

            if (DroneCullingManager.Instance)
            {
                if (IsControllingDrone && !newState)
                {
                    DroneCullingManager.Instance.EnableCulling();
                }

                if (!IsControllingDrone && newState)
                {
                    DroneCullingManager.Instance.DisableCulling();
                }
            }

            IsControllingDrone = newState;

            Player localPlayer = InstanceHelper.LocalPlayer;

            InstanceHelper.HudCamera.enabled = newState;
            InstanceHelper.PostProcessCamera.enabled = newState;
            InstanceHelper.StaticEffect.enabled = newState;
            localPlayer.PointOfView = newState ? EPointOfView.ThirdPerson : EPointOfView.FirstPerson;
            Singleton<CommonUI>.Instance.EftBattleUIScreen.CanvasGroup.gameObject.SetActive(!newState);
            PhysicsExtensions.Simulation.UpdateMode = newState ? PhysicsExtensions.Simulation.UpdateModeType.FixedUpdate : PhysicsExtensions.Simulation.UpdateModeType.SmoothSimulate;

            EftGamePlayerOwner playerOwner = localPlayer.GetComponent<EftGamePlayerOwner>();
            playerOwner.enabled = !newState;
            
            VisorEffect visorEffect = CameraManager.Instance.VisorEffect;
            NightVision nightVision = CameraManager.Instance.NightVision;
            ThermalVision thermalVision = CameraManager.Instance.ThermalVision;
            Material visorMaterial = visorEffect.GetMaterial();

            if (CurrentController)
            {
                // TODO: this is aids, redo it someday... inb4 1 year later
                LootItem lootItem = CurrentController.GetComponent<LootItem>();
                DroneItem item = (DroneItem)lootItem.Item;
                
                if (newState)
                {
                    CurrentController.OnPilotEnter();
                    
                    LastNvgEnabled = nightVision.On;
                    LastNvgMask = nightVision.TextureMask.Mask;
                    LastThermalEnabled = thermalVision.On;
                    LastVisorMask = visorMaterial.GetTexture("_Mask");

                    nightVision.On = false;
                    nightVision.TextureMask.enabled = false;
                    thermalVision.On = false;
                    thermalVision.TextureMask.enabled = false;
                    visorMaterial.SetTexture("_Mask", null);
                    
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
                    else if (item.HasNightVisionModule())
                    {
                        nightVision.On = true;
                        nightVision.TextureMask.Mask = AssetHelper.DroneNightVisionMask;
                        nightVision.Material.SetTexture(_maskId, AssetHelper.DroneNightVisionLens); // but for why
                        nightVision.Color = new Color(60, 235, 100) / 255f;
                        nightVision.NoiseIntensity = 0.25f;
                        nightVision.NoiseScale = 0.15f;
                    }
                    else
                    {
                        nightVision.On = false;
                        nightVision.TextureMask.enabled = false;
                        thermalVision.On = false;
                        thermalVision.TextureMask.enabled = false;
                    }
                }
                else
                {
                    CurrentController.OnPilotExit();
                }
            }

            Camera camera = CameraManager.Instance.Camera;
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
                visorMaterial.SetTexture("_Mask", LastVisorMask);
                
                nightVision.On = LastNvgEnabled;
                nightVision.TextureMask.enabled = LastNvgEnabled || LastThermalEnabled;
                nightVision.TextureMask.Mask = LastNvgMask;
                thermalVision.On = LastThermalEnabled;
                thermalVision.TextureMask.enabled = LastNvgEnabled || LastThermalEnabled;

                if (LastNvgEnabled)
                {
                    nightVision.ApplySettings();
                }

                if (LastThermalEnabled)
                {
                    thermalVision.SetMask(NightVisionComponent.EMask.Thermal);
                }
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
            
            if (CurrentController?.SignalController?.HasSignal == false)
            {
                failReason = EDronePilotFailReason.NoSignal;
                return false;
            }

            return true;
        }

        public static string GetFailReasonString(EDronePilotFailReason failReason)
        {
            return failReason switch
            {
                EDronePilotFailReason.NoDrone => "NO DRONE".Localized(),
                EDronePilotFailReason.NoHelmet => "NO HEADSET".Localized(),
                EDronePilotFailReason.NoDroneNearby => "NO SELECTED OR NEARBY".Localized(), // TODO: add this
                EDronePilotFailReason.NoSignal => "NO SIGNAL".Localized(),
                EDronePilotFailReason.NoController => "NO CONTROLLER".Localized(), // shouldn't happen
                EDronePilotFailReason.NoBattery => "NO BATTERY".Localized(),
                EDronePilotFailReason.NotOwner => "NOT OWNER".Localized(),
                _ => null // shouldn't happen
            };
        }

        public static void UseDrone(BaseDroneController controller)
        {
            if (controller != null)
            {
                NotificationManager.DisplayMessageNotification(
                    "SELECTED DRONE".Localized()
                );

                CurrentController = controller;
                SelectedControllers.Add(controller);
            }
        }

        public static void FlipDrone(BaseDroneController controller)
        {
            if (controller != null)
            {
                NotificationManager.DisplayMessageNotification(
                    "FLIPPED DRONE".Localized()
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
                NotificationManager.DisplayMessageNotification(
                    "No selected drones", // todo: localize
                    ENotificationDurationType.Default,
                    ENotificationIconType.Alert
                );
                return;
            }

            List<BaseDroneController> processed = [];
            AvailableInteractionState actions = new AvailableInteractionState();

            foreach (BaseDroneController controller in SelectedControllers)
            {
                if (controller == null || !controller.gameObject.activeInHierarchy || processed.Contains(controller))
                {
                    continue;
                }
                
                LootItem lootItem = controller.GetComponent<LootItem>();

                if (lootItem)
                {
                    actions.CreateAction(lootItem.Item.ShortName, () => OnDroneSelectedAction(controller));
                    processed.Add(controller);
                }
            }

            if (actions.Actions.Count > 0)
            {
                EftGamePlayerOwner playerOwner = InstanceHelper.LocalPlayer.GetComponent<EftGamePlayerOwner>();
                playerOwner.AvailableInteractionState.Value = actions;
                playerOwner.AvailableInteractionState.Value.SelectAction(actions.Actions[0]);
            }
        }

        private static void OnDroneSelectedAction(BaseDroneController controller)
        {
            if (controller.Owner == null) return;
            
            if (controller.Owner.IsYourPlayer)
            {
                CurrentController = controller;
            
                NotificationManager.DisplayMessageNotification(
                    "SELECTED DRONE".Localized()
                );
            
                EftGamePlayerOwner playerOwner = InstanceHelper.LocalPlayer.GetComponent<EftGamePlayerOwner>();
                playerOwner.ClearInteractionState();
            }
            else
            {
                NotificationManager.DisplayMessageNotification(
                    "NOT OWNER".Localized(),
                    ENotificationDurationType.Default,
                    ENotificationIconType.Alert
                );
            }
        }
    }
}

