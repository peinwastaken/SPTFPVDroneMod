#if !UNITY_EDITOR
using System.Collections.Generic;

namespace FPVDroneMod.Globals
{
    public static class BotGlobals
    {
        public static float DroneSightRange = 100f;
        public static float DroneHearRange = 50f;

        public static List<string> AllBrainNames = new List<string>
        {
            "PMC",
            "PmcBear",
            "PmcUsec",
            "Assault",
            "Marksman",
            "BossBully",
            "FollowerBully",
            "ExUsec",
            "Knight",
            "FollowerKojaniy",
            "ArenaFighter",
            "BossBoar",
            "BoarSniper",
            "KillaAgro",
            "BossPartisan",
            "BossSanitar",
            "TagillaAgro",
            "Tagilla",
            "BossZryachiy",
            "CursAssault",
            "Obdolbs",
            "BigPipe",
            "BirdEye",
            "FollowerBully",
            "FollowerSanitar",
            "TagillaFollower",
            "Fl_Zraychiy",
            "Gifter",
            "BossGluhar",
            "FollowerGluharAssault",
            "FollowerGluharProtect",
            "FollowerGluharScout",
            "Killa",
            "BossKolontay",
            "FlKlnAslt",
            "KolonSec",
            "PeaceZryachiy",
            "BossKojaniy",
            "SectantWarrior",
            "SctPredvst",
            "SectantPriest",
            "PrizrakSt",
            "HelperAgro"
        };

        public static Dictionary<EWeaponClass, float> DroneAttackDistances = new Dictionary<EWeaponClass, float>
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
#endif
