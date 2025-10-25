using System.Collections.Generic;

namespace FPVDroneMod.Models
{
    public class PlayerExplosionData
    {
        public List<EBodyPart> ProcessedLimbs = [];
        public Dictionary<BodyPartCollider, float> BodyPartColliders = [];
        
        private List<EBodyPart> _fracturableLimbs = [EBodyPart.LeftArm, EBodyPart.RightArm, EBodyPart.LeftLeg, EBodyPart.RightLeg];
        
        public EBodyPart GetClosestBodyPart()
        {
            EBodyPart bodyPart = EBodyPart.Chest;
            float distance = float.MaxValue;

            foreach (KeyValuePair<BodyPartCollider, float> kvp in BodyPartColliders)
            {
                if (kvp.Value < distance)
                {
                    bodyPart = kvp.Key.BodyPartType;
                    distance = kvp.Value;
                }
            }

            return bodyPart;
        }
        
        public EBodyPart GetClosestFracturableBodyPart()
        {
            EBodyPart bodyPart = EBodyPart.Chest;
            float distance = float.MaxValue;

            foreach (KeyValuePair<BodyPartCollider, float> kvp in BodyPartColliders)
            {
                if (kvp.Value < distance && _fracturableLimbs.Contains(kvp.Key.BodyPartType))
                {
                    bodyPart = kvp.Key.BodyPartType;
                    distance = kvp.Value;
                }
            }

            return bodyPart;
        }
    }
}
