#if !UNITY_EDITOR
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using FPVDroneMod.Bots.Logic;
using FPVDroneMod.Components;

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

        public override bool IsActive()
        {
            return _droneListener.IsAnyDroneInThreatRange();
        }

        public override Action GetNextAction()
        {
            //return new Action(typeof(AttackDroneAction), "DroneInThreatRange");
            return new Action(typeof(EvadeDroneAction), "DroneDangerouslyClose");
        }

        public override bool IsCurrentActionEnding()
        {
            return false;
        }
    }
}
#endif