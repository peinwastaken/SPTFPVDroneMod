#if !UNITY_EDITOR
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using FPVDroneMod.Bots.Logic;
using FPVDroneMod.Components;
using FPVDroneMod.Enum;
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

        public override bool IsActive()
        {
            return _droneListener.IsAnyDroneInThreatRange();
        }

        public override Action GetNextAction()
        {
            if (_droneListener.CurrentAction == EDroneCombatAction.AttackDrone)
            {
                return new Action(typeof(AttackDroneAction), "DroneIsCloseAndJustEvaded");
            }

            return new Action(typeof(EvadeDroneAction), "DroneIsClose");
        }

        public override void BuildDebugText(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"CurrentAction: {_droneListener.CurrentAction}");
            base.BuildDebugText(stringBuilder);
        }

        public override bool IsCurrentActionEnding()
        {
            if (_droneListener.HasActionChanged)
            {
                _droneListener.HasActionChanged = false;
                return true;
            }

            return false;
        }
    }
}
#endif