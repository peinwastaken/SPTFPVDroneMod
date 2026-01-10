#if !UNITY_EDITOR
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
using UnityEngine;

namespace FPVDroneModClient.Helpers
{
    public static class DroneHelper
    {
        public static BaseDroneController CurrentController;
        public static bool IsControllingDrone;

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

            InstanceHelper.StaticEffect.enabled = newState;
            InstanceHelper.LocalPlayer.PointOfView = newState ? EPointOfView.ThirdPerson : EPointOfView.FirstPerson;
            Singleton<CommonUI>.Instance.EftBattleUIScreen.CanvasGroup.gameObject.SetActive(!newState);
            EFTPhysicsClass.SyncTransformsClass.UpdateMode = newState ? EFTPhysicsClass.SyncTransformsClass.UpdateModeType.FixedUpdate : EFTPhysicsClass.SyncTransformsClass.UpdateModeType.SmoothSimulate;

            EftGamePlayerOwner playerOwner = InstanceHelper.LocalPlayer.GetComponent<EftGamePlayerOwner>();
            playerOwner.enabled = !newState;

            if (CurrentController)
            {
                if (newState)
                {
                    CurrentController.OnPilotEnter();
                }
                else
                {
                    CurrentController.OnPilotExit();
                }
            }

            if (GeneralConfig.DisableCulling.Value && DroneCullingManager.Instance)
            {
                DroneCullingManager.Instance.SetCullingState(!newState);
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
    }
}
#endif
