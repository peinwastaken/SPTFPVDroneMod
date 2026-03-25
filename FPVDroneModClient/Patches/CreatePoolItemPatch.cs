using EFT.InventoryLogic;
using FPVDroneModClient.Components.Base;
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
            return AccessTools.Method(typeof(PoolManagerClass), nameof(PoolManagerClass.CreateItemAsync));
        }

        [PatchPostfix]
        private static async void PatchPostfix(Item item, Task<GameObject> __result)
        {
            GameObject gameObject = await __result;
            if (!gameObject) return;
            
            if (item is PayloadItem payloadItem)
            {
                Plugin.Logger.LogInfo($"Created {payloadItem}! {item.StringTemplateId}");
                
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
                Plugin.Logger.LogInfo($"Created {droneItem}! {item.StringTemplateId}");
                
                BaseDroneController droneController = gameObject.GetComponentInChildren<BaseDroneController>();
                if (droneController)
                {
                    droneController.Item = item;
                }
            }
        }
    }
}
