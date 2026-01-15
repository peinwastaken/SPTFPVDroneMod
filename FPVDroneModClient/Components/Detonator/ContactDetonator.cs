using System.Collections.Generic;
using Comfort.Common;
using EFT;
using EFT.Vehicle;
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Helpers;
using UnityEngine;

namespace FPVDroneModClient.Components.Detonator
{
    public class ContactDetonator : BasePayloadController
    {
        public override void OnTriggerEnter(Collider collider)
        {
            int layerMask = 1 << collider.gameObject.layer;
            bool hitSomethingOrWater = (layerMask & LayerMaskClass.TripwireCheckLayerMask) != 0 || (layerMask & LayerMaskClass.WaterLayer) != 0;
        
            if (IsArmed && hitSomethingOrWater)
            {
                BTRVehicle btr = collider.GetComponentInParent<BTRVehicle>();

                if (btr)
                {
                    string mapId = Singleton<GameWorld>.Instance.LocationId;
                    IReadOnlyList<BTRPassenger> passengers = btr.Passengers;

                    RouteHelper.UpdateTankDeathState(true, mapId, btr.transform.position, btr.transform.eulerAngles);
                }
                
                Detonate(gameObject.transform.position, null, null);
            }
        }
    }
}