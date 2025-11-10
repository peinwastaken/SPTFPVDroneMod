#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Dissonance;
using DrakiaXYZ.BigBrain.Brains;
using EFT.InventoryLogic;
using FPVDroneMod.Bots.Layers;
using FPVDroneMod.Config;
using FPVDroneMod.Globals;
using FPVDroneMod.Helpers;
using FPVDroneMod.Patches;
using System.Collections.Generic;
using UnityEngine;

namespace FPVDroneMod
{
    [BepInPlugin("com.pein.fpvdronemod", "SPTFPVDroneMod", "0.2.0")]
    [BepInDependency("xyz.drakia.bigbrain")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        public static ConfigEntry<float> CameraNearClip;

        private void Awake()
        {
            Logger = base.Logger;
            DebugLogger.Logger = Logger;

            GeneralConfig.Bind(0, Category.General, Config);
            DroneConfig.Bind(1, Category.Drone, Config);
            BindsConfig.Bind(2, Category.Binds, Config);
            PostProcessConfig.Bind(3, Category.PP, Config);

            AssetHelper.LoadBundles();
            AssetHelper.LoadAssets();
            AssetHelper.LoadSounds();

            new InteractionPatch().Enable();
            new CameraPositionPatch().Enable();
            new SetCameraPatch().Enable();
            new GameStartedPatch().Enable();
            new WeaponInputPatch().Enable();
            new LootItemPhysicsPatch().Enable();
            //+new BotActivatePatch().Enable();

            BrainManager.AddCustomLayer(typeof(DroneCombatLayer),
                [
                    "Assault",
                    "PMC"
                ],
                9999
            );

            CameraNearClip = Config.Bind("General", "Drone Camera Near Clip", 0.051f, "Changes camera near clip plane distance while piloting drone.");
        }
    }
}
#endif
