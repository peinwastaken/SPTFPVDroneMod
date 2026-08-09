using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using System.Reflection;
using FPVDroneModServer.Services;
using SPTarkov.Common.Logger;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Spt.Config;
using WTTServerCommonLib.Services;
using Path = System.IO.Path;

namespace FPVDroneModServer
{
    [Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.Preload + 1)]
    public class FPVDroneModServer(
        SptLogger<FPVDroneModServer> logger,
        TankDeathService tankDeathService,
        WTTCustomQuestService questService,
        WTTCustomQuestZoneService zoneService,
        WTTCustomLootspawnService lootService,
        WTTCustomAssortSchemeService assortService,
        WTTCustomItemServiceExtended itemService,
        WTTCustomSlotImageService imageService,
        WTTCustomLocaleService localeService,
        WTTCustomItemParentService parentService,
        TraderConfig traderConfig) : IOnLoad
    {
        public string AssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        public string ConfigPath => Path.Combine(AssemblyLocation, "config");

        public async Task OnLoadAsync(CancellationToken cancellationToken)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            tankDeathService.LoadTankStateConfig(ConfigPath, "tankdeathstate.json");
            imageService.CreateSlotImages(assembly, "slots");
            await parentService.CreateCustomParents(assembly, "db/parents");
            await localeService.CreateCustomLocales(assembly, "db/locales");
            await itemService.CreateCustomItems(assembly, "db/items");
            await questService.CreateCustomQuests(assembly, "db/quests");
            await zoneService.CreateCustomQuestZones(assembly, "db/zones");
            await lootService.CreateCustomLootSpawns(assembly, "db/loot");
            await assortService.CreateCustomAssortSchemes(assembly, "db/assort");
            //await recipeService.CreateHideoutRecipes(Assembly.GetExecutingAssembly(), "db/recipes");
            
            // set stack size of payloads for fence to prevent error
            traderConfig.Fence.ItemStackSizeOverrideMinMax["69669ea64847b58fd5393f71"] = new MinMax<int>(1, 1);
            
            logger.Success("Successfully loaded FPV Drone Mod! Don't blow yourself up.");
            await Task.CompletedTask;
        }
    }
}
