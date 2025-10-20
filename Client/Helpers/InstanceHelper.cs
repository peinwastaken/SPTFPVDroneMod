#if !UNITY_EDITOR
using Comfort.Common;
using EFT;
using FPVDroneMod.Components;
using UnityEngine;

namespace FPVDroneMod.Helpers
{
    public static class InstanceHelper
    {
        public static Player LocalPlayer => Singleton<GameWorld>.Instance.MainPlayer;
        public static Camera Camera => CameraClass.Instance.Camera;
        public static FPVScreenPostProcess StaticEffect => Camera.GetComponent<FPVScreenPostProcess>();
        public static DroneHudController DroneHudController = null;
        public static Canvas DroneHudCanvas = null;
    }
}
#endif