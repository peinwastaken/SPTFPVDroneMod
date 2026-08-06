using EFT.InventoryLogic;
using FPVDroneModClient.Globals;

namespace FPVDroneModClient.Helpers
{
    public static class WeaponHelper
    {
        public static EWeaponClass GetWeaponClass(this WeaponTemplate template)
        {
            return template.weapClass.ToLower() switch
            {
                "assaultrifle" => EWeaponClass.AssaultRifle,
                "assaultcarbine" => EWeaponClass.AssaultCarbine,
                "pistol" => EWeaponClass.Pistol,
                "shotgun" => EWeaponClass.Shotgun,
                "sniperrifle" => EWeaponClass.SniperRifle,
                "machinegun" => EWeaponClass.MachineGun,
                "smg" => EWeaponClass.SubMachineGun,
                "marksmanrifle" => EWeaponClass.MarksmanRifle,
                "grenadelauncher" => EWeaponClass.GrenadeLauncher,
                "specialWeapon" => EWeaponClass.SpecialWeapon,
                _ => EWeaponClass.None // hopefully this never happens
            };
        }
    }
}

