using EFT;
using FPVDroneModClient.Interface;
using FPVDroneModClient.Models;
using UnityEngine;
#if !UNITY_EDITOR
using FPVDroneModClient.Config;
using FPVDroneModClient.Helpers;
#endif

namespace FPVDroneModClient.Components
{
    public class FPVDroneController : BaseDroneController, IDetonatable, IArmable
    {
        public Transform DetonatorGameObject;
        public DroneDetonator DroneDetonator;

        #if !UNITY_EDITOR
        public override void OnPilotEnter()
        {
            base.OnPilotEnter();
            
            DetonatorGameObject.gameObject.layer = LayerMask.NameToLayer("Default");
        }
        
        public override void OnPilotExit()
        {
            base.OnPilotExit();
        }

        protected override void GetReferences()
        {
            base.GetReferences();
            
            DroneDetonator = DetonatorGameObject.GetComponent<DroneDetonator>();
        }

        protected override void Start()
        {
            base.Start();
            
            HudController.SetArmedTextVisible(DroneDetonator.Armed);
        }

        public override void OnHit(DamageInfoStruct damageInfo)
        {
            DebugLogger.LogInfo("drone was hit");
            Detonate();
        }

        public void ToggleArmed()
        {
            DroneDetonator.SetArmed(!DroneDetonator.Armed);
            HudController.SetArmedTextVisible(DroneDetonator.Armed);
        }

        public void Detonate()
        {
            if (!RigidBody)
            {
                GetReferences();
            }

            DroneHelper.ControlDrone(false);

            if (DroneHelper.CurrentController == this)
            {
                DroneHelper.CurrentController = null;
            }

            ExplosionData explosion = new ExplosionData
            {
                Position = RigidBody.position,
                Damage = ExplosionConfig.ExplosionDamage.Value,
                MaxDistance = ExplosionConfig.ExplosionMaxDistance.Value,
                HeavyBleedDelta = ExplosionConfig.ExplosionHeavyBleedDelta.Value,
                LightBleedDelta = ExplosionConfig.ExplosionLightBleedDelta.Value,
                FractureDelta = ExplosionConfig.ExplosionFractureDelta.Value,
                StaminaBurnRate = ExplosionConfig.ExplosionStaminaBurnRate.Value,
                PlayerOwner = null, // TODO: fix ts
                Weapon = null // TODO: fix ts
            };

            ExplosionHelper.CreateExplosion(explosion);

            BotDroneListener.RemoveDrone(this);
            Destroy(gameObject);
        }
        
        public override void ApplyPitch(float amount)
        {
            RigidBody.rotation *= Quaternion.Euler(amount, 0, 0);
        }

        public override void ApplyYaw(float amount)
        {
            RigidBody.rotation *= Quaternion.Euler(0, amount, 0);
        }

        public override void ApplyRoll(float amount)
        {
            RigidBody.rotation *= Quaternion.Euler(0, 0, amount);
        }

        public override void ApplyThrust(float amount)
        {
            amount = Mathf.Clamp01(amount);
            float thrustForce = ThrustForce * amount;
            
            Vector3 velocity = RigidBody.velocity;
            Vector3 upForce = transform.up * thrustForce;
            Vector3 counterForce = Vector3.zero;

            if (velocity.magnitude > MaxVelocity && Thrust > 0f)
            {
                Vector3 excess = velocity.normalized * (velocity.magnitude - MaxVelocity);
                counterForce = -excess * RigidBody.mass / Time.fixedDeltaTime;
            }

            RigidBody.AddForce(upForce + counterForce, ForceMode.Acceleration);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            Thrust = Mathf.Lerp(Thrust, DroneInput.ThrottleInput, PropellerAccelerationSpeed * Time.fixedDeltaTime);
            ApplyThrust(Thrust);
        }

        protected override void Update()
        {
            base.Update();
            
            float dt = Time.deltaTime;
            if (!DroneInput)
            {
                DebugLogger.LogError("DRONEINPUT IS NULL");
            }

            if (BatteryRemaining > 0f)
            {
                if (DroneInput.RollInput != 0f) ApplyRoll(DroneInput.RollInput * RollSpeed * dt);
                if (DroneInput.PitchInput != 0f) ApplyPitch(DroneInput.PitchInput * PitchSpeed * dt);
                if (DroneInput.YawInput != 0f) ApplyYaw(DroneInput.YawInput * YawSpeed * dt);
            }
        }

        protected override void UpdateFromConfig()
        {
            RigidBody.mass = FPVDroneConfig.DroneMass.Value;
            RigidBody.angularDrag = 15f;

            ThrustForce = FPVDroneConfig.DroneThrustForce.Value;
            MaxVelocity = FPVDroneConfig.DroneMaxVelocity.Value;

            PitchSpeed = FPVDroneConfig.DronePitchSpeed.Value;
            YawSpeed = FPVDroneConfig.DroneYawSpeed.Value;
            RollSpeed = FPVDroneConfig.DroneRollSpeed.Value;

            PropellerAccelerationSpeed = FPVDroneConfig.DronePropellerAccelerationSpeed.Value;
            MinPropellerSpeed = FPVDroneConfig.DroneMinPropellerSpeed.Value;
            MaxPropellerSpeed = FPVDroneConfig.DroneMaxPropellerSpeed.Value;

            MaxBattery = FPVDroneConfig.DroneMaxBattery.Value;
            BatteryDecayRateIdle = FPVDroneConfig.DroneBatteryDecayIdle.Value;
            BatteryDecayRateAccel = FPVDroneConfig.DroneBatteryDecayAccel.Value;
        }
        #endif
        
        #if UNITY_EDITOR
        public void Detonate()
        {
            throw new System.NotImplementedException();
        }

        public void ToggleArmed()
        {
            throw new System.NotImplementedException();
        }
        #endif
    }
}
