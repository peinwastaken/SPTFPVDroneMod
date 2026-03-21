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
            if (item is PayloadItem payloadItem)
            {
                Plugin.Logger.LogInfo($"Created {payloadItem.GetType()}! {item.StringTemplateId}");
                GameObject gameObject = await __result;
                
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
                }
            }
        }
    }
}
