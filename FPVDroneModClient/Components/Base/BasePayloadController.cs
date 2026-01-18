using EFT.InventoryLogic;
using FPVDroneModClient.Interface;
using System;
using UnityEngine;
#if !UNITY_EDITOR
using EFT.Ballistics;
using FPVDroneModClient.Config;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Models;
#endif

namespace FPVDroneModClient.Components.Base
{
    public abstract class BasePayloadController : MonoBehaviour, IArmable, IDetonatable
    {
        public bool IsArmed { get; set; } = false;
        public string Description { get; }
        public Detonator Detonator;
        public event Action<bool> OnToggleArmed;
        public event Action OnDetonate;

        #if !UNITY_EDITOR
        private void Awake()
        {
            BallisticCollider collider = GetComponent<BallisticCollider>();
            Detonator = GetComponentInChildren<Detonator>();

            collider.OnHitAction += OnHit;
            
            if (Detonator)
            {
                Detonator.TriggerEntered += OnTriggerEnter;
                Detonator.TriggerExited += OnTriggerExit;
            }
        }

        private void OnHit(DamageInfoStruct damageInfo)
        {
            Detonate();
        }

        public virtual void ToggleArmed()
        {
            IsArmed = !IsArmed;
            
            OnToggleArmed?.Invoke(IsArmed);
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
            
            OnDetonate?.Invoke();
        }

        public abstract void OnTriggerEnter(Collider collider);
        
        public abstract void OnTriggerExit(Collider collider);
        #endif

        #if UNITY_EDITOR
        public virtual void ToggleArmed()
        {
            
        }

        public virtual void Detonate()
        {
            
        }
        
        public abstract void OnTriggerEnter(Collider collider);
        
        public abstract void OnTriggerExit(Collider collider);
        #endif
    }
}