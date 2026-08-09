using EFT;
using FPVDroneModClient.Components.Base;
using UnityEngine;
using Comfort.Common;
using EFT.Ballistics;
using EFT.Vehicle;
using FPVDroneModClient.Helpers;
using UnityEngine.UIElements;

namespace FPVDroneModClient.Components.Payloads
{
    public class ContactPayload : BasePayloadController
    {
        public override void OnTriggerEnter(Collider collider)
        {
            int layerMask = 1 << collider.gameObject.layer;
            bool hitSomethingOrWater = (layerMask & LayersMaskController.TripwireCheckLayerMask) != 0 || (layerMask & LayersMaskController.WaterLayer) != 0;
        
            if (IsArmed && hitSomethingOrWater)
            {
                DebugLogger.LogInfo(collider.gameObject.name);

                if (IsAntiTank)
                {
                    BTRView btr = collider.GetComponentInParent<BTRView>();
                    var gameWorld = Singleton<GameWorld>.Instance;
                    var mapId = gameWorld.LocationId;

                    if (btr)
                    {
                        DebugLogger.LogInfo("hit btr :)");
                        btr.gameObject.SetActive(false);
                        btr.enabled = false;
                        InstanceHelper.CreateTankCorpse(btr.transform.position, btr.transform.eulerAngles, true);
                        RouteHelper.UpdateTankDeathState(true, mapId, btr.transform.position, btr.transform.eulerAngles);
                        
                        var driver = BtrController.Instance.BotShooterBtr;
                        var playerBridge = gameWorld.GetAlivePlayerBridgeByProfileID(Owner.ProfileId);
                        var player = gameWorld.GetAlivePlayerByProfileID(Owner.ProfileId);
                        var damageInfo = new DamageInfo
                        {
                            Damage = 10000f,
                            BodyPartColliderType = EBodyPartColliderType.HeadCommon,
                            DamageType = EDamageType.Explosion,
                            Player = playerBridge,
                            Weapon = DroneController.Item
                        };
                        
                        driver.GetPlayer.ApplyDamageInfo(damageInfo, EBodyPart.Head, EBodyPartColliderType.HeadCommon, 0f);
                        driver.GetPlayer.OnBeenKilledByAggressor(player, damageInfo, EBodyPart.Head, EDamageType.Explosion);
                    }
                    else
                    {
                        DebugLogger.LogInfo("not hit btr :(");
                    }
                }
                
                Detonate();
            }
        }

        public override void OnTriggerExit(Collider collider)
        {
            
        }
    }
}

