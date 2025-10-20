#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using FPVDroneMod.Helpers;
using FPVDroneMod.Patches;
using UnityEngine;

namespace FPVDroneMod
{
    [BepInPlugin("com.pein.fpvdronemod", "SPTFPVDroneMod", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        public static ConfigEntry<float> CameraNearClip;

        private void Awake()
        {
            Logger = base.Logger;
            
            AssetHelper.LoadAssets();
            AssetHelper.LoadSounds();
            
            new InteractionPatch().Enable();
            new KeyboardInputPatch().Enable();
            new WeaponInputPatch().Enable();
            new MouseInputPatch().Enable();
            new CameraPositionPatch().Enable();
            new SetCameraPatch().Enable();
            new CreateRotatorPatch().Enable();
            new LootItemPhysicsPatch().Enable();
            new GameStartedPatch().Enable();

            CameraNearClip = Config.Bind("General", "Drone Camera Near Clip", 0.051f, "Changes camera near clip plane distance while piloting drone.");
        }
    }
}
#endif