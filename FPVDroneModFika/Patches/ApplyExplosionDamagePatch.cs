using Comfort.Common;
using EFT;
using EFT.Ballistics;
using Fika.Core.Main.Players;
using Fika.Core.Main.Utils;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib;
using Fika.Core.Networking.Packets.Player.Common;
using Fika.Core.Networking.Packets.Player.Common.SubPackets;
using FlyingWormConsole3.LiteNetLib;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Models;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FPVDroneModFika.Patches
{
    // thx fika Minefield_method_2_Patch.cs :^)
    public class ApplyExplosionDamagePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ExplosionHelper), "ApplyDamageToAffectedPlayers");
        }

        [PatchPrefix]
        private static bool PatchPrefix(Dictionary<Player, PlayerExplosionData> affectedPlayers, ExplosionData explosion)
        {
            // only apply damage if we are the server
            if (FikaBackendUtils.IsServer)
            {
                ReplicateDamageToAffectedPlayers(affectedPlayers, explosion);
                
                return false;
            }

            return true;
        }

        private static void ReplicateDamageToAffectedPlayers(Dictionary<Player, PlayerExplosionData> affectedPlayers, ExplosionData explosion)
        {
            foreach (KeyValuePair<Player, PlayerExplosionData> kvp in affectedPlayers)
            {
                Player player = kvp.Key;
                FikaPlayer fikaPlayer = (FikaPlayer)Singleton<GameWorld>.Instance.GetAlivePlayerByProfileID(player.ProfileId);
                PlayerExplosionData info = kvp.Value;
                
                float distanceFromExplosion = Vector3.Distance(player.Position, explosion.Position);
                Vector3 dirFromExplosion = Vector3.Normalize(player.Position - explosion.Position);
                float playerDistanceMultiplier = Mathf.Clamp01(distanceFromExplosion / explosion.MaxDistance);

                if (explosion.InstantKillDistance > 0 && distanceFromExplosion < explosion.InstantKillDistance)
                {
                    player.ActiveHealthController.Kill(EDamageType.Explosion);
                }
                
                player.ActiveHealthController?.DoContusion(20f * playerDistanceMultiplier, playerDistanceMultiplier);
                player.ActiveHealthController?.DoDisorientation(5f * playerDistanceMultiplier);
                player.ProceduralWeaponAnimation?.ForceReact?.AddForce(dirFromExplosion, playerDistanceMultiplier, 1f, 2f);

                if (Random.Range(0f, 1f) < explosion.FractureDelta * playerDistanceMultiplier)
                {
                    EBodyPart closestBodyPart = affectedPlayers[player].GetClosestFracturableBodyPart();

                    player.ActiveHealthController?.DoFracture(closestBodyPart);
                }

                foreach (BodyPartCollider collider in info.BodyPartColliders.Keys)
                {
                    EBodyPart bodyPart = collider.BodyPartType;
                    EBodyPartColliderType colliderType = collider.BodyPartColliderType;

                    bool isVisible = VectorHelper.VisCheck(explosion.Position, collider.transform.position, LayersMaskController.HighPolyWithTerrainNoGrassMask);
                    if (!isVisible) continue;
                    
                    float colliderDistance = Vector3.Distance(collider.transform.position, explosion.Position);
                    float colliderDistanceMultiplier = 1f - Mathf.Clamp01(colliderDistance / explosion.MaxDistance);
                    Vector3 directionFromExplosion = Vector3.Normalize(collider.transform.position - explosion.Position);
                    float finalDamage = explosion.Damage * Mathf.Pow(colliderDistanceMultiplier, 3f);
                
                    DamageInfo damageInfo = new DamageInfo
                    {
                        DamageType = EDamageType.Explosion,
                        Damage = finalDamage,
                        ArmorDamage = 0.35f,
                        PenetrationPower = 25,
                        Direction = directionFromExplosion,
                        HitNormal = -directionFromExplosion,
                        HitPoint = collider.transform.position,
                        Player = explosion.PlayerOwner,
                        Weapon = explosion.Weapon,
                        HeavyBleedingDelta = explosion.HeavyBleedDelta,
                        LightBleedingDelta = explosion.LightBleedDelta,
                        StaminaBurnRate = explosion.StaminaBurnRate
                    };

                    if (fikaPlayer is ObservedPlayer)
                    {
                        fikaPlayer.CommonPacket.Type = ECommonSubPacketType.Damage;
                        fikaPlayer.CommonPacket.SubPacket = DamagePacket.FromValue(fikaPlayer.NetId, damageInfo, bodyPart, colliderType);
                        Singleton<IFikaNetworkManager>.Instance.SendNetReusable(ref fikaPlayer.CommonPacket, DeliveryMethod.ReliableOrdered, true);
                    }
                    else
                    {
                        player.ApplyDamageInfo(damageInfo, bodyPart, colliderType, 0f);
                    }
                    
                    DebugLogger.LogInfo($"applied damage to: {player.name} | damage: {damageInfo.Damage}");
                }
            }
        }
    }
}
