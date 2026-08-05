using EFT.Ballistics;
using EFT.InventoryLogic;
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Interface;
using UnityEngine;
#if !UNITY_EDITOR
using FPVDroneModClient.Items;
using FPVDroneModClient.Helpers;
#endif

namespace FPVDroneModClient.Components
{
    // hi ozen if you ever see this
    // this is only for collision detection
    // sorry to disappoint
    public class BatteryController : MonoBehaviour
    {
        public Item Item;
        public BallisticCollider BallisticCollider;
        public BaseDroneController DroneController;

        #if !UNITY_EDITOR
        public void Awake()
        {
            BallisticCollider = GetComponentInChildren<BallisticCollider>();

            if (!BallisticCollider)
            {
                DebugLogger.LogWarning("no ballistic collider found!");
            }
            
            BallisticCollider.OnHitAction += OnHit;
        }

        public void OnHit(DamageInfo DamageInfo)
        {
            DebugLogger.LogInfo("battery hit");
            Destroy(gameObject);
            
            if (DroneController)
            {
                DebugLogger.LogInfo("found dronecontroller");
                IDetonatable detonatable = DroneController.Detonatable;
                
                if (detonatable is BasePayloadController payload)
                {
                    DebugLogger.LogInfo("found payload");
                    payload.OnHit(DamageInfo);
                    return;
                }
                
                DebugLogger.LogInfo("no payload");
                DroneController.OnHit(DamageInfo);
            }
        }
        #endif
    }
}
