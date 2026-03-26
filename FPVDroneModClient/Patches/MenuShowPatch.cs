using BepInEx.Bootstrap;
using EFT;
using EFT.Communications;
using EFT.UI;
using FPVDroneModClient.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneModClient.Patches
{
    public class MenuShowPatch : ModulePatch
    {
        private static bool _warningShown = false;
        
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MenuScreen), nameof(MenuScreen.Show), [
                typeof(Profile),
                typeof(MatchmakerPlayerControllerClass),
                typeof(ESessionMode)
            ]);
        }

        [PatchPostfix]
        private static void PatchPostfix()
        {
            if (!_warningShown)
            {
                bool fikaInstalled = Chainloader.PluginInfos.ContainsKey("com.fika.core");
                DebugLogger.LogInfo($"fika installed: {fikaInstalled}");
                bool syncInstalled = Chainloader.PluginInfos.ContainsKey("com.pein.fpvdronemodfikasync");
                DebugLogger.LogInfo($"drone mod sync installed: {syncInstalled}");
            
                if (fikaInstalled && !syncInstalled)
                {
                    NotificationManagerClass.DisplayMessageNotification(
                        "FPVMOD USING FIKA WARNING".Localized(),
                        ENotificationDurationType.Long,
                        ENotificationIconType.Alert);
                }

                _warningShown = true;
            }
        }
    }
}
