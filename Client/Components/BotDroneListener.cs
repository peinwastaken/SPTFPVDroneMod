#if !UNITY_EDITOR
using FPVDroneMod.Helpers;
using FPVDroneMod.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FPVDroneMod.Components
{
    public class BotDroneListener : MonoBehaviour
    {
        public static List<DroneController> ActiveDrones = [];

        public float TimeSinceLastDroneCheck = 0f;
        public float TimeBetweenChecks = 1f;
        public float DroneThreatDistance = 40f;
        public ClosestDroneData ClosestDroneData;
        public Vector3 ClosestDroneMoveDirection = Vector3.zero;
        public float ScaredFactor = 0f;
        
        public static void AddDrone(DroneController controller)
        {
            if (ActiveDrones.Count <= 0)
            {
                ActiveDrones.Add(controller);
            }

            foreach (DroneController addedController in ActiveDrones)
            {
                if (addedController == controller)
                {
                    DebugLogger.LogInfo("Drone already added to global list");
                    return;
                }
            }
            
            ActiveDrones.Add(controller);
        }

        public static void RemoveDrone(DroneController controller)
        {
            ActiveDrones.Remove(controller);
        }

        public Dictionary<DroneController, float> GetDroneDistances()
        {
            Dictionary<DroneController, float> distances = [];
            Vector3 position = gameObject.transform.position;

            foreach (DroneController controller in ActiveDrones)
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
            Dictionary<DroneController, float> droneDistances = GetDroneDistances();

            DroneController closestDrone = null;
            float closestDistance = float.MaxValue;

            foreach (KeyValuePair<DroneController, float> kvp in droneDistances)
            {
                if (kvp.Value < closestDistance)
                {
                    closestDrone = kvp.Key;
                    closestDistance = kvp.Value;
                }
            }

            return new ClosestDroneData()
            {
                Controller = closestDrone,
                Distance = closestDistance
            };
        }

        public bool GetClosestDroneInThreatRange(out ClosestDroneData closestDrone)
        {
            closestDrone = GetClosestDrone();

            if (closestDrone.Controller && closestDrone.Distance < DroneThreatDistance)
            {
                return true;
            }

            return false;
        }
        
        public bool IsAnyDroneInThreatRange()
        {
            return ClosestDroneData.Controller && ClosestDroneData.Distance < DroneThreatDistance;
        }

        public Vector3 GetClosestDroneDirection()
        {
            if (ClosestDroneData.Controller != null)
            {
                ClosestDroneMoveDirection = ClosestDroneData.Controller.RigidBody.velocity.normalized;
            }

            return ClosestDroneMoveDirection;
        }

        public void CalculateScaredFactor()
        {
            if (ClosestDroneData.Controller != null)
            {
                Rigidbody rb = ClosestDroneData.Controller.RigidBody;
                Vector3 velocity = rb.velocity;
                Vector3 lookDir = rb.transform.forward;
            }
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
    }
}
#endif