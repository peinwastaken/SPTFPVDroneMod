using FPVDroneModClient.Components.Base;
using System.Collections.Generic;
using UnityEngine;

namespace FPVDroneModClient.Components
{
    public class ElectronicWarfareManager : MonoBehaviour
    {
        public static ElectronicWarfareManager Instance;
        
        public HashSet<ElectronicWarfareController> Controllers = [];
        public HashSet<BaseDroneController> DronesInJammers = [];

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void AddController(ElectronicWarfareController controller)
        {
            Controllers.Add(controller);
        }

        public void RemoveController(ElectronicWarfareController controller)
        {
            Controllers.Remove(controller);
        }

        private void Update()
        {
            DronesInJammers.Clear();
            
            foreach (ElectronicWarfareController ewc in Controllers)
            {
                foreach (BaseDroneController dc in ewc.DronesInRange)
                {
                    DronesInJammers.Add(dc);
                }
            }

            foreach (BaseDroneController dc in DronesInJammers)
            {
                dc.SignalController.JammingStrength = 0f;
            }

            foreach (ElectronicWarfareController ewc in Controllers)
            {
                foreach (BaseDroneController dc in ewc.DronesInRange)
                {
                    if (!dc) continue;

                    float minRange = ewc.MinRange;
                    float maxRange = ewc.MaxRange;
                    
                    float dist = (dc.transform.position - ewc.transform.position).magnitude;
                    float mult = (dist - minRange) / (maxRange - minRange);
                    float jamMult =  1f - Mathf.Clamp01(mult);

                    dc.SignalController.JammingStrength += jamMult;
                }    
            }
        }
    }
}
