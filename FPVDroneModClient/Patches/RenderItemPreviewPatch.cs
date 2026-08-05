#if !UNITY_EDITOR
using EFT.InventoryLogic;
using FPVDroneModClient.Components;
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
using UnityEngine;

namespace FPVDroneModClient.Patches
{
    public class RenderItemPreviewPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            Type generic = typeof(IconCreatorBase<,>).MakeGenericType(typeof(Item), typeof(IResourceIcon));
            return AccessTools.Method(generic, nameof(IconCreatorBase<,>.CaptureSpriteOfModel));
        }

        [PatchPrefix]
        private static void PatchPrefix(GameObject model)
        {
            try
            {
                BaseDroneController droneController = model.GetComponentInChildren<BaseDroneController>();
                SlotVisibilityToggler[] togglers = model.GetComponentsInChildren<SlotVisibilityToggler>();

                if (droneController)
                {
                    for (int i = 0; i < togglers.Length; i++)
                    {
                        SlotVisibilityToggler toggler = togglers[i];
                        toggler?.OnEquip();
                    }
                }
                else
                {
                    if (togglers.Length > 0 && togglers[0] != null)
                    {
                        togglers[0].OnUnequip();
                    }
                }
            }
            catch (Exception e)
            {
                DebugLogger.LogError(e.Message);
                throw;
            }
        }
    }
}
#endif
