#if !UNITY_EDITOR
using Comfort.Common;
using EFT;
using FPVDroneModClient.Config;
using FPVDroneModClient.Helpers;
#endif
using EFT.Ballistics;
using EFT.InventoryLogic;
using FPVDroneModClient.Components.Drone;
using FPVDroneModClient.Interface;
using UnityEngine;
using UnityEngine.Serialization;

namespace FPVDroneModClient.Components.Base
{
    public abstract class BaseDroneController : MonoBehaviour, IPilotable, IOwnable
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

        public IPlayer Owner { get; set; }
        public DroneInput DroneInput;
        public DroneSoundController DroneSoundController;
        public BallisticCollider BallisticCollider;
        public DroneHudController HudController;
        public DroneSignalController SignalController;
        public Rigidbody RigidBody;
        public IArmable Armable;
        public IDetonatable Detonatable;
        public Item Item;

        public float PropellerSpeed = 0f;
        public float BatteryRemaining = 100f;
        public bool Grounded = false;
        public bool IsBeingControlled = false;
        public bool IsAboutToBeDestroyed = false;
        public bool IsInInventory = true;
        public bool WasJustDropped = false;
        
        #if !UNITY_EDITOR
        protected void Awake()
        {
            GetReferences();

            if (DroneInput == null)
            {
                DroneInput = gameObject.AddComponent<DroneInput>();
            }

            if (SignalController == null)
            {
                SignalController = gameObject.AddComponent<DroneSignalController>();
            }
            
            DroneInput.enabled = false;
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

            if (Detonatable is BasePayloadController basePayloadController)
            {
                basePayloadController.gameObject.layer = LayerMask.NameToLayer("Default");
            }

            Canvas hudCanvas = HudController.GetComponent<Canvas>();
            hudCanvas.worldCamera = InstanceHelper.HudCamera;
            hudCanvas.planeDistance = 1f;
        }

        protected virtual void GetReferences()
        {
            RigidBody = GetComponentInChildren<Rigidbody>(true);
            DroneSoundController = GetComponentInChildren<DroneSoundController>(true);
            DroneInput = GetComponentInChildren<DroneInput>(true);
            BallisticCollider = GetComponentInChildren<BallisticCollider>(true);
            Propellers = GetComponentsInChildren<DronePropeller>(true);
            HudController = GetComponentInChildren<DroneHudController>(true);
            Armable = GetComponentInChildren<IArmable>(true);
            Detonatable = GetComponentInChildren<IDetonatable>(true);
            SignalController = GetComponentInChildren<DroneSignalController>(true);

            if (Detonatable != null && Detonatable is BasePayloadController payloadController)
            {
                payloadController.Owner = Owner;
                payloadController.DroneController = this;
            }
            
            InitializeEvents();
        }

        protected void InitializeEvents()
        {
            if (BallisticCollider)
            {
                BallisticCollider.OnHitAction -= OnHit;
                BallisticCollider.OnHitAction += OnHit;
            }

            if (Armable != null)
            {
                Armable.OnToggleArmed -= OnToggleArmed;
                Armable.OnToggleArmed += OnToggleArmed;
            }

            if (Detonatable != null)
            {
                Detonatable.OnDetonate -= DestroyDrone;
                Detonatable.OnDetonate += DestroyDrone;
            }

            if (SignalController)
            {
                SignalController.OnSignalLost += () => DroneHelper.ControlDrone(false);
            }
        }

        protected abstract void UpdateFromConfig();

        public virtual void OnPilotEnter(bool isDoneLocally = true)
        {
            GetReferences();
            
            Canvas hudCanvas = HudController.GetComponent<Canvas>();

            if (isDoneLocally)
            {
                CameraBody.gameObject.SetActive(false);
                HudController.gameObject.SetActive(true);
                DroneInput.enabled = true;
                
                HudController.gameObject.SetActive(true);
                hudCanvas.gameObject.layer = LayerMask.NameToLayer("UI");
            }
            
            enabled = true;
            IsBeingControlled = true;
            DroneSoundController.AudioSource.Play();
            DroneSoundController.AudioSource.spatialBlend = isDoneLocally ? 0 : 1;
            DroneSoundController.AudioSource.outputAudioMixerGroup = Singleton<BetterAudio>.Instance.WorldMixer;

            WasJustDropped = false;

            UpdateFromConfig();
        }

        public virtual void OnPilotExit(bool isDoneLocally = true)
        {
            if (isDoneLocally)
            {
                CameraBody.gameObject.SetActive(true);
                HudController.gameObject.SetActive(false);
                DroneInput.enabled = false;
            }
            
            enabled = false;
            IsBeingControlled = false;
            DroneSoundController.AudioSource.Stop();
        }

        public virtual void OnToggleArmed(bool newValue)
        {

        }

        public void DestroyDrone()
        {
            if (IsAboutToBeDestroyed) return;
            IsAboutToBeDestroyed = true;

            if (DroneHelper.CurrentController == this)
            {
                DroneHelper.CurrentController = null;
                DroneHelper.ControlDrone(false);
            }

            BotDroneListener.RemoveDrone(this);
            InstanceHelper.LocalPlayer?.ClearInteractions();
            Destroy(gameObject);
        }

        public virtual void OnHit(DamageInfoStruct damageInfo)
        {
            IDetonatable detonatable = GetComponentInChildren<IDetonatable>(true);
            
            if (detonatable != null)
            {
                detonatable.Detonate();
                return;
            }
            
            DestroyDrone();
        }

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

            if (HudController && Owner.IsYourPlayer && IsBeingControlled)
            {
                HudController.UpdateBatteryLevel(BatteryRemaining / MaxBattery);

                Player player = InstanceHelper.LocalPlayer;
                float distanceFromPlayer = (player.Position - transform.position).magnitude;
                float distanceStrength = 1f - Mathf.Pow(Mathf.Clamp01(distanceFromPlayer / 1250f), 8);
                SignalController.DistanceStrength = distanceStrength;
                
                HudController.UpdateSignalStrength(SignalController.SignalStrength);
                InstanceHelper.UpdateNoiseAmount(1f - Mathf.Pow(SignalController.SignalStrength, 3));
                
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

            if (BatteryRemaining > 0f && SignalController.HasSignal)
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

            bool hit = VectorHelper.HitCheck(RigidBody.position, RigidBody.position + Vector3.down * 1000f, LayerMaskClass.HighPolyWithTerrainNoGrassMask, out RaycastHit result);

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
