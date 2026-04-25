#if !UNITY_EDITOR
using EFT.InventoryLogic;
using FPVDroneModClient.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using WTTClientCommonLib.Attributes;

namespace FPVDroneModClient.Items
{
    public class BatteryItemTemplate(int capacity) : BarterItemTemplateClass
    {
        public int Capacity = capacity;
    }
    
    [CustomParent("69c932c7a7d59932499b5cde", typeof(BatteryItem), typeof(BatteryItemTemplate))]
    public class BatteryItem : SlotToggleableItem<BatteryItem>
    {
        [GAttribute26]
        [JsonProperty("Resource")]
        public readonly ResourceComponent ResourceComponent;

        public int Capacity;
        
        public BatteryItem(string id, BatteryItemTemplate template) : base(id, template)
        {
            Capacity = template.Capacity;
            Components.Add(ResourceComponent = new ResourceComponent(this, template));

            List<ItemAttributeClass> attributes = FilterAttributes(Attributes,
                EItemAttributeId.Resource
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

        private List<ItemAttributeClass> FilterAttributes(List<ItemAttributeClass> list, params EItemAttributeId[] attributes)
        {
            List<ItemAttributeClass> retainedAttributes = [];
            List<EItemAttributeId> attributesList = attributes.ToList();
            
            foreach (var attribute in list)
            {
                System.Enum id = attribute.Id;
                
                if (id is EItemAttributeId attributeId && attributesList.Contains(attributeId))
                {
                    retainedAttributes.Add(attribute);
                }
            }

            return retainedAttributes;
        }
    }
}
#endif
