using EFT.Ballistics;
using FPVDroneModClient.Config;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Interface;
using UnityEngine;

namespace FPVDroneModClient.Components
{
    public abstract class BaseDroneController : MonoBehaviour, IPilotable
    {
        public float Thrust;
        
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

        public Transform CameraBody;
        public Transform CameraPos;

        public DronePropeller[] Propellers;

        public DroneInput DroneInput;
        public DroneSoundController DroneSoundController;
        public BallisticCollider BallisticCollider;
        public DroneHudController HudController;
        public Rigidbody RigidBody;

        public float PropellerSpeed;
        public float BatteryRemaining;
        public bool Grounded;

        #if !UNITY_EDITOR
        protected void Awake()
        {
            GetReferences();

            DroneInput = gameObject.AddComponent<DroneInput>();
            DroneInput.enabled = false;

            BallisticCollider = GetComponentInChildren<BallisticCollider>(true);
            BallisticCollider.OnHitAction += OnHit;
            
            Canvas hudCanvas = HudController.gameObject.GetComponent<Canvas>();
            hudCanvas.worldCamera = CameraClass.Instance.Camera;
            hudCanvas.planeDistance = 0.55f;

            BotDroneListener.AddDrone(this);
        }
        
        protected virtual void GetReferences()
        {
            DebugLogger.LogInfo("something was missing, get references");

            RigidBody = GetComponentInChildren<Rigidbody>(true);
            DroneSoundController = GetComponentInChildren<DroneSoundController>(true);
            DroneInput = GetComponentInChildren<DroneInput>(true);
            BallisticCollider = GetComponentInChildren<BallisticCollider>(true);
            Propellers = GetComponentsInChildren<DronePropeller>(true);
            HudController = GetComponentInChildren<DroneHudController>(true);
        }
        
        protected void UpdateFromConfig()
        {
            RigidBody.mass = DroneConfig.DroneMass.Value;
            RigidBody.angularDrag = 15f;

            ThrustForce = DroneConfig.DroneThrustForce.Value;
            MaxVelocity = DroneConfig.DroneMaxVelocity.Value;

            PitchSpeed = DroneConfig.DronePitchSpeed.Value;
            YawSpeed = DroneConfig.DroneYawSpeed.Value;
            RollSpeed = DroneConfig.DroneRollSpeed.Value;

            PropellerAccelerationSpeed = DroneConfig.DronePropellerAccelerationSpeed.Value;
            MinPropellerSpeed = DroneConfig.DroneMinPropellerSpeed.Value;
            MaxPropellerSpeed = DroneConfig.DroneMaxPropellerSpeed.Value;

            MaxBattery = DroneConfig.DroneMaxBattery.Value;
            BatteryDecayRateIdle = DroneConfig.DroneBatteryDecayIdle.Value;
            BatteryDecayRateAccel = DroneConfig.DroneBatteryDecayAccel.Value;
        }

        public virtual void OnPilotEnter()
        {
            if (!RigidBody || !DroneSoundController || !DroneInput || !BallisticCollider)
            {
                GetReferences();
            }
            
            CameraBody.gameObject.SetActive(false);
            DroneInput.enabled = true;
            HudController.gameObject.SetActive(true);
            enabled = true;
            
            DroneSoundController.AudioSource.Play();
            HudController.gameObject.SetActive(true);

            UpdateFromConfig();
        }

        public virtual void OnPilotExit()
        {
            if (!RigidBody || !DroneSoundController || !DroneInput || !BallisticCollider)
            {
                GetReferences();
            }
            
            CameraBody.gameObject.SetActive(true);
            DroneInput.enabled = false;
            HudController.gameObject.SetActive(false);
            enabled = false;
            
            DroneSoundController.AudioSource.Stop();
        }

        public abstract void OnHit(DamageInfoStruct damageInfo);

        protected void RotatePropellers(float amount)
        {
            foreach (DronePropeller propeller in Propellers)
            {
                propeller.Rotate(Vector3.right, amount);
            }
        }

        public abstract void ApplyPitch(float amount);

        public abstract void ApplyYaw(float amount);

        public abstract void ApplyRoll(float amount);
        
        public abstract void ApplyThrust(float amount);

        protected virtual void FixedUpdate()
        {
            Grounded = IsGrounded();
        }

        protected void ApplyStableThrust()
        {
            float gravityComp = -Physics.gravity.y;
            float verticalDamp = -RigidBody.velocity.y * 4f;

            RigidBody.AddForce(Vector3.up * (gravityComp + verticalDamp), ForceMode.Acceleration);
        }

        public void ResetTransform()
        {
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
        }
        
        private bool IsGrounded()
        {
            if (!RigidBody) return false;
            
            bool hit = VectorHelper.HitCheck(RigidBody.position, Vector3.down * 1f, LayerMaskClass.HighPolyWithTerrainNoGrassMask);

            return !hit && RigidBody.velocity.sqrMagnitude >= 0.2f;
        }
        #endif
        
        #if UNITY_EDITOR
        public void OnPilotEnter()
        {
            throw new System.NotImplementedException();
        }

        public void OnPilotExit()
        {
            throw new System.NotImplementedException();
        }
        #endif
    }
}
