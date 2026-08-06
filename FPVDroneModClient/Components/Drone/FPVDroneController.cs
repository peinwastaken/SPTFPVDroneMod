using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Config;
using FPVDroneModClient.Helpers;
using UnityEngine;

namespace FPVDroneModClient.Components.Drone
{
    public class FPVDroneController : BaseDroneController
    {
        public bool IsArmed { get; set; }
        
        public override void OnPilotEnter(bool isDoneLocally = true)
        {
            base.OnPilotEnter(isDoneLocally);

            HudController.CustomizedText.enabled = FPVDroneConfig.EnableCustomizedText.Value;
            HudController.CustomizedText.enableWordWrapping = FPVDroneConfig.CustomizedTextWrapping.Value;
            HudController.SetCustomizedText(FPVDroneConfig.CustomizedText.Value);
        }

        protected override void Start()
        {
            base.Start();

            if (Armable != null)
            {
                HudController.SetArmedTextVisible(Armable.IsArmed);
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

            if (BatteryResource.Value > 0f)
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
            
            BatteryDecayRateIdle = FPVDroneConfig.DroneBatteryDecayIdle.Value;
            BatteryDecayRateAccel = FPVDroneConfig.DroneBatteryDecayAccel.Value;
        }

        public override void OnToggleArmed(bool newState)
        {
            HudController.SetArmedTextVisible(newState);
        }
        
    }
}

