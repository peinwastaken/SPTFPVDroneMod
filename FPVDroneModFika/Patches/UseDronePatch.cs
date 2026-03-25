using EFT.Communications;
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneModFika.Patches
{
    public class UseDronePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(DroneHelper), nameof(DroneHelper.UseDrone));
        }

        [PatchPrefix]
        private static bool PatchPrefix(BaseDroneController controller)
        {
            if (!controller.Owner.IsYourPlayer)
            {
                NotificationManagerClass.DisplayMessageNotification(
                    "NOT OWNER".Localized(),
                    ENotificationDurationType.Default,
                    ENotificationIconType.Alert
                );
                
                return false;
            }

            return true;
        }
    }
}
