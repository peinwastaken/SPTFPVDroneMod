 #if !UNITY_EDITOR
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using DrakiaXYZ.BigBrain.Brains;
using EFT.CameraControl;
using EFT.Communications;
using EFT.InventoryLogic;
using FPVDroneModClient.Bots.Layers;
using FPVDroneModClient.Config;
using FPVDroneModClient.Globals;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Items;
using System.Collections.Generic;
using FPVDroneModClient.Models;
using SPT.Reflection.Patching;
using System.Collections;
using UnityEngine;
using WTTClientCommonLib.Services;

namespace FPVDroneModClient
{
    [BepInPlugin("com.pein.fpvdronemod", "SPTFPVDroneMod", "0.8.0")]
    [BepInDependency("xyz.drakia.bigbrain")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        public static Plugin Instance;
        public static TankDeathState TankDeathState;
        public static ConfigEntry<float> CameraNearClip;
        
        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;
            
            DebugLogger.Logger = Logger;
            
            PatchManager patchManager = new PatchManager(this, true);

            GeneralConfig.Bind(0, Category.General, Config);
            FPVDroneConfig.Bind(1, Category.Drone, Config);
            FPVBindsConfig.Bind(2, Category.Binds, Config);
            ReconDroneConfig.Bind(3, Category.ReconDrone, Config);
            ReconBindsConfig.Bind(4, Category.ReconBinds, Config);
            PostProcessConfig.Bind(5, Category.PP, Config);

            AssetHelper.LoadBundles();
            AssetHelper.LoadAssets();
            
            patchManager.EnablePatches();

            BrainManager.AddCustomLayer(typeof(DroneCombatLayer),
                BotGlobals.AllBrainNames,
                9999 // wtf :waytoodank: TODO: rewrite ai stuff
            );

            int itemIndex = GClass3381.IndexOf(typeof(Item));
            GClass3381.List_0.Insert(itemIndex, typeof(DroneItem));
            GClass3381.List_0.Insert(itemIndex, typeof(PayloadItem));
            GClass3381.List_0.Insert(itemIndex, typeof(BatteryItem));

            TankDeathState = RouteHelper.FetchTankDeathState();
            
            CameraNearClip = Config.Bind("General", "Drone Camera Near Clip", 0.051f, "Changes camera near clip plane distance while piloting drone.");
        }
    }
}
#endif
