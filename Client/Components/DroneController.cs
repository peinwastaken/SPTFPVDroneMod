using EFT.GameTriggers;
using EFT.Interactive;
using System;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

#if !UNITY_EDITOR
using FPVDroneMod.Helpers;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using FPVDroneMod.Patches;
using Systems.Effects;
using Unity.Audio;
using UnityEngine.UIElements;
#endif

namespace FPVDroneMod.Components
{
    public class DroneController : MonoBehaviour
    {
        [Header("Drone Handling")]
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

        [Header("GameObjects")]
        public Transform PropellerFR;
        public Transform PropellerFL;
        public Transform PropellerRR;
        public Transform PropellerRL;
        public Transform DetonatorGameObject;
        public Transform CameraGameObject;
        public Transform CameraPos;
        
        public Rigidbody RigidBody;
        public DroneDetonator DroneDetonator;
        public float PropellerSpeed;
        public float BatteryRemaining;
        
        private void Start()
        {
            RigidBody = GetComponent<Rigidbody>();
            DroneDetonator = DetonatorGameObject.GetComponent<DroneDetonator>();
            
            BatteryRemaining = MaxBattery;
            PropellerSpeed = MinPropellerSpeed;
            
            #if !UNITY_EDITOR
            InstanceHelper.DroneHudController.SetArmedTextVisible(DroneDetonator.Armed);

            DroneSoundController sndController = gameObject.AddComponent<DroneSoundController>();
            sndController.enabled = true;
            #endif
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
            #if !UNITY_EDITOR
            InstanceHelper.DroneHudController.SetArmedTextVisible(DroneDetonator.Armed);
            #endif
        }

        public void ResetTransform()
        {
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
        }

        private void CreateGrenadeExplosion()
        {
            string profileId = InstanceHelper.LocalPlayer.ProfileId;
            
            ThrowWeapItemClass grenadeItem = Singleton<ItemFactoryClass>.Instance.CreateItem("5710c24ad2720bc3458b45a3", "543be6564bdc2df4348b4568", null) as ThrowWeapItemClass;
            Grenade grenade = Singleton<GameWorld>.Instance.GrenadeFactory.Create(
                gameObject.GetComponent<GrenadeSettings>(),
                RigidBody.position,
                Quaternion.identity,
                Vector3.zero,
                grenadeItem,
                profileId,
                0.1f,
                false,
                false
            );
            grenade.gameObject.transform.position = RigidBody.position;
            grenade.InvokeBlowUpEvent();
        }

        public void Detonate()
        {
            #if !UNITY_EDITOR
            DroneHelper.ControlDrone(false);

            if (DroneHelper.CurrentController == this)
            {
                DroneHelper.CurrentController = null;
            }

            CreateGrenadeExplosion();
            #endif
            
            Destroy(gameObject.transform.parent);
        }

        private void FixedUpdate()
        {
            Plugin.Logger.LogInfo(RigidBody.position);
            float thrustForce = ThrustForce * Thrust;

            Vector3 velocity = RigidBody.velocity;
            Vector3 upForce = transform.up * thrustForce;
            Vector3 counterForce = Vector3.zero;

            if (velocity.magnitude > MaxVelocity && Thrust > 0f)
            {
                Vector3 excess = velocity.normalized * (velocity.magnitude - MaxVelocity);
                counterForce = -excess * RigidBody.mass / Time.fixedDeltaTime;
            }

            RigidBody.AddForce(upForce + counterForce, ForceMode.Force);
            
            #if !UNITY_EDITOR
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
            #endif
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            #if !UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                DroneHelper.ControlDrone(false);
            }
            
            if (Input.GetKeyDown(KeyCode.K))
            {
                ToggleArmed();
            }
            #endif

            if (BatteryRemaining > 0)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    Thrust = Mathf.Lerp(Thrust, 1f, PropellerAccelerationSpeed * dt);
                }
                else
                {
                    Thrust = Mathf.Lerp(Thrust, 0f, PropellerAccelerationSpeed * dt);
                }

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

                Debug.Log(BatteryRemaining);
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
    }
}
