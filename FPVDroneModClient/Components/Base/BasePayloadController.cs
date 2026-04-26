using EFT.InventoryLogic;
using FPVDroneModClient.Interface;
using FPVDroneModClient.Models;
using System;
using UnityEngine;
using EFT;
using EFT.Ballistics;
#if !UNITY_EDITOR
using Comfort.Common;
using FPVDroneModClient.Config;
using FPVDroneModClient.Helpers;
#endif

namespace FPVDroneModClient.Components.Base
{
    public abstract class BasePayloadController : MonoBehaviour, IArmable, IDetonatable, IOwnable
    {
        public bool IsArmed { get; set; } = false;
        public bool IsAntiTank;
        public float Damage;
        public float MaxDistance;
        public float HeavyBleedDelta;
        public float LightBleedDelta;
        public float FractureDelta;
        public float StaminaBurnRate;
        public string Description { get; }
        public Detonator Detonator;
        public event Action<bool> OnToggleArmed;
        public event Action OnDetonate;
        public IPlayer Owner { get; set; }
        public BaseDroneController DroneController;
        public Item Item;
        public BallisticCollider BallisticCollider;

        #if !UNITY_EDITOR
        private void Awake()
        {
            BallisticCollider = GetComponentInChildren<BallisticCollider>();
            Detonator = GetComponentInChildren<Detonator>();

            BallisticCollider.OnHitAction += OnHit;
            
            if (Detonator)
            {
                Detonator.TriggerEntered += OnTriggerEnter;
                Detonator.TriggerExited += OnTriggerExit;
            }
        }

        public void OnHit(DamageInfoStruct damageInfo)
        {
            Detonate();
        }

        public virtual void ToggleArmed()
        {
            IsArmed = !IsArmed;
            
            OnToggleArmed?.Invoke(IsArmed);
        }

        public virtual ExplosionData Detonate()
        {
            IPlayerOwner owner = Singleton<GameWorld>.Instance.GetAlivePlayerBridgeByProfileID(Owner.ProfileId);
            
            ExplosionData explosion = new ExplosionData
            {
                Position = gameObject.transform.position,
                Damage = Damage,
                MaxDistance = MaxDistance,
                HeavyBleedDelta = HeavyBleedDelta,
                LightBleedDelta = LightBleedDelta,
                FractureDelta = FractureDelta,
                StaminaBurnRate = StaminaBurnRate,
                PlayerOwner = owner,
                Weapon = DroneController.Item
            };

            ExplosionHelper.CreateExplosion(explosion);
            
            OnDetonate?.Invoke();

            return explosion;
        }

        public abstract void OnTriggerEnter(Collider collider);
        
        public abstract void OnTriggerExit(Collider collider);
        #endif

        #if UNITY_EDITOR
        public void OnHit(DamageInfoStruct damageInfo)
        {
            Detonate();
        }
        
        public virtual void ToggleArmed()
        {
            return;
        }

        public virtual ExplosionData Detonate()
        {
            return null;
        }
        
        public abstract void OnTriggerEnter(Collider collider);
        
        public abstract void OnTriggerExit(Collider collider);
        #endif
    }
}