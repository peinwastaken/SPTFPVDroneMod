using EFT.InventoryLogic;
using FPVDroneModClient.Interface;
using FPVDroneModClient.Models;
using System;
using UnityEngine;
using EFT;
using EFT.Ballistics;
using Comfort.Common;
using FPVDroneModClient.Config;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Items;
using Systems.Effects;

namespace FPVDroneModClient.Components.Base
{
    public abstract class BasePayloadController : MonoBehaviour, IArmable, IDetonatable, IOwnable
    {
        public bool IsArmed { get; set; } = false;
        public bool IsAntiTank;
        public string Description { get; }
        public Detonator Detonator;
        public event Action<bool> OnToggleArmed;
        public event Action OnDetonate;
        public IPlayer Owner { get; set; }
        public BaseDroneController DroneController;
        public Item Item;
        public BallisticCollider BallisticCollider;
        public bool IsBeingDetonated = false;

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

        public void OnHit(DamageInfo damageInfo)
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
            var explosiveComponent = Item.GetItemComponent<ExplosiveAmmoComponent>();
            if (explosiveComponent != null && Item is PayloadItem payloadItem)
            {
                var ballistics = Singleton<GameWorld>.Instance.SharedBallisticsCalculator as BallisticsCalculator;
                if (ballistics == null) return;
                
                var template = (PayloadItemTemplate)payloadItem.Template;
                var pos = transform.position;
                var shift = Vector3.up * 0.08f;
                
                Singleton<Effects>.Instance.EmitGrenade(
                    template.ExplosionType,
                    pos,
                    Vector3.up
                );
                
                Grenade.Explosion(
                    null,
                    explosiveComponent,
                    transform.position,
                    Owner.ProfileId,
                    ballistics,
                    DroneController.Item,
                    shift
                );
            }
            
            OnDetonate?.Invoke();
        }

        public abstract void OnTriggerEnter(Collider collider);
        
        public abstract void OnTriggerExit(Collider collider);
    }
}

