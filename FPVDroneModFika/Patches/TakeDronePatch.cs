using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneModFika.Patches
{
    [IgnoreAutoPatch]
    public class TakeDronePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(DroneHelper), nameof(DroneHelper.PickUpDrone));
        }

        [PatchPrefix]
        private static bool PatchPrefix(BaseDroneController controller)
        {
            if (!controller.Owner.IsYourPlayer)
            {
                NotificationManagerClass.DisplayMessageNotification(
                    "NOT OWNER".Localized()
                );
                
                return false;
            }

            return true;
        }
    }
}
