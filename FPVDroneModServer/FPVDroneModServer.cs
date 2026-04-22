using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils.Logger;
using System.Reflection;
using FPVDroneModServer.Services;
using SPTarkov.Server.Core.Services.Mod;
using WTTServerCommonLib.Helpers;
using WTTServerCommonLib.Services;
using Path = System.IO.Path;

namespace FPVDroneModServer
{
    [Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader + 2)]
    public class FPVDroneModServer(
        SptLogger<FPVDroneModServer> logger,
        DatabaseService dbService,
        TankDeathService tankDeathService,
        ContainerHelper containerHelper,
        ItemBaseClassService itemBaseClassService,
        WTTCustomQuestService questService,
        WTTCustomQuestZoneService zoneService,
        WTTCustomLootspawnService lootService,
        WTTCustomHideoutRecipeService recipeService,
        WTTCustomAssortSchemeService assortService,
        WTTCustomItemServiceExtended itemService,
        WTTCustomSlotImageService imageService,
        WTTCustomLocaleService localeService,
        ConfigHelper configHelper) : IOnLoad
    {
        public string AssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        public string ConfigPath => Path.Combine(AssemblyLocation, "config");
        public string ParentsPath => Path.Combine(AssemblyLocation, "parents");
        
        public async Task OnLoad()
        {
            tankDeathService.LoadTankStateConfig(ConfigPath, "tankdeathstate.json");
            imageService.CreateSlotImages(Assembly.GetExecutingAssembly(), "slots");
            await LoadCustomParents();
            await localeService.CreateCustomLocales(Assembly.GetExecutingAssembly(), "db/locales");
            await itemService.CreateCustomItems(Assembly.GetExecutingAssembly(), "db/items");
            await questService.CreateCustomQuests(Assembly.GetExecutingAssembly(), "db/quests");
            await zoneService.CreateCustomQuestZones(Assembly.GetExecutingAssembly(), "db/zones");
            await lootService.CreateCustomLootSpawns(Assembly.GetExecutingAssembly(), "db/loot");
            await assortService.CreateCustomAssortSchemes(Assembly.GetExecutingAssembly(), "db/assort");
            //await recipeService.CreateHideoutRecipes(Assembly.GetExecutingAssembly(), "db/recipes");
            
            // add DroneItem, PayloadItem and BatteryItem to item case
            containerHelper.AddToFilter("59fb042886f7746c5005a7b2", "6964ea3a5e4c1218314e1b2f");
            containerHelper.AddToFilter("59fb042886f7746c5005a7b2", "69669ea64847b58fd5393f71");
            containerHelper.AddToFilter("59fb042886f7746c5005a7b2", "69c932c7a7d59932499b5cde");
            
            // add DroneItem and PayloadItem to weapon case
            containerHelper.AddToFilter("59fb023c86f7746d0d4b423c", "6964ea3a5e4c1218314e1b2f");
            containerHelper.AddToFilter("59fb023c86f7746d0d4b423c", "69669ea64847b58fd5393f71");
            
            // add DroneItem, PayloadItem and BatteryItem to thicc case
            containerHelper.AddToFilter("5c0a840b86f7742ffa4f2482", "6964ea3a5e4c1218314e1b2f");
            containerHelper.AddToFilter("5c0a840b86f7742ffa4f2482", "69669ea64847b58fd5393f71");
            containerHelper.AddToFilter("5c0a840b86f7742ffa4f2482", "69c932c7a7d59932499b5cde");
            
            // add DroneItem and PayloadItem to thicc weapon case
            containerHelper.AddToFilter("5b6d9ce188a4501afc1b2b25", "6964ea3a5e4c1218314e1b2f");
            containerHelper.AddToFilter("5b6d9ce188a4501afc1b2b25", "69669ea64847b58fd5393f71");
            
            // add BatteryItem to scav box
            containerHelper.AddToFilter("5b7c710788a4506dec015957", "69c932c7a7d59932499b5cde");
            
            logger.Success("Successfully loaded FPV Drone Mod! Don't blow yourself up.");
            
            await Task.CompletedTask;
        }

        private async Task LoadCustomParents()
        {
            Dictionary<MongoId, TemplateItem> items = dbService.GetTables().Templates.Items;
            string[] parentJsons = Directory.GetFiles(ParentsPath, "*.json*", SearchOption.AllDirectories);

            foreach (string filePath in parentJsons)
            {
                var parentsDict = await configHelper.LoadJsonFileFlexible<Dictionary<string, TemplateItem>>(filePath);

                foreach (var parents in parentsDict)
                {
                    foreach (var parent in parents)
                    {
                        MongoId id = parent.Key;
                        TemplateItem tpl = parent.Value;
                        
                        items[id] = tpl;
                        itemBaseClassService.AddItemToCache(id);
                    }
                }
            }
            
            //sptItemService
        } 
    }
}
