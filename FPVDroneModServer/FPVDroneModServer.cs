using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils.Logger;
using System.Reflection;
using FPVDroneModServer.Services;
using WTTServerCommonLib.Services;
using Path = System.IO.Path;

namespace FPVDroneModServer
{
    [Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader + 2)]
    public class FPVDroneModServer(
        SptLogger<FPVDroneModServer> logger,
        WTTCustomItemServiceExtended itemService,
        WTTCustomSlotImageService imageService,
        WTTCustomLocaleService localeService,
        DatabaseService dbService,
        TankDeathService tankDeathService,
        ContainerHelper containerHelper,
        WTTCustomQuestService questService,
        WTTCustomQuestZoneService zoneService,
        WTTCustomLootspawnService lootService,
        WTTCustomHideoutRecipeService recipeService,
        WTTCustomAssortSchemeService assortService) : IOnLoad
    {
        public string AssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        public string ConfigPath => Path.Combine(AssemblyLocation, "config");
        
        public async Task OnLoad()
        {
            Dictionary<MongoId, TemplateItem> itemsDb = dbService.GetTables().Templates.Items;
            
            itemsDb["6964ea3a5e4c1218314e1b2f"] = new TemplateItem()
            {
                Id = "6964ea3a5e4c1218314e1b2f",
                Name = "DroneItem",
                Parent = "566162e44bdc2d3f298b4573",
                Type = "Node",
                Properties = new TemplateItemProperties()
            };

            itemsDb["69669ea64847b58fd5393f71"] = new TemplateItem()
            {
                Id = "69669ea64847b58fd5393f71",
                Name = "PayloadItem",
                Parent = "54009119af1c881c07000029",
                Type = "Node",
                Properties = new TemplateItemProperties()
            };
            
            itemsDb["69c932c7a7d59932499b5cde"] = new TemplateItem()
            {
                Id = "69c932c7a7d59932499b5cde",
                Name = "BatteryItem",
                Parent = "54009119af1c881c07000029",
                Type = "Node",
                Properties = new TemplateItemProperties()
            };
            
            tankDeathService.LoadTankStateConfig(ConfigPath, "tankdeathstate.json");
            imageService.CreateSlotImages(Assembly.GetExecutingAssembly(), "slots");
            await localeService.CreateCustomLocales(Assembly.GetExecutingAssembly(), "db/locales");
            await itemService.CreateCustomItems(Assembly.GetExecutingAssembly(), "db/items");
            await questService.CreateCustomQuests(Assembly.GetExecutingAssembly(), "db/quests");
            await zoneService.CreateCustomQuestZones(Assembly.GetExecutingAssembly(), "db/zones");
            await lootService.CreateCustomLootSpawns(Assembly.GetExecutingAssembly(), "db/loot");
            //await recipeService.CreateHideoutRecipes(Assembly.GetExecutingAssembly(), "db/recipes");
            //await assortService.CreateCustomAssortSchemes(Assembly.GetExecutingAssembly(), "db/assort");
            
            // add DroneItem and PayloadItem to item case
            containerHelper.AddToFilter("59fb042886f7746c5005a7b2", "6964ea3a5e4c1218314e1b2f");
            containerHelper.AddToFilter("59fb042886f7746c5005a7b2", "69669ea64847b58fd5393f71");
            
            // add DroneItem and PayloadItem to weapon case
            containerHelper.AddToFilter("59fb023c86f7746d0d4b423c", "6964ea3a5e4c1218314e1b2f");
            containerHelper.AddToFilter("59fb023c86f7746d0d4b423c", "69669ea64847b58fd5393f71");
            
            // add DroneItem and PayloadItem to thicc case
            containerHelper.AddToFilter("5c0a840b86f7742ffa4f2482", "6964ea3a5e4c1218314e1b2f");
            containerHelper.AddToFilter("5c0a840b86f7742ffa4f2482", "69669ea64847b58fd5393f71");
            
            // add DroneItem and PayloadItem to thicc weapon case
            containerHelper.AddToFilter("5b6d9ce188a4501afc1b2b25", "6964ea3a5e4c1218314e1b2f");
            containerHelper.AddToFilter("5b6d9ce188a4501afc1b2b25", "69669ea64847b58fd5393f71");
            
            logger.Success("Successfully loaded FPV Drone Mod! Don't blow yourself up.");
            
            await Task.CompletedTask;
        }
    }
}
