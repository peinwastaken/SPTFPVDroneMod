#if !UNITY_EDITOR
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using FPVDroneMod.Components;
using FPVDroneMod.Enum;
using FPVDroneMod.Helpers;
using FPVDroneMod.Models;
using System.Text;
using UnityEngine;

namespace FPVDroneMod.Bots.Logic
{
    public class AttackDroneAction : CustomLogic
    {
        private BotDroneListener _droneListener;
        private float TimeSinceLastShot = 0f;
        private float TimeToNextShot = 2f;
        private int ShotCount = 0;
        
        public AttackDroneAction(BotOwner botOwner) : base(botOwner)
        {
            _droneListener = botOwner.GetComponent<BotDroneListener>();
        }

        public override void Start()
        {
            DebugLogger.LogInfo("start attack action");
            BotOwner.SetPose(1f);
            BotOwner.BotLay.GetUp(true);
            BotOwner.SetTargetMoveSpeed(1f);
            BotOwner.Sprint(false);
            BotOwner.PatrollingData.Pause();
            BotOwner.WeaponManager.ShootController.SetAim(true);
            base.Start();
        }

        public override void Stop()
        {
            DebugLogger.LogInfo("stop attack action");
            BotOwner.Sprint(false);
            BotOwner.PatrollingData.Unpause();
            BotOwner.WeaponManager.ShootController.SetAim(false);
            ShotCount = 0;
            base.Stop();
        }
        
        public override void BuildDebugText(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"ShotCount: {ShotCount}");
        }

        public override void Update(CustomLayer.ActionData data)
        {
            ClosestDroneData closestDrone = _droneListener.ClosestDroneData;
            TimeSinceLastShot += Time.deltaTime;

            if (closestDrone.Controller == null) return;

            BotOwner.AimingManager.CurrentAiming.SetTarget(closestDrone.Controller.RigidBody.position);
            BotOwner.Steering.LookToPoint(closestDrone.Controller.RigidBody.position);

            if (TimeSinceLastShot > TimeToNextShot && _droneListener.IsClosestDroneVisible())
            {
                BotOwner.ShootData.Shoot();
                TimeSinceLastShot = 0f;
                TimeToNextShot = Random.Range(0.5f, 2.5f);
                ShotCount++;
                DebugLogger.LogWarning($"SHOT!!!!!! count: {ShotCount}");
            }

            if (ShotCount >= 5 || TimeSinceLastShot >= 10f)
            {
                DebugLogger.LogInfo("5 shots should stop now thx. or waited too long");
                _droneListener.SetAction(EDroneCombatAction.EvadeDrone);
            }
        }
    }
}
#endif
