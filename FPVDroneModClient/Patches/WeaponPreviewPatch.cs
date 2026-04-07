using EFT.UI.WeaponModding;
using FPVDroneModClient.Components;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace FPVDroneModClient.Patches
{
    public class WeaponPreviewPatch : ModulePatch
    {
        private static int _depth = 5;
        
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(WeaponPreview), nameof(WeaponPreview.method_5));
        }

        [PatchPostfix]
        private static void PatchPostfix(GameObject itemGameObject)
        {
            if (itemGameObject == null) return;
            SlotVisibilityToggler[] togglers = itemGameObject.GetComponentsInChildren<SlotVisibilityToggler>();

            foreach (var toggler in togglers)
            {
                if (toggler == null) continue;
                Transform currentParent = toggler.transform.parent;
                bool modFound = false;
                
                for (int i = 0; i < _depth; i++)
                {
                    if (currentParent != null)
                    {
                        if (currentParent.transform.name.StartsWith("mod_"))
                        {
                            modFound = true;
                            break;
                        }
                    
                        currentParent = currentParent.parent;
                    }
                }

                if (!modFound)
                {
                    toggler.OnUnequip();
                }
            }
        }
    }
}
