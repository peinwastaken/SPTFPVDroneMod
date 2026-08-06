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
            if (Item is PayloadItem payloadItem)
            {
                DebugLogger.LogInfo("hihiihihi");
                
                var ballistics = Singleton<GameWorld>.Instance.SharedBallisticsCalculator as BallisticsCalculator;
                if (ballistics == null) return;

                var shot = ballistics.CreateShot(
                    payloadItem,
                    transform.position,
                    transform.forward,
                    -1,
                    Owner.ProfileId,
                    DroneController.Item
                );
                ballistics.Shoot(shot);
                shot.HandleCollision(Time.deltaTime, shot._currentPosition, shot._currentVelocity);
            }
            
            OnDetonate?.Invoke();
        }

        public abstract void OnTriggerEnter(Collider collider);
        
        public abstract void OnTriggerExit(Collider collider);

    }
}

