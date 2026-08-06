using Comfort.Common;
using FPVDroneModClient.Config;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Items;
using EFT;
using EFT.Ballistics;
using EFT.InventoryLogic;
using FPVDroneModClient.Components.Drone;
using FPVDroneModClient.Interface;
using UnityEngine;

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
        public Item Battery;
        public ResourceComponent BatteryResource;
        public BatteryController BatteryController;

        public float PropellerSpeed = 0f;
        public bool Grounded = false;
        public bool IsBeingControlled = false;
        public bool IsAboutToBeDestroyed = false;
        public bool IsInInventory = true;
        public bool WasJustDropped = false;
        
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

        public virtual void GetReferences()
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
            BatteryController = GetComponentInChildren<BatteryController>(true);

            if (Detonatable is BasePayloadController payloadController)
            {
                payloadController.Owner = Owner;
                payloadController.DroneController = this;
            }

            if (BatteryController)
            {
                BatteryController.DroneController = this;
            }

            if (Item != null)
            {
                DroneItem droneItem = (DroneItem)Item;
                Slot batterySlot = droneItem.GetBatterySlot();
                if (batterySlot.ContainedItem is BatteryItem item)
                {
                    Battery = item;
                    BatteryResource = item.ResourceComponent;
                }
            }
            
            InitializeEvents();
        }

        public void FixColliderLayers()
        {
            GameObject droneBody = transform.Find("DroneBody")?.gameObject;
            
            if (droneBody)
            {
                droneBody.layer = LayerMask.NameToLayer("Deadbody");
            }
            
            if (BatteryController && BatteryController.BallisticCollider)
            {
                BatteryController.BallisticCollider.gameObject.layer = LayerMask.NameToLayer("Deadbody");
            }

            if (Detonatable is BasePayloadController payloadController && payloadController.BallisticCollider)
            {
                payloadController.BallisticCollider.gameObject.layer = LayerMask.NameToLayer("Deadbody");
            }
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
            DroneSoundController.AudioSource.volume = GeneralConfig.DroneAudioVolume.Value;
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

        public virtual void OnHit(DamageInfo damageInfo)
        {
            IDetonatable detonatable = GetComponentInChildren<IDetonatable>(true);
            
            if (detonatable is BasePayloadController payload)
            { 
                payload.OnHit(damageInfo);
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
                if (BatteryResource != null)
                {
                    HudController.UpdateBatteryLevel(BatteryResource.RelativeValue);
                }

                Player player = InstanceHelper.LocalPlayer;
                float distanceFromPlayer = (player.Position - transform.position).magnitude;
                float distanceStrength = 1f - Mathf.Pow(Mathf.Clamp01(distanceFromPlayer / 1250f), 8);
                SignalController.DistanceStrength = distanceStrength;
                
                HudController.UpdateSignalStrength(SignalController.SignalStrength);
                InstanceHelper.UpdateNoiseAmount(1f - Mathf.Pow(SignalController.SignalStrength, 3));
                
                RaycastHit hit;
                HudController.UpdateAltitude(
                    Physics.Raycast(transform.position, Vector3.down, out hit, 999f, LayersMaskController.HighPolyWithTerrainMask) ?
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
            
            if (BatteryResource != null && BatteryResource.Value > 0f && SignalController.HasSignal)
            {
                float speedTarget = Mathf.Lerp(MinPropellerSpeed, MaxPropellerSpeed, Thrust);
                PropellerSpeed = Mathf.Lerp(PropellerSpeed, speedTarget, PropellerAccelerationSpeed * dt);

                BatteryResource.Value -= (Thrust > 0 ? BatteryDecayRateAccel : BatteryDecayRateIdle) * dt;
                BatteryResource.Value = Mathf.Clamp(BatteryResource.Value, 0, BatteryResource.MaxResource);
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

            bool hit = VectorHelper.HitCheck(RigidBody.position, RigidBody.position + Vector3.down * 1000f, LayersMaskController.HighPolyWithTerrainNoGrassMask, out RaycastHit result);

            return hit && result.distance < 0.2f;
        }

    }
}

