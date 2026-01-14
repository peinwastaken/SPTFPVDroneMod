using EFT.InventoryLogic;
using FPVDroneModClient.Config;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Interface;
using FPVDroneModClient.Items;
using FPVDroneModClient.Models;
using UnityEngine;

namespace FPVDroneModClient.Components.Base
{
    public abstract class BasePayloadController : MonoBehaviour, IArmable, IDetonatable, IPhysicsTrigger
    {
        public bool IsArmed { get; set; } = false;
        public string Description { get; }

        public virtual void ToggleArmed()
        {
            IsArmed = !IsArmed;
        }

        public virtual void Detonate(Vector3 position, IPlayerOwner playerOwner, Item weapon)
        {
            ExplosionData explosion = new ExplosionData
            {
                Position = position,
                Damage = ExplosionConfig.ExplosionDamage.Value,
                MaxDistance = ExplosionConfig.ExplosionMaxDistance.Value,
                HeavyBleedDelta = ExplosionConfig.ExplosionHeavyBleedDelta.Value,
                LightBleedDelta = ExplosionConfig.ExplosionLightBleedDelta.Value,
                FractureDelta = ExplosionConfig.ExplosionFractureDelta.Value,
                StaminaBurnRate = ExplosionConfig.ExplosionStaminaBurnRate.Value,
                PlayerOwner = playerOwner,
                Weapon = weapon
            };

            ExplosionHelper.CreateExplosion(explosion);    
        }
    
        public virtual void OnTriggerEnter(Collider collider) {}
        public virtual void OnTriggerExit(Collider collider) {}
    }
}