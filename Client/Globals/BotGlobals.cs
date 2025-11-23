using System.Collections.Generic;

namespace FPVDroneMod.Globals
{
    public class BotGlobals
    {
        public static float DroneSightRange = 100f;
        public static float DroneHearRange = 50f;

        public Dictionary<EWeaponClass, float> DroneAttackDistances = new Dictionary<EWeaponClass, float>
        {
            { EWeaponClass.AssaultRifle, DroneSightRange },
            { EWeaponClass.AssaultCarbine, DroneSightRange },
            { EWeaponClass.Shotgun, DroneSightRange },
            { EWeaponClass.Pistol, DroneHearRange },
            { EWeaponClass.MachineGun, DroneHearRange },
            { EWeaponClass.SniperRifle, DroneHearRange },
            { EWeaponClass.SubMachineGun, DroneHearRange }
        };
    }
}
