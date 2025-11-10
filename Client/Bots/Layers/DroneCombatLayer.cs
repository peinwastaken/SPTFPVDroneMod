#if !UNITY_EDITOR
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using FPVDroneMod.Bots.Logic;
using FPVDroneMod.Components;
using System.Text;

namespace FPVDroneMod.Bots.Layers
{
    public class DroneCombatLayer : CustomLayer
    {
        private BotDroneListener _droneListener;
        
        public DroneCombatLayer(BotOwner botOwner, int priority) : base(botOwner, priority)
        {
            _droneListener = botOwner.GetPlayer.gameObject.AddComponent<BotDroneListener>();
        }

        public override string GetName()
        {
            return "DroneCombatLayer";
        }

        public override void BuildDebugText(StringBuilder stringBuilder)
        {
            
        }

        public override bool IsActive()
        {
            return _droneListener.IsAnyDroneInThreatRange();
        }

        public override Action GetNextAction()
        {
            if (_droneListener.JustEvaded)
            {
                return new Action(typeof(AttackDroneAction), "DroneInThreatRangeAndJustEvaded");
            }
            
            return new Action(typeof(EvadeDroneAction), "DroneDangerouslyClose");
        }

        public override bool IsCurrentActionEnding()
        {
            return false;
        }
    }
}
#endif