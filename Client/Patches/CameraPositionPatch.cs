#if !UNITY_EDITOR
using EFT.CameraControl;
using FPVDroneMod.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneMod.Patches
{
    public class CameraPositionPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(PlayerCameraController), nameof(PlayerCameraController.LateUpdate));
        }

        [PatchPostfix]
        private static void PatchPostfix(PlayerCameraController __instance)
        {
            if (DroneHelper.CurrentController != null && DroneHelper.IsControllingDrone)
            {
                __instance.Camera.transform.position = DroneHelper.CurrentController.CameraPos.transform.position;
                __instance.Camera.transform.rotation = DroneHelper.CurrentController.CameraPos.transform.rotation;
                __instance.Camera.nearClipPlane = Plugin.CameraNearClip.Value;
            }
            else
            {
                __instance.Camera.nearClipPlane = 0.03f;
            }
        }
    }
}
#endif