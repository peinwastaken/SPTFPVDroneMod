using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using System.Reflection;
using FPVDroneModServer.Services;
using SPTarkov.Common.Logger;
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
        WTTCustomHideoutRecipeService recipeService,
        WTTCustomAssortSchemeService assortService,
        WTTCustomItemServiceExtended itemService,
        WTTCustomSlotImageService imageService,
        WTTCustomLocaleService localeService,
        WTTCustomItemParentService parentService) : IOnLoad
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
            
            logger.Success("Successfully loaded FPV Drone Mod! Don't blow yourself up.");
            
            await Task.CompletedTask;
        }
    }
}
