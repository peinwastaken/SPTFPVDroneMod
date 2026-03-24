using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;

namespace FPVDroneModServer
{
    [Injectable]
    public class ContainerHelper(DatabaseService dbService)
    {
        public void AddToFilter(MongoId containerId, MongoId parentId)
        {
            Dictionary<MongoId, TemplateItem> items = dbService.GetItems();

            items.TryGetValue(containerId, out TemplateItem? container);

            if (container != null)
            {
                container.Properties?.Grids?.FirstOrDefault()?.Properties?.Filters?.FirstOrDefault()?.Filter?.Add(parentId);
            }
        }
    }
}
