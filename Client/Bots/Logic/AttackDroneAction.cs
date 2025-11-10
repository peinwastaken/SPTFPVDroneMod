#if !UNITY_EDITOR
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using FPVDroneMod.Components;
using FPVDroneMod.Models;
using UnityEngine;

namespace FPVDroneMod.Bots.Logic
{
    public class AttackDroneAction : CustomLogic
    {
        private BotDroneListener _droneListener;
        private float TimeSinceLastShot = 0f;
        private float TimeBetweenShots = 2f;
        
        public AttackDroneAction(BotOwner botOwner) : base(botOwner)
        {
            _droneListener = botOwner.GetComponent<BotDroneListener>();
        }

        public override void Start()
        {
            BotOwner.SetPose(1f);
            BotOwner.BotLay.GetUp(true);
            BotOwner.SetTargetMoveSpeed(1f);
            BotOwner.Sprint(false);
            BotOwner.PatrollingData.Pause();
            BotOwner.WeaponManager.ShootController.SetAim(true);
            base.Stop();
        }

        public override void Stop()
        {
            BotOwner.Sprint(false);
            BotOwner.PatrollingData.Unpause();
            BotOwner.WeaponManager.ShootController.SetAim(false);
            base.Stop();
        }

        public override void Update(CustomLayer.ActionData data)
        {
            ClosestDroneData closestDrone = _droneListener.ClosestDroneData;
            TimeSinceLastShot += Time.deltaTime;

            if (closestDrone.Controller != null)
            {
                BotOwner.AimingManager.CurrentAiming.SetTarget(closestDrone.Controller.RigidBody.position);
                BotOwner.Steering.LookToPoint(closestDrone.Controller.RigidBody.position);

                if (TimeSinceLastShot > 2f)
                {
                    BotOwner.ShootData.Shoot();
                    TimeSinceLastShot = 0f;
                }
            }
        }
    }
}
#endif
