#if !UNITY_EDITOR
using EFT.InputSystem;
using EFT.InventoryLogic;
using FPVDroneMod.Globals;
using FPVDroneMod.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneMod.Patches
{
    public class WeaponInputPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Class1604), nameof(Class1604.TranslateCommand));
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
    
    public class KeyboardInputPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Class1602), nameof(Class1602.TranslateCommand));
        }

        [PatchPrefix]
        private static bool PatchPrefix()
        {
            return !DroneHelper.IsControllingDrone;
        }
    }
    
    public class MouseInputPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Class1602), nameof(Class1602.TranslateAxes));
        }

        [PatchPrefix]
        private static bool PatchPrefix()
        {
            return !DroneHelper.IsControllingDrone;
        }
    }
}
#endif