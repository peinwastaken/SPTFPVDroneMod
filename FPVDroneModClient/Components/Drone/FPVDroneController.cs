using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Interface;
#if !UNITY_EDITOR
using FPVDroneModClient.Config;
using FPVDroneModClient.Helpers;
#endif
using UnityEngine;

namespace FPVDroneModClient.Components.Drone
{
    public class FPVDroneController : BaseDroneController, IArmable
    {
        public bool IsArmed { get; set; }
        
        #if !UNITY_EDITOR
        public override void OnPilotEnter()
        {
            base.OnPilotEnter();

            if (PayloadController)
            {
                PayloadController.gameObject.layer = LayerMask.NameToLayer("Default");
            }

            HudController.CustomizedText.enabled = FPVDroneConfig.EnableCustomizedText.Value;
            HudController.CustomizedText.enableWordWrapping = FPVDroneConfig.CustomizedTextWrapping.Value;
            HudController.SetCustomizedText(FPVDroneConfig.CustomizedText.Value);
        }
        
        public override void OnPilotExit()
        {
            base.OnPilotExit();
        }

        protected override void GetReferences()
        {
            base.GetReferences();
        }

        protected override void Start()
        {
            base.Start();
            
            HudController.SetArmedTextVisible(PayloadController.IsArmed);
        }

        public override void OnHit(DamageInfoStruct damageInfo)
        {
            DebugLogger.LogInfo("drone was hit");
            Detonate();
        }

        public void ToggleArmed()
        {
            PayloadController.ToggleArmed();
            HudController.SetArmedTextVisible(PayloadController.IsArmed);
        }

        public void Detonate()
        {
            if (!RigidBody)
            {
                GetReferences();
            }
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
            return;
        }

        public void ToggleArmed()
        {
            return;
        }
        #endif
    }
}
