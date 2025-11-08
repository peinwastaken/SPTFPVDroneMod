#if !UNITY_EDITOR
using Comfort.Common;
using EFT;
using EFT.Communications;
using EFT.Interactive;
using EFT.InventoryLogic;
using EFT.UI;
using FPVDroneMod.Components;
using FPVDroneMod.Enum;
using FPVDroneMod.Globals;
using UnityEngine;

namespace FPVDroneMod.Helpers
{
    public static class DroneHelper
    {
        public static DroneController CurrentController = null;
        public static bool IsControllingDrone = false;

        public static void ControlDrone(bool newState)
        {
            if (!CanPilotDrone(out EDronePilotFailReason failReason) && newState)
            {
                Plugin.Logger.LogWarning($"Can't pilot drone due to: ${failReason}");
                
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
            
            InstanceHelper.StaticEffect.enabled = newState;
            InstanceHelper.LocalPlayer.PointOfView = newState ? EPointOfView.ThirdPerson : EPointOfView.FirstPerson;
            InstanceHelper.DroneHudController.gameObject.SetActive(newState);
            InstanceHelper.DroneHudCanvas.worldCamera = InstanceHelper.Camera;
            Singleton<CommonUI>.Instance.EftBattleUIScreen.CanvasGroup.gameObject.SetActive(!newState);
            EFTPhysicsClass.SyncTransformsClass.UpdateMode = newState ? EFTPhysicsClass.SyncTransformsClass.UpdateModeType.FixedUpdate : EFTPhysicsClass.SyncTransformsClass.UpdateModeType.SmoothSimulate;

            EftGamePlayerOwner playerOwner = InstanceHelper.LocalPlayer.GetComponent<EftGamePlayerOwner>();
            playerOwner.enabled = !newState;
            
            if (CurrentController)
            {
                CurrentController.OnControl(newState);
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
            return (failReason) switch
            {
                EDronePilotFailReason.NoDrone => "No drone selected",
                EDronePilotFailReason.NoHelmet => "No headset equipped",
                EDronePilotFailReason.NoDroneNearby => "No drone selected and no drone nearby",
                EDronePilotFailReason.NoController => null, // shouldn't happen
                _ => null, // shouldn't happen
            };
        }
        
        public static void UseDrone(LootItem lootItem)
        {
            DroneController controller = lootItem.GetComponentInChildren<DroneController>(true);

            if (controller != null)
            {
                NotificationManagerClass.DisplayMessageNotification(
                    "Successfully selected drone",
                    ENotificationDurationType.Default,
                    ENotificationIconType.Default
                );
                
                CurrentController = controller;
            }
        }

        public static void FlipDrone(LootItem lootItem)
        {
            DroneController controller = lootItem.GetComponentInChildren<DroneController>(true);

            if (controller != null)
            {
                NotificationManagerClass.DisplayMessageNotification(
                    "Flipped drone",
                    ENotificationDurationType.Default,
                    ENotificationIconType.Default
                );
                
                Vector3 current = controller.gameObject.transform.eulerAngles;
                current.z = 0;
                controller.gameObject.transform.eulerAngles = current;
            }
        }
    }
}
#endif