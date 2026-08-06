using EFT;
using EFT.InventoryLogic;
using FPVDroneModClient.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using WTTClientCommonLib.Attributes;

namespace FPVDroneModClient.Items
{
    public class BatteryItemTemplate(int capacity) : BarterItemTemplate
    {
        public int Capacity = capacity;
    }
    
    [CustomParent("69c932c7a7d59932499b5cde", typeof(BatteryItem), typeof(BatteryItemTemplate))]
    public class BatteryItem : SlotToggleableItem<BatteryItem>
    {
        [Component]
        [JsonProperty("Resource")]
        public readonly ResourceComponent ResourceComponent;

        public int Capacity;
        
        public BatteryItem(string id, BatteryItemTemplate template) : base(id, template)
        {
            Capacity = template.Capacity;
            Components.Add(ResourceComponent = new ResourceComponent(this, template));

            List<ItemAttribute> attributes = Attributes.GetFilteredAttributes(
                EItemAttributeId.Resource,
                EItemAttributeId.LimitedDiscard
            );

            attributes.AddRange([
                new(EItemAttributeId.ContainerSize)
                {
                    Name = "DRONE BATTERY CAPACITY",
                    Base = () => Capacity,
                    StringValue = () => $"{Capacity} {"AMPERE HOURS UNIT".Localized()}",
                    DisplayType = () => EItemAttributeDisplayType.Compact,
                }
            ]);
            
            Attributes = attributes;
        }
    }
}

