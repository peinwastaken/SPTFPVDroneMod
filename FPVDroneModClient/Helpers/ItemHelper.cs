#if !UNITY_EDITOR
using EFT.InventoryLogic;
using System.Collections.Generic;

namespace FPVDroneModClient.Helpers
{
    public static class ItemHelper
    {
        public static Slot GetSlotById(this CompoundItem item, string slotId)
        {
            foreach (Slot slot in item.Slots)
            {
                if (slot.ID == slotId)
                {
                    return slot;
                }
            }

            return null;
        }
        
        public static List<ItemAttributeClass> GetFilteredAttributes(this List<ItemAttributeClass> list, params EItemAttributeId[] attributes)
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
        
        public static ItemAttributeClass FindAttribute(this List<ItemAttributeClass> list, EItemAttributeId attributeToFind)
        {
            foreach (var attribute in list)
            {
                System.Enum id = attribute.Id;
                
                if (id is EItemAttributeId attributeId && attributeId == attributeToFind)
                {
                    return attribute;
                }
            }

            return null;
        }
    }
}
#endif