
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Config;
using FPVDroneModClient.Helpers;
using UnityEngine;

namespace FPVDroneModClient.Components.Drone
{
    public class ReconDroneController : BaseDroneController
    {
        public float CameraPitch = 0f;
        public float CameraZoom = 0f;
        
        #if !UNITY_EDITOR
        public override void OnPilotEnter()
        {
            base.OnPilotEnter();
            
            RigidBody.isKinematic = false;
        }

        public override void OnPilotExit()
        {
            base.OnPilotExit();

            if (!Grounded && DroneHelper.CurrentController == this)
            {
                enabled = true;
                RigidBody.isKinematic = true;
                DroneSoundController.enabled = true;
            }
        }

        protected override void UpdateFromConfig()
        {
            RigidBody.mass = ReconDroneConfig.DroneMass.Value;
            RigidBody.angularDrag = 15f;
            RigidBody.drag = 0.75f;

            ThrustForce = ReconDroneConfig.DroneThrustForce.Value;
            MaxVelocity = ReconDroneConfig.DroneMaxVelocity.Value;

            PitchSpeed = ReconDroneConfig.DronePitchSpeed.Value;
            YawSpeed = ReconDroneConfig.DroneYawSpeed.Value;
            RollSpeed = ReconDroneConfig.DroneRollSpeed.Value;

            PropellerAccelerationSpeed = ReconDroneConfig.DronePropellerAccelerationSpeed.Value;
            MinPropellerSpeed = ReconDroneConfig.DroneMinPropellerSpeed.Value;
            MaxPropellerSpeed = ReconDroneConfig.DroneMaxPropellerSpeed.Value;

            MaxBattery = ReconDroneConfig.DroneMaxBattery.Value;
            BatteryDecayRateIdle = ReconDroneConfig.DroneBatteryDecayIdle.Value;
            BatteryDecayRateAccel = ReconDroneConfig.DroneBatteryDecayAccel.Value;
        }

        public override void OnHit(DamageInfoStruct damageInfo)
        {
            DroneHelper.ControlDrone(false);

            if (DroneHelper.CurrentController == this)
            {
                DroneHelper.CurrentController = null;
            }
            
            BotDroneListener.RemoveDrone(this);
            Destroy(gameObject);
        }
        
        public override void ApplyPitch(float amount)
        {
            if (Grounded) return;
            RigidBody.AddForce(RigidBody.transform.forward * (ThrustForce * amount), ForceMode.Acceleration);
        }

        public override void ApplyYaw(float amount)
        {
            if (Grounded) return;
            RigidBody.rotation *= Quaternion.AngleAxis(YawSpeed * amount, Vector3.up);
        }

        public override void ApplyRoll(float amount)
        {
            if (Grounded) return;
            RigidBody.AddForce(RigidBody.transform.right * (ThrustForce * amount), ForceMode.Acceleration);
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

            RigidBody.AddForce(upForce + counterForce, ForceMode.Force);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!RigidBody || BatteryRemaining <= 0f) return;
            
            if (!Grounded && Mathf.Approximately(DroneInput.AltitudeInput, 0f))
            {
                ApplyStableThrust();
                
                float yaw = RigidBody.rotation.eulerAngles.y;
                RigidBody.rotation = Quaternion.Lerp(RigidBody.rotation, Quaternion.Euler(0f, yaw, 0f), 8f * Time.fixedDeltaTime);
            }
            else
            {
                Thrust = Mathf.Lerp(Thrust, DroneInput.AltitudeInput, PropellerAccelerationSpeed * Time.fixedDeltaTime);
                ApplyThrust(Thrust);
            }

            Canvas canvas = HudController.GetComponent<Canvas>();
            if (canvas)
            {
                canvas.planeDistance = Mathf.Lerp(0.055f, 1f, CameraZoom);
            }
        }

        protected override void Update()
        {
            base.Update();
            
            float dt = Time.deltaTime;

            if (BatteryRemaining > 0f)
            {
                if (DroneInput.RollInput != 0f) ApplyRoll(DroneInput.RollInput * 20f * dt);
                if (DroneInput.PitchInput != 0f) ApplyPitch(DroneInput.PitchInput * 20f * dt);
                if (DroneInput.YawInput != 0f) ApplyYaw(DroneInput.YawInput * 20f * dt);

                CameraPitch += DroneInput.CameraPitchInput * 100f * dt;
                CameraPitch = Mathf.Clamp(CameraPitch, 0f, 90f);

                CameraZoom += DroneInput.CameraZoomInput;
                CameraZoom = Mathf.Clamp(CameraZoom, 0f, 1f);
            }
        }
        #endif
    }
}
