using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils.Logger;
using System.Reflection;
using WTTServerCommonLib.Services;

namespace FPVDroneModServer
{
    [Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader + 2)]
    public class FPVDroneModServer(SptLogger<FPVDroneModServer> logger, WTTCustomItemServiceExtended itemService, DatabaseService dbService) : IOnLoad
    {
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
            
            await itemService.CreateCustomItems(Assembly.GetExecutingAssembly(), "db/items");
            
            logger.Success("Successfully loaded FPV Drone Mod! Don't blow yourself up.");
            
            await Task.CompletedTask;
        }
    }
}
