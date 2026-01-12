 #if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DrakiaXYZ.BigBrain.Brains;
using EFT.InventoryLogic;
using FPVDroneModClient.Bots.Layers;
using FPVDroneModClient.Config;
using FPVDroneModClient.Globals;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Items;
using FPVDroneModClient.Patches;
using System.Collections.Generic;
using WTTClientCommonLib.Services;

namespace FPVDroneModClient
{
    [BepInPlugin("com.pein.fpvdronemod", "SPTFPVDroneMod", "0.4.0")]
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
            FPVDroneConfig.Bind(1, Category.Drone, Config);
            FPVBindsConfig.Bind(2, Category.Binds, Config);
            ReconDroneConfig.Bind(3, Category.ReconDrone, Config);
            ReconBindsConfig.Bind(4, Category.ReconBinds, Config);
            PostProcessConfig.Bind(5, Category.PP, Config);
            ExplosionConfig.Bind(6, Category.Explosion, Config);

            AssetHelper.LoadBundles();
            AssetHelper.LoadAssets();
            AssetHelper.LoadSounds();

            new InteractionPatch().Enable();
            new CameraPositionPatch().Enable();
            new SetCameraPatch().Enable();
            new GameStartedPatch().Enable();
            new WeaponInputPatch().Enable();
            new LootItemPhysicsPatch().Enable();
            new LocalPlayerDiedPatch().Enable();
            new ItemFactoryGetItemTypePatch().Enable();

            BrainManager.AddCustomLayer(typeof(DroneCombatLayer),
                BotGlobals.AllBrainNames,
                9999
            );
            
            List<TemplateIdToObjectType> mappings = [
                new TemplateIdToObjectType(
                    "6964ea3a5e4c1218314e1b2f",
                    typeof(DroneItem),
                    typeof(CompoundItemTemplateClass),
                    (string id, object tpl) => new DroneItem(id, (CompoundItemTemplateClass)tpl)
                )
            ];

            CustomTemplateIdToObjectService.AddNewTemplateIdToObjectMapping(mappings);

            CameraNearClip = Config.Bind("General", "Drone Camera Near Clip", 0.051f, "Changes camera near clip plane distance while piloting drone.");
        }
    }
}
#endif
