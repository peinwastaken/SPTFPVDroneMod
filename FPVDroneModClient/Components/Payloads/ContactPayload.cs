using EFT;
using FPVDroneModClient.Components.Base;
using UnityEngine;
#if !UNITY_EDITOR
using Comfort.Common;
using EFT.Vehicle;
using FPVDroneModClient.Helpers;
using UnityEngine.UIElements;
#endif

namespace FPVDroneModClient.Components.Payloads
{
    public class ContactPayload : BasePayloadController
    {
        #if !UNITY_EDITOR
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
                    string mapId = Singleton<GameWorld>.Instance.LocationId;
                    DebugLogger.LogInfo("HIT!!!!! SOMETHING!!!!");

                    if (btr)
                    {
                        btr.gameObject.SetActive(false);
                        btr.enabled = false;
                        InstanceHelper.CreateTankCorpse(btr.transform.position, btr.transform.eulerAngles, true);
                        RouteHelper.UpdateTankDeathState(true, mapId, btr.transform.position, btr.transform.eulerAngles);
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
        #endif
        
        #if UNITY_EDITOR
        public override void OnTriggerEnter(Collider collider)
        {
            
        }

        public override void OnTriggerExit(Collider collider)
        {
            
        }
        #endif
    }
}