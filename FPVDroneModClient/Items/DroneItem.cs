#if !UNITY_EDITOR
using EFT.InventoryLogic;
using FPVDroneModClient.Components;
using System;
using FPVDroneModClient.Globals;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Models;

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
    }
}
#endif
