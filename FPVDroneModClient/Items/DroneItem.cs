using EFT.InventoryLogic;
using System;
using FPVDroneModClient.Globals;
using FPVDroneModClient.Helpers;
using WTTClientCommonLib.Attributes;

namespace FPVDroneModClient.Items
{
    [CustomParent("6964ea3a5e4c1218314e1b2f", typeof(DroneItem), typeof(CompoundItemTemplate))]
    public class DroneItem : CompoundItem
    {
        public DroneItem(string id, CompoundItemTemplate template) : base(id, template)
        {
            Slots = Array.ConvertAll(template.Slots, CG_Ctor);

            var compatibleAttribute = Attributes.FindAttribute(EItemAttributeId.CompatibleWith);

            Attributes.Remove(compatibleAttribute);
        }

        public bool HasThermalModule()
        {
            return this.GetSlotById(SlotIds.GearSlot)?.ContainedItem?.StringTemplateId == ItemIds.Iridium;
        }
        
        public bool HasNightVisionModule()
        {
            return this.GetSlotById(SlotIds.GearSlot)?.ContainedItem?.StringTemplateId == ItemIds.SovaDevice;
        }

        public bool HasPayload()
        {
            return this.GetSlotById(SlotIds.PayloadSlot)?.ContainedItem != null;
        }

        public Slot GetPayloadSlot()
        {
            foreach (Slot slot in Slots)
            {
                if (slot.ID == SlotIds.PayloadSlot)
                {
                    return slot;
                }
            }

            return null;
        }

        public PayloadItem GetPayload()
        {
            Slot payloadSlot = GetPayloadSlot();

            return (PayloadItem)payloadSlot?.ContainedItem;
        }

        public Slot GetBatterySlot()
        {
            foreach (Slot slot in Slots)
            {
                if (slot.ID == SlotIds.BatterySlot)
                {
                    return slot;
                }
            }

            return null;
        }
    }
}

