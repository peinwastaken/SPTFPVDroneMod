using System;
using UnityEngine;

#if !UNITY_EDITOR
using FPVDroneMod.Helpers;
using EFT;
using FPVDroneMod.Models;
#endif

namespace FPVDroneMod.Components
{
    public class DroneController : MonoBehaviour
    {
        public float ThrustForce = 20f;
        public float MaxVelocity = 100f;
        public float PitchSpeed = 100f;
        public float YawSpeed = 100f;
        public float RollSpeed = 100f;
        public float PropellerAccelerationSpeed = 4f;
        public float MinPropellerSpeed = 0f;
        public float MaxPropellerSpeed = 10000f;
        public float MaxBattery = 150f;
        public float BatteryDecayRateIdle = 0.001f;
        public float BatteryDecayRateAccel = 0.01f;
        public float Thrust = 0f;
        
        public Transform PropellerFR;
        public Transform PropellerFL;
        public Transform PropellerRR;
        public Transform PropellerRL;
        public Transform DetonatorGameObject;
        public Transform CameraGameObject;
        public Transform CameraPos;
        
        public Rigidbody RigidBody;
        public DroneDetonator DroneDetonator;
        public DroneSoundController DroneSoundController;
        public float PropellerSpeed;
        public float BatteryRemaining;
        
        #if !UNITY_EDITOR
        private void Awake()
        {
            RigidBody = GetComponent<Rigidbody>();
            DroneDetonator = DetonatorGameObject.GetComponent<DroneDetonator>();
            DroneSoundController = gameObject.GetComponent<DroneSoundController>();
        }

        private void Start()
        {
            BatteryRemaining = MaxBattery;
            PropellerSpeed = MinPropellerSpeed;
            
            InstanceHelper.DroneHudController.SetArmedTextVisible(DroneDetonator.Armed);
            
            RigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            RigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        public void OnControl(bool state)
        {
            enabled = state;
            CameraGameObject.gameObject.SetActive(!state);

            if (state)
            {
                DroneSoundController.AudioSource.Play();
            }
            else
            {
                DroneSoundController.AudioSource.Stop();
            }
        }

        private void ApplyPitch(float amount)
        {
            RigidBody.rotation *= Quaternion.Euler(amount, 0, 0);
        }

        private void ApplyYaw(float amount)
        {
            RigidBody.rotation *= Quaternion.Euler(0, amount, 0);
        }

        private void ApplyRoll(float amount)
        {
            RigidBody.rotation *= Quaternion.Euler(0, 0, amount);
        }

        private void RotatePropellers(float amount)
        {
            PropellerFR.Rotate(Vector3.right, amount);
            PropellerFL.Rotate(Vector3.right, amount);
            PropellerRR.Rotate(Vector3.right, amount);
            PropellerRL.Rotate(Vector3.right, amount);
        }

        public void ToggleArmed()
        {
            DroneDetonator.SetArmed(!DroneDetonator.Armed);
            InstanceHelper.DroneHudController.SetArmedTextVisible(DroneDetonator.Armed);
        }

        public void ResetTransform()
        {
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
        }

        public void Detonate()
        {
            DroneHelper.ControlDrone(false);

            if (DroneHelper.CurrentController == this)
            {
                DroneHelper.CurrentController = null;
            }

            ExplosionData explosion = new ExplosionData
            {
                Position = RigidBody.position
            };
            
            ExplosionHelper.CreateExplosion(explosion);
            
            Destroy(gameObject.transform.parent.gameObject);
        }

        private void FixedUpdate()
        {
            Thrust = Mathf.Lerp(Thrust, Input.GetKey(KeyCode.W) ? 1f : 0f, PropellerAccelerationSpeed * Time.fixedDeltaTime);
            
            float thrustForce = ThrustForce * Thrust;

            Vector3 velocity = RigidBody.velocity;
            Vector3 upForce = transform.up * thrustForce;
            Vector3 counterForce = Vector3.zero;

            if (velocity.magnitude > MaxVelocity && Thrust > 0f)
            {
                Vector3 excess = velocity.normalized * (velocity.magnitude - MaxVelocity);
                counterForce = -excess * RigidBody.mass / Time.fixedDeltaTime;
            }

            RigidBody.AddForce(upForce + counterForce, ForceMode.Acceleration);
            
            DroneHudController hud = InstanceHelper.DroneHudController;

            if (hud)
            {
                hud.UpdateBatteryLevel(BatteryRemaining / MaxBattery);

                Player player = InstanceHelper.LocalPlayer;
                float distanceFromPlayer = (player.Position - transform.position).magnitude; 
                hud.UpdateSignalStrength(1f - Mathf.Clamp01(distanceFromPlayer / 1000f));

                RaycastHit hit;
                hud.UpdateAltitude(
                    Physics.Raycast(transform.position, Vector3.down, out hit, 999f, LayerMaskClass.HighPolyWithTerrainMask) ? 
                    hit.distance : 999f
                );
                
                hud.UpdateSpeed(RigidBody.velocity.magnitude * 3.6f);
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                DroneHelper.ControlDrone(false);
            }
            
            if (Input.GetKeyDown(KeyCode.K))
            {
                ToggleArmed();
            }

            if (BatteryRemaining > 0)
            {
                float rollInput = (Input.GetKey(KeyCode.RightArrow) ? -1f : 0f) + (Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
                float pitchInput = (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) + (Input.GetKey(KeyCode.DownArrow) ? -1f : 0f);
                float yawInput = (Input.GetKey(KeyCode.D) ? 1f : 0f) + (Input.GetKey(KeyCode.A) ? -1f : 0f);

                if (rollInput != 0f) ApplyRoll(rollInput * RollSpeed * dt);
                if (pitchInput != 0f) ApplyPitch(pitchInput * PitchSpeed * dt);
                if (yawInput != 0f) ApplyYaw(yawInput * YawSpeed * dt);

                float speedTarget = Mathf.Lerp(MinPropellerSpeed, MaxPropellerSpeed, Thrust);
                PropellerSpeed = Mathf.Lerp(PropellerSpeed, speedTarget, PropellerAccelerationSpeed * dt);

                BatteryRemaining -= (Thrust > 0 ? BatteryDecayRateAccel : BatteryDecayRateIdle) * dt;
                BatteryRemaining = Mathf.Clamp(BatteryRemaining, 0, MaxBattery);
            }
            else
            {
                Thrust = 0;
                PropellerSpeed = Mathf.Lerp(PropellerSpeed, 0, PropellerAccelerationSpeed * dt);
            }

            if (PropellerSpeed > 0)
            {
                RotatePropellers(PropellerSpeed);
            }
        }
        #endif
    }
}
