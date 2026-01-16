using System.Collections.Generic;
using Comfort.Common;
using EFT;
using EFT.Vehicle;
using FPVDroneModClient.Components.Base;
using UnityEngine;
#if !UNITY_EDITOR
using FPVDroneModClient.Helpers;
#endif

namespace FPVDroneModClient.Components.Detonator
{
    public class ContactDetonator : BasePayloadController
    {
        #if !UNITY_EDITOR
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
                    
                    btr.gameObject.SetActive(false);
                    InstanceHelper.CreateTankCorpse(btr.transform.position, btr.transform.eulerAngles, true);

                    RouteHelper.UpdateTankDeathState(true, mapId, btr.transform.position, btr.transform.eulerAngles);
                }
                
                Detonate();
            }
        }
        #endif
    }
}