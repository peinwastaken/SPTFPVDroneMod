#if !UNITY_EDITOR
using Comfort.Common;
using EFT;
using FPVDroneModClient.Models;
using System.Collections.Generic;
using Systems.Effects;
using UnityEngine;
using UnityEngine.UIElements;

namespace FPVDroneModClient.Helpers
{
    // TODO: convert payloads to be ammunition instead and use vanilla explosion systems
    public static class ExplosionHelper
    {
        private static Collider[] _colliders = new Collider[64];
        
        public static void CreateExplosion(ExplosionData explosion, bool emitParticles = true)
        {
            Dictionary<Player, PlayerExplosionData> affectedPlayers = [];

            if (emitParticles)
            {
                Singleton<Effects>.Instance.EmitGrenade(explosion.EffectName, explosion.Position, explosion.EffectDirection, 1f);
            }
            
            int size = Physics.OverlapSphereNonAlloc(explosion.Position, explosion.MaxDistance, _colliders, LayerMaskClass.HitColliderMask);
            
            DebugLogger.LogInfo($"collider hits: {size}");
            
            // grab all colliders and players
            for (int i = 0; i < size; i++)
            {
                Collider collider = _colliders[i];
                if (!collider) continue;
                
                BodyPartCollider bodyPartCollider = collider.GetComponent<BodyPartCollider>();
                if (!bodyPartCollider) continue;

                Player player = (Player)bodyPartCollider.Player;
                if (!player) continue;

                if (!affectedPlayers.ContainsKey(player))
                {
                    affectedPlayers.Add(player, new PlayerExplosionData());
                }

                PlayerExplosionData data = affectedPlayers[player];

                if (!data.ProcessedLimbs.Contains(bodyPartCollider.BodyPartType) &&
                    !data.BodyPartColliders.ContainsKey(bodyPartCollider))
                {
                    affectedPlayers[player].BodyPartColliders.Add(
                        bodyPartCollider,
                        Vector3.Distance(bodyPartCollider.transform.position, explosion.Position)
                    );

                    affectedPlayers[player].ProcessedLimbs.Add(bodyPartCollider.BodyPartType);
                }
            }

            DebugLogger.LogInfo($"players in range: {affectedPlayers.Count}");
            
            // apply screen effects to affected players
            foreach (KeyValuePair<Player, PlayerExplosionData> kvp in affectedPlayers)
            {
                Player player = kvp.Key;
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

                    bool isVisible = VectorHelper.VisCheck(explosion.Position, collider.transform.position, LayerMaskClass.HighPolyWithTerrainNoGrassMask);
                    if (!isVisible) continue;

                    float colliderDistance = Vector3.Distance(collider.transform.position, explosion.Position);
                    float colliderDistanceMultiplier = 1f - Mathf.Clamp01(colliderDistance / explosion.MaxDistance);
                    Vector3 directionFromExplosion = Vector3.Normalize(collider.transform.position - explosion.Position);
                    float finalDamage = explosion.Damage * Mathf.Pow(colliderDistanceMultiplier, 3f);
                
                    DamageInfoStruct damageInfo = new DamageInfoStruct
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

                    player.ApplyDamageInfo(damageInfo, bodyPart, colliderType, 0f);
                    
                    DebugLogger.LogInfo($"applied damage to: {player.name} | damage:{damageInfo.Damage}");
                }
            }
        }
    }
}
#endif
