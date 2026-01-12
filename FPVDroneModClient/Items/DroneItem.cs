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

        public bool HasThermalModule()
        {
            return Slots[0]?.ContainedItem?.StringTemplateId == "5d0377ce86f774186372f689";
        }
        
        public bool HasNightVisionModule()
        {
            return Slots[0]?.ContainedItem?.StringTemplateId == "696504ca8ce4c9b2404e1b32";
        }
    }
}
