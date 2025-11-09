using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Utils.Logger;
using System.Reflection;
using WTTServerCommonLib.Services;

namespace FPVDroneServerMod
{
    [Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader + 2)]
    public class FPVDroneMod(SptLogger<FPVDroneMod> logger, WTTCustomItemServiceExtended itemService) : IOnLoad
    {
        public async Task OnLoad()
        {
            await itemService.CreateCustomItems(Assembly.GetExecutingAssembly(), "db/items");
            
            logger.Success("Successfully loaded FPV Drone Mod! Don't blow yourself up.");
        }
    }
}
