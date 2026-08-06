using EFT;
using EFT.InventoryLogic;
using FPVDroneModClient.Components;
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Interface;
using FPVDroneModClient.Items;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace FPVDroneModClient.Patches
{
    public class CreatePoolItemPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ObjectsFactory), nameof(ObjectsFactory.CreateItemAsync));
        }

        [PatchPostfix]
        private static async void PatchPostfix(Item item, Task<GameObject> __result)
        {
            GameObject gameObject = await __result;
            if (!gameObject) return;
            
            if (item is PayloadItem payloadItem)
            {
                DebugLogger.LogInfo($"Created {payloadItem.Name}! {item.StringTemplateId}");
                
                BasePayloadController payloadController = gameObject.GetComponentInChildren<BasePayloadController>();
                if (payloadController)
                {
                    payloadController.Damage = payloadItem.Damage;
                    payloadController.IsAntiTank = payloadItem.IsAntiTank;
                    payloadController.FractureDelta = payloadItem.FractureDelta;
                    payloadController.MaxDistance = payloadItem.MaxDistance;
                    payloadController.HeavyBleedDelta = payloadItem.HeavyBleedDelta;
                    payloadController.LightBleedDelta = payloadItem.LightBleedDelta;
                    payloadController.StaminaBurnRate = payloadItem.StaminaBurnRate;
                    payloadController.Item = item;
                }
            }
            else if (item is DroneItem droneItem)
            {
                DebugLogger.LogInfo($"Created {droneItem.Name}! {item.StringTemplateId}");
                
                BaseDroneController droneController = gameObject.GetComponentInChildren<BaseDroneController>();
                if (droneController)
                {
                    droneController.Item = droneItem;
                }
            }
            else if (item is BatteryItem batteryItem)
            {
                DebugLogger.LogInfo($"Created {batteryItem.Name}! {item.StringTemplateId}");
            }

            if (item is BaseSlotToggleable toggleableItem)
            {
                SlotVisibilityToggler toggler = gameObject.GetComponentInChildren<SlotVisibilityToggler>();
                if (toggler)
                {
                    toggleableItem.SlotToggleController = toggler;
                    toggleableItem.Item = item;
                    
                    bool equipped = item.CurrentAddress is Slot.ProtectedSlotItemAddress;
                    toggleableItem.OnItemEquipped(equipped);
                }
            }
        }
    }
}

