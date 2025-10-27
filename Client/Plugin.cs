#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using FPVDroneMod.Config;
using FPVDroneMod.Globals;
using FPVDroneMod.Helpers;
using FPVDroneMod.Patches;
using UnityEngine;

namespace FPVDroneMod
{
    [BepInPlugin("com.pein.fpvdronemod", "SPTFPVDroneMod", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        public static ConfigEntry<float> CameraNearClip;

        private void Awake()
        {
            Logger = base.Logger;
            
            AssetHelper.LoadAssets();
            AssetHelper.LoadSounds();
            
            DroneConfig.Bind(1, Category.Drone, Config);
            BindsConfig.Bind(2, Category.Binds, Config);
            
            new InteractionPatch().Enable();
            new KeyboardInputPatch().Enable();
            new WeaponInputPatch().Enable();
            new MouseInputPatch().Enable();
            new CameraPositionPatch().Enable();
            new SetCameraPatch().Enable();
            new CreateRotatorPatch().Enable();
            new OnRigidBodyStartedPatch().Enable();
            new OnRigidBodyStoppedPatch().Enable();
            new GameStartedPatch().Enable();
            //new EFTPhysicsUpdatePatch().Enable();

            CameraNearClip = Config.Bind("General", "Drone Camera Near Clip", 0.051f, "Changes camera near clip plane distance while piloting drone.");
        }
    }
}
#endif