#if !UNITY_EDITOR
using EFT.InventoryLogic;
using FPVDroneModClient.Components;
using FPVDroneModClient.Interface;
using UnityEngine;

namespace FPVDroneModClient.Items
{
    public class BatteryItem(string id, ItemTemplate template) : SlotToggleableItem(id, template)
    {
        
    }
}
#endif
