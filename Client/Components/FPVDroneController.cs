using EFT.Ballistics;
using FPVDroneMod.Interface;
using UnityEngine;
#if !UNITY_EDITOR
using FPVDroneMod.Helpers;
using EFT;
using FPVDroneMod.Config;
using FPVDroneMod.Models;
#endif

namespace FPVDroneMod.Components
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

        private void Start()
        {
            BatteryRemaining = MaxBattery;
            PropellerSpeed = MinPropellerSpeed;

            HudController.SetArmedTextVisible(DroneDetonator.Armed);

            RigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            RigidBody.interpolation = RigidbodyInterpolation.Interpolate;
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

            if (HudController)
            {
                HudController.UpdateBatteryLevel(BatteryRemaining / MaxBattery);

                Player player = InstanceHelper.LocalPlayer;
                float distanceFromPlayer = (player.Position - transform.position).magnitude;
                HudController.UpdateSignalStrength(1f - Mathf.Clamp01(distanceFromPlayer / 1000f));

                RaycastHit hit;
                HudController.UpdateAltitude(
                    Physics.Raycast(transform.position, Vector3.down, out hit, 999f, LayerMaskClass.HighPolyWithTerrainMask) ?
                    hit.distance : 999f
                );

                HudController.UpdateSpeed(RigidBody.velocity.magnitude * 3.6f);
            }
        }

        private void Update()
        {
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

                float speedTarget = Mathf.Lerp(MinPropellerSpeed, MaxPropellerSpeed, Thrust);
                PropellerSpeed = Mathf.Lerp(PropellerSpeed, speedTarget, PropellerAccelerationSpeed * dt);

                BatteryRemaining -= (Thrust > 0 ? BatteryDecayRateAccel : BatteryDecayRateIdle) * dt;
                BatteryRemaining = Mathf.Clamp(BatteryRemaining, 0, MaxBattery);
            }
            else
            {
                Thrust = 0f;
                PropellerSpeed = Mathf.Lerp(PropellerSpeed, 0, PropellerAccelerationSpeed * dt);
            }

            if (PropellerSpeed > 0f)
            {
                RotatePropellers(PropellerSpeed);
            }
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
