#if !UNITY_EDITOR
using EFT;
using EFT.InputSystem;
using EFT.InventoryLogic;
using FPVDroneModClient.Globals;
using FPVDroneModClient.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneModClient.Patches
{
    public class WeaponInputPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(FirearmHandsInputTranslator), nameof(FirearmHandsInputTranslator.TranslateCommand));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ECommand command)
        {
            Weapon equippedWeapon = PlayerHelper.GetEquippedWeapon();

            if (equippedWeapon?.StringTemplateId == ItemIds.ControllerTemplateId)
            {
                if (command == ECommand.ToggleShooting)
                {
                    DroneHelper.ControlDrone(true);
                }

                if (command == ECommand.ToggleAlternativeShooting)
                {
                    DroneHelper.ShowSelectedDrones();
                }
                
                return false;
            }
            
            return !DroneHelper.IsControllingDrone;
        }
    }
}
#endif