#if !UNITY_EDITOR
using EFT;
using FPVDroneMod.Enum;
using FPVDroneMod.Globals;
using FPVDroneMod.Helpers;
using FPVDroneMod.Models;
using System.Collections.Generic;
using UnityEngine;

namespace FPVDroneMod.Components
{
    public class BotDroneListener : MonoBehaviour
    {
        public static List<BaseDroneController> ActiveDrones = [];

        public float TimeBetweenChecks = 1f;
        public float TimeSinceLastDroneCheck;
        public EDroneCombatAction CurrentAction = EDroneCombatAction.EvadeDrone;
        public bool HasActionChanged;

        public Player Player;
        public ClosestDroneData ClosestDroneData;

        public void Awake()
        {
            Player = GetComponentInParent<Player>();
        }

        public void Update()
        {
            TimeSinceLastDroneCheck += Time.deltaTime;

            if (TimeSinceLastDroneCheck >= TimeBetweenChecks)
            {
                GetClosestDroneInThreatRange(out ClosestDroneData closestDrone);
                ClosestDroneData = closestDrone;
                TimeSinceLastDroneCheck = 0f;
            }
        }

        public static void AddDrone(BaseDroneController controller)
        {
            if (ActiveDrones.Count <= 0)
            {
                ActiveDrones.Add(controller);
            }

            foreach (BaseDroneController addedController in ActiveDrones)
            {
                if (addedController == controller)
                {
                    DebugLogger.LogInfo("Drone already added to global list");
                    return;
                }
            }

            ActiveDrones.Add(controller);
        }

        public static void RemoveDrone(BaseDroneController controller)
        {
            ActiveDrones.Remove(controller);
        }

        public Dictionary<BaseDroneController, float> GetDroneDistances()
        {
            Dictionary<BaseDroneController, float> distances = [];
            Vector3 position = gameObject.transform.position;

            foreach (BaseDroneController controller in ActiveDrones)
            {
                if (controller)
                {
                    distances.Add(controller, Vector3.Distance(position, controller.transform.position));
                }
            }

            return distances;
        }

        public ClosestDroneData GetClosestDrone()
        {
            Dictionary<BaseDroneController, float> droneDistances = GetDroneDistances();

            BaseDroneController closestDrone = null;
            float closestDistance = float.MaxValue;

            foreach (KeyValuePair<BaseDroneController, float> kvp in droneDistances)
            {
                if (kvp.Value < closestDistance)
                {
                    closestDrone = kvp.Key;
                    closestDistance = kvp.Value;
                }
            }

            return new ClosestDroneData
            {
                Controller = closestDrone,
                Distance = closestDistance
            };
        }

        public bool GetClosestDroneInThreatRange(out ClosestDroneData closestDrone)
        {
            closestDrone = GetClosestDrone();

            if (closestDrone.Controller && closestDrone.Distance < BotGlobals.DroneHearRange)
            {
                return true;
            }

            return false;
        }

        public bool IsDroneThreatActive()
        {
            if (!ClosestDroneData?.Controller) return false;

            bool isInRange = ClosestDroneData.Distance < BotGlobals.DroneHearRange;
            bool isVisible = IsClosestDroneVisible();

            return isInRange && isVisible;
        }

        public void SetAction(EDroneCombatAction action)
        {
            CurrentAction = action;
            HasActionChanged = true;
        }

        public bool IsClosestDroneVisible()
        {
            BaseDroneController controller = ClosestDroneData?.Controller;

            if (controller == null || controller.RigidBody == null) return false;

            return ClosestDroneData?.Controller && VectorHelper.VisCheck(Player.MainParts[BodyPartType.head].Position, ClosestDroneData.Controller.RigidBody.position, LayerMaskClass.HighPolyCollider);
        }
    }
}
#endif
