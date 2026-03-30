#if !UNITY_EDITOR
using EFT.InventoryLogic;
using FPVDroneModClient.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FPVDroneModClient.Items
{
    public class BatteryItemTemplate(int capacity) : BarterItemTemplateClass
    {
        public int Capacity = capacity;
    }
    
    public class BatteryItem : SlotToggleableItem
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
