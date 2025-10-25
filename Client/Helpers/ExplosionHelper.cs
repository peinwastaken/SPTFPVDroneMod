using System.Collections.Generic;
using UnityEngine;
using EFT;
using Comfort.Common;
using Systems.Effects;

namespace FPVDroneMod.Helpers
{
    public class ExplosionData
    {
        public Vector3 Position = Vector3.zero;
        public float MaxDistance = 10f;
        public float Damage = 200f;
        public string EffectName = "Grenade_new";
        public float FractureDelta = 0.6f;
        public float HeavyBleedDelta = 0.4f;
        public float LightBleedDelta = 0.7f;
        public float StaminaBurnRate = 0.7f;
        public float InstantKillDistance = -1f;
        public Vector3 EffectDirection = Vector3.up;
    }

    public class PlayerExplosionInfo
    {
        public List<EBodyPart> ProcessedLimbs = [];
        public Dictionary<BodyPartCollider, float> BodyPartColliders = [];
        
        private List<EBodyPart> _fracturableLimbs = [EBodyPart.LeftArm, EBodyPart.RightArm, EBodyPart.LeftLeg, EBodyPart.RightLeg];
        
        public EBodyPart GetClosestBodyPart()
        {
            EBodyPart bodyPart = EBodyPart.Chest;
            float distance = float.MaxValue;

            foreach (KeyValuePair<BodyPartCollider, float> kvp in BodyPartColliders)
            {
                if (kvp.Value < distance)
                {
                    bodyPart = kvp.Key.BodyPartType;
                    distance = kvp.Value;
                }
            }

            return bodyPart;
        }
        
        public EBodyPart GetClosestFracturableBodyPart()
        {
            EBodyPart bodyPart = EBodyPart.Chest;
            float distance = float.MaxValue;

            foreach (KeyValuePair<BodyPartCollider, float> kvp in BodyPartColliders)
            {
                if (kvp.Value < distance && _fracturableLimbs.Contains(kvp.Key.BodyPartType))
                {
                    bodyPart = kvp.Key.BodyPartType;
                    distance = kvp.Value;
                }
            }

            return bodyPart;
        }
    }

    public static class ExplosionHelper
    {
        public static void CreateExplosion(ExplosionData explosion)
        {
            Dictionary<Player, PlayerExplosionInfo> affectedPlayers = [];
            
            Singleton<Effects>.Instance.EmitGrenade(explosion.EffectName, explosion.Position, explosion.EffectDirection, 1f);
            
            Collider[] overlapColliders = Physics.OverlapSphere(explosion.Position, explosion.MaxDistance);

            // grab all colliders and players
            foreach (Collider collider in overlapColliders)
            {
                BodyPartCollider bodyPartCollider = collider.GetComponent<BodyPartCollider>();
                if (bodyPartCollider == null) continue;
                
                Player player = bodyPartCollider.GetComponentInParent<Player>();
                if (player == null) continue;

                if (!affectedPlayers.ContainsKey(player))
                {
                    affectedPlayers.Add(player, new PlayerExplosionInfo());
                }

                if (!affectedPlayers[player].BodyPartColliders.ContainsKey(bodyPartCollider))
                {
                    affectedPlayers[player].BodyPartColliders.Add(
                        bodyPartCollider,
                        Vector3.Distance(bodyPartCollider.transform.position, explosion.Position)
                    );
                    
                    Plugin.Logger.LogInfo($"collider count for player {player.name}: {affectedPlayers[player].BodyPartColliders.Count}");
                }
            }
            
            Plugin.Logger.LogInfo($"players in range: {affectedPlayers.Count}");
            
            // apply screen effects to affected players
            foreach (KeyValuePair<Player, PlayerExplosionInfo> kvp in affectedPlayers)
            {
                Player player = kvp.Key;
                PlayerExplosionInfo info = kvp.Value;
                
                float distanceFromExplosion = Vector3.Distance(player.Position, explosion.Position);
                Vector3 dirFromExplosion = Vector3.Normalize(player.Position - explosion.Position);
                float playerDistanceMultiplier = Mathf.Clamp01(distanceFromExplosion / explosion.MaxDistance);

                if (explosion.InstantKillDistance > 0 && distanceFromExplosion < explosion.InstantKillDistance)
                {
                    player.ActiveHealthController.Kill(EDamageType.Explosion);
                }
                
                player.ActiveHealthController.DoContusion(20f * playerDistanceMultiplier, playerDistanceMultiplier);
                player.ActiveHealthController.DoDisorientation(5f * playerDistanceMultiplier);
                player.ProceduralWeaponAnimation.ForceReact.AddForce(dirFromExplosion, playerDistanceMultiplier, 1f, 2f);

                if (Random.Range(0f, 1f) < explosion.FractureDelta * playerDistanceMultiplier)
                {
                    EBodyPart closestBodyPart = affectedPlayers[player].GetClosestFracturableBodyPart();

                    player.ActiveHealthController.DoFracture(closestBodyPart);
                }

                foreach (BodyPartCollider collider in info.BodyPartColliders.Keys)
                {
                    EBodyPart bodyPart = collider.BodyPartType;
                    EBodyPartColliderType colliderType = collider.BodyPartColliderType;

                    float colliderDistance = Vector3.Distance(collider.transform.position, explosion.Position);
                    float colliderDistanceMultiplier = 1f - Mathf.Clamp01(colliderDistance / explosion.MaxDistance);
                    Vector3 directionFromExplosion = Vector3.Normalize(collider.transform.position - explosion.Position);
                    float finalDamage = explosion.Damage * colliderDistanceMultiplier;
                
                    DamageInfoStruct damageInfo = new DamageInfoStruct
                    {
                        DamageType = EDamageType.Explosion,
                        Damage = finalDamage,
                        ArmorDamage = 0.35f,
                        PenetrationPower = 25,
                        Direction = directionFromExplosion,
                        HitNormal = -directionFromExplosion,
                        HitPoint = collider.transform.position,
                        Player = null,
                        Weapon = null,
                        HeavyBleedingDelta = explosion.HeavyBleedDelta,
                        LightBleedingDelta = explosion.LightBleedDelta,
                        StaminaBurnRate = explosion.StaminaBurnRate
                    };

                    player.ApplyDamageInfo(damageInfo, bodyPart, colliderType, 0f);
                    
                    Plugin.Logger.LogInfo($"applied damage to: {player.name} | damage:{damageInfo.Damage}");
                }
            }
        }
    }
}
