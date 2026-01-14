using EFT;
using EFT.Ballistics;
using FPVDroneModClient.Components.Drone;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Interface;
using UnityEngine;

namespace FPVDroneModClient.Components.Base
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
        public IArmable Armable;
        public IDetonatable Detonatable;
        
        public float PropellerSpeed = 0f;
        public float BatteryRemaining = 100f;
        public float SignalStrength = 1f;
        public bool Grounded = false;

        #if !UNITY_EDITOR
        protected void Awake()
        {
            GetReferences();

            DroneInput = gameObject.AddComponent<DroneInput>();
            DroneInput.enabled = false;

            BallisticCollider = GetComponentInChildren<BallisticCollider>(true);
            BallisticCollider.OnHitAction += OnHit;

            enabled = false;

            BotDroneListener.AddDrone(this);
        }

        protected virtual void Start()
        {
            GetReferences();
            
            BatteryRemaining = MaxBattery;
            PropellerSpeed = MinPropellerSpeed;

            if (RigidBody)
            {
                RigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                RigidBody.interpolation = RigidbodyInterpolation.Interpolate;
            }
            
            Canvas hudCanvas = HudController.GetComponent<Canvas>();
            hudCanvas.worldCamera = InstanceHelper.HudCamera;
            hudCanvas.planeDistance = 1f;
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
            Armable = GetComponentInChildren<IArmable>(true);
            Detonatable = GetComponentInChildren<IDetonatable>(true);
        }

        protected abstract void UpdateFromConfig();

        public virtual void OnPilotEnter()
        {
            if (!RigidBody || !DroneSoundController || !DroneInput || !BallisticCollider)
            {
                GetReferences();
            }
            
            Canvas hudCanvas = HudController.GetComponent<Canvas>();
            
            CameraBody.gameObject.SetActive(false);
            HudController.gameObject.SetActive(true);
            DroneInput.enabled = true;
            enabled = true;
            
            DroneSoundController.AudioSource.Play();
            HudController.gameObject.SetActive(true);
            hudCanvas.gameObject.layer = LayerMask.NameToLayer("UI");
            
            UpdateFromConfig();
        }

        public virtual void OnPilotExit()
        {
            if (!RigidBody || !DroneSoundController || !DroneInput || !BallisticCollider)
            {
                GetReferences();
            }
            
            CameraBody.gameObject.SetActive(true);
            HudController.gameObject.SetActive(false);
            DroneInput.enabled = false;
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
            
            if (HudController)
            {
                HudController.UpdateBatteryLevel(BatteryRemaining / MaxBattery);

                Player player = InstanceHelper.LocalPlayer;
                float distanceFromPlayer = (player.Position - transform.position).magnitude;
                float strength = Mathf.Clamp01(distanceFromPlayer / 1000f);
                HudController.UpdateSignalStrength(1f - strength);
                SignalStrength = strength; //TODO: give this functionality

                RaycastHit hit;
                HudController.UpdateAltitude(
                    Physics.Raycast(transform.position, Vector3.down, out hit, 999f, LayerMaskClass.HighPolyWithTerrainMask) ?
                    hit.distance : 999f
                );

                HudController.UpdateSpeed(RigidBody.velocity.magnitude * 3.6f);
            }
        }

        protected virtual void Update()
        {
            float dt = Time.deltaTime;
            if (!DroneInput)
            {
                DebugLogger.LogError("DRONEINPUT IS NULL");
            }

            if (BatteryRemaining > 0f)
            {
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

        protected void ApplyStableThrust()
        {
            if (!RigidBody) return;
            
            float gravityComp = -Physics.gravity.y;
            float verticalDamp = -RigidBody.velocity.y * 4f;

            RigidBody.AddForce(Vector3.up * (gravityComp + verticalDamp), ForceMode.Acceleration);
            Thrust = Mathf.Lerp(Thrust, gravityComp / ThrustForce, PropellerAccelerationSpeed * Time.fixedDeltaTime);
        }

        public void ResetTransform()
        {
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
        }
        
        private bool IsGrounded()
        {
            if (!RigidBody) return false;
            
            bool hit = VectorHelper.HitCheck(RigidBody.position, RigidBody.position + Vector3.down * 1000f, LayerMaskClass.HighPolyWithTerrainNoGrassMask,  out RaycastHit result);

            return hit && result.distance < 0.2f;
        }
        #endif
        
        #if UNITY_EDITOR
        public void OnPilotEnter()
        {
            return;
        }

        public void OnPilotExit()
        {
            return;
        }
        #endif
    }
}
