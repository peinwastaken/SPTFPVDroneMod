using EFT.InventoryLogic;
using FPVDroneModClient.Interface;
using System;
using UnityEngine;
#if !UNITY_EDITOR
using FPVDroneModClient.Config;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Items;
using FPVDroneModClient.Models;
#endif

namespace FPVDroneModClient.Components.Base
{
    public abstract class BasePayloadController : MonoBehaviour, IArmable, IDetonatable, IPhysicsTrigger
    {
        public BaseDroneController DroneController;
        public bool IsArmed { get; set; } = false;
        public string Description { get; }

        #if !UNITY_EDITOR
        private void Awake()
        {
            DroneController = GetComponentInParent<BaseDroneController>();
        }

        public virtual void ToggleArmed()
        {
            IsArmed = !IsArmed;
        }

        public virtual void Detonate()
        {
            ExplosionData explosion = new ExplosionData
            {
                Position = gameObject.transform.position,
                Damage = ExplosionConfig.ExplosionDamage.Value,
                MaxDistance = ExplosionConfig.ExplosionMaxDistance.Value,
                HeavyBleedDelta = ExplosionConfig.ExplosionHeavyBleedDelta.Value,
                LightBleedDelta = ExplosionConfig.ExplosionLightBleedDelta.Value,
                FractureDelta = ExplosionConfig.ExplosionFractureDelta.Value,
                StaminaBurnRate = ExplosionConfig.ExplosionStaminaBurnRate.Value,
                PlayerOwner = null,
                Weapon = null
            };

            ExplosionHelper.CreateExplosion(explosion);
            
            DroneController.Destroy();
        }
    
        public virtual void OnTriggerEnter(Collider collider) {}
        public virtual void OnTriggerExit(Collider collider) {}
        #endif

        #if UNITY_EDITOR
        public virtual void ToggleArmed()
        {
            
        }

        public virtual void Detonate(Vector3 position)
        {
            
        }

        public virtual void OnTriggerEnter(Collider collider)
        {
            
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            
        }
        #endif
    }
}