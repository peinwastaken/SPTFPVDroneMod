#if !UNITY_EDITOR
using EFT.CameraControl;
using FPVDroneMod.Config;
using FPVDroneMod.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

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
                Vector3 dronePos = DroneHelper.CurrentController.CameraPos.transform.position;
                Quaternion droneAng = DroneHelper.CurrentController.CameraPos.transform.rotation;
                float configAngleOffset = DroneConfig.DroneCameraAngleOffset.Value;

                Quaternion offset = Quaternion.AngleAxis(configAngleOffset, Vector3.right);
                droneAng = offset * droneAng;

                __instance.Camera.transform.position = dronePos;
                __instance.Camera.transform.rotation = droneAng;
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
