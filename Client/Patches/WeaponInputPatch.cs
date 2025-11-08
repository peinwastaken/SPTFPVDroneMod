#if !UNITY_EDITOR
using EFT.InputSystem;
using EFT.InventoryLogic;
using FPVDroneMod.Globals;
using FPVDroneMod.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using WeaponInputClass = Class1730;

namespace FPVDroneMod.Patches
{
    public class WeaponInputPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(WeaponInputClass), nameof(WeaponInputClass.TranslateCommand));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ECommand command)
        {
            Weapon equippedWeapon = PlayerHelper.GetEquippedWeapon();
            
            if (command == ECommand.ToggleShooting && equippedWeapon?.StringTemplateId == ItemIds.ControllerTemplateId)
            {
                DroneHelper.ControlDrone(true);
                return false;
            }
            
            return !DroneHelper.IsControllingDrone;
        }
    }
}
#endif