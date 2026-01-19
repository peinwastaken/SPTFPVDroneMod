 #if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DrakiaXYZ.BigBrain.Brains;
using EFT.CameraControl;
using EFT.InventoryLogic;
using FPVDroneModClient.Bots.Layers;
using FPVDroneModClient.Config;
using FPVDroneModClient.Globals;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Items;
using FPVDroneModClient.Patches;
using System.Collections.Generic;
using FPVDroneModClient.Models;
using SPT.Custom.Models;
using SPT.Custom.Utils;
using UnityEngine;
using WTTClientCommonLib.Services;

namespace FPVDroneModClient
{
    [BepInPlugin("com.pein.fpvdronemod", "SPTFPVDroneMod", "0.4.0")]
    [BepInDependency("xyz.drakia.bigbrain")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        public static TankDeathState TankDeathState;
        public static ConfigEntry<float> CameraNearClip;
        
        private void Awake()
        {
            Logger = base.Logger;
            DebugLogger.Logger = Logger;

            GeneralConfig.Bind(0, Category.General, Config);
            FPVDroneConfig.Bind(1, Category.Drone, Config);
            FPVBindsConfig.Bind(2, Category.Binds, Config);
            ReconDroneConfig.Bind(3, Category.ReconDrone, Config);
            ReconBindsConfig.Bind(4, Category.ReconBinds, Config);
            PostProcessConfig.Bind(5, Category.PP, Config);
            ExplosionConfig.Bind(6, Category.Explosion, Config);

            AssetHelper.LoadBundles();
            AssetHelper.LoadAssets();

            new InteractionPatch().Enable();
            new CameraPositionPatch().Enable();
            new SetCameraPatch().Enable();
            new GameStartedPatch().Enable();
            new WeaponInputPatch().Enable();
            new LootItemPhysicsPatch().Enable();
            new LocalPlayerDiedPatch().Enable();
            new ItemFactoryGetItemTypePatch().Enable();
            new SpawnBtrPatch().Enable();
            new GetIndexOfItemTypePatch().Enable();

            BrainManager.AddCustomLayer(typeof(DroneCombatLayer),
                BotGlobals.AllBrainNames,
                9999
            );

            List<TemplateIdToObjectType> mappings =
            [
                new TemplateIdToObjectType(
                    "6964ea3a5e4c1218314e1b2f",
                    typeof(DroneItem),
                    typeof(CompoundItemTemplateClass),
                    (string id, object tpl) => new DroneItem(id, (CompoundItemTemplateClass)tpl)
                ),
                new TemplateIdToObjectType(
                    "69669ea64847b58fd5393f71",
                    typeof(PayloadItem),
                    typeof(PayloadItemTemplate),
                    (string id, object tpl) => new PayloadItem(id, (PayloadItemTemplate)tpl))
            ];

            CustomTemplateIdToObjectService.AddNewTemplateIdToObjectMapping(mappings);

            int itemIndex = GClass3381.IndexOf(typeof(Item));
            GClass3381.List_0.Insert(itemIndex, typeof(DroneItem));
            GClass3381.List_0.Insert(itemIndex, typeof(PayloadItem));

            TankDeathState = RouteHelper.FetchTankDeathState();
            
            CameraNearClip = Config.Bind("General", "Drone Camera Near Clip", 0.051f, "Changes camera near clip plane distance while piloting drone.");
        }
    }
}
#endif
