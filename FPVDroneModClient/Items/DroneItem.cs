using EFT.InventoryLogic;
using System;

namespace FPVDroneModClient.Items
{
    public class DroneItem : CompoundItem
    {
        public DroneItem(string id, CompoundItemTemplateClass template) : base(id, template)
        {
            Slots = Array.ConvertAll(template.Slots, method_7);
        }
    }
}
