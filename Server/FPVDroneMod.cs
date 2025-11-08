using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Services.Mod;
using SPTarkov.Server.Core.Utils.Logger;
using System.Reflection;
using WTTServerCommonLib.Services;

namespace SPTFPVDroneServerMod
{
    [Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader)]
    public class FPVDroneMod(SptLogger<FPVDroneMod> logger, WTTCustomItemServiceExtended itemService) : IOnLoad
    {
        public Task OnLoad()
        {
            itemService.CreateCustomItems(Assembly.GetExecutingAssembly(), "db/items");
            
            return Task.CompletedTask;
        }
    }
}
