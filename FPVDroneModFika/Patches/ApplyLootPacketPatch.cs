using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneModFika.Patches
{
    // TODO: this is a bandaid fix for nullref spam, items (drones) should be properly removed through proper fika channels
    public class ApplyLootPacketPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ObservedLootItem), nameof(ObservedLootItem.ApplyNetPacket));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ObservedLootItem __instance)
        {
            if (__instance.transform == null)
            {
                return false;
            }

            return true;
        }
    }
}
