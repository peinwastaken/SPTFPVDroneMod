#if !UNITY_EDITOR
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using FPVDroneMod.Components;
using FPVDroneMod.Enum;
using FPVDroneMod.Helpers;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace FPVDroneMod.Bots.Logic
{
    public class EvadeDroneAction : CustomLogic
    {
        private BotDroneListener _droneListener;
        private bool _canStartEvade = true;
        private float _timeSpentEvading = 0f;
        private float _maxEvadeTime = 3f;
        private bool _isEvading = false;
        private Vector3 _lastEvadePos;
        
        public EvadeDroneAction(BotOwner botOwner) : base(botOwner)
        {
            _droneListener = botOwner.GetComponent<BotDroneListener>();
        }
        
        private void GetEvadePosition(out Vector3 position)
        {
            position = BotOwner.GetPlayer.Transform.position; // default fallback

            if (!_droneListener.ClosestDroneData.Controller)
            {
                DebugLogger.LogWarning("no controller");
            }

            DroneController controller = _droneListener.ClosestDroneData.Controller;
            Vector3 botPos = BotOwner.GetPlayer.Transform.position;
            
            Vector3 dirToBot = (botPos - controller.transform.position).normalized;
            Vector2 dirToBotFlat = new Vector2(dirToBot.x, dirToBot.z);
            
            float perpendicularMult = Random.value < 0.5f ? -1f : 1f;
            Vector2 perpendicular = Vector2.Perpendicular(dirToBotFlat) * perpendicularMult;
            
            Vector2 evadeDir = (perpendicular + dirToBotFlat * 0.3f).normalized;
            Vector3 targetPos = botPos + new Vector3(evadeDir.x, 0, evadeDir.y) * 10f;
            
            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                position = hit.position;
            }
        }
        
        public override void Start()
        {
            DebugLogger.LogInfo("start evade action");
            BotOwner.SetPose(1f);
            BotOwner.BotLay.GetUp(true);
            BotOwner.SetTargetMoveSpeed(1f);
            BotOwner.Sprint(true);
            base.Start();
        }

        public override void Stop()
        {
            DebugLogger.LogInfo("start evade action");
            BotOwner.Sprint(false);
            _canStartEvade = true;
            _isEvading = false;
            _timeSpentEvading = 0f;
            _lastEvadePos = Vector3.up * 999f;
            base.Stop();
        }
        
        public override void BuildDebugText(StringBuilder stringBuilder)
        {
            
        }

        public override void Update(CustomLayer.ActionData data)
        {
            if (_canStartEvade)
            {
                DebugLogger.LogInfo("pick new position");
                GetEvadePosition(out Vector3 position);
                BotOwner.Sprint(true);
                BotOwner.GoToPoint(position, false);
                
                _canStartEvade = false;
                _lastEvadePos = position;
                _isEvading = true;
            }

            if (_isEvading)
            {
                _timeSpentEvading += Time.deltaTime;
            }

            if (Vector3.Distance(BotOwner.Position, _lastEvadePos) < 2f || _timeSpentEvading > _maxEvadeTime)
            {
                DebugLogger.LogInfo("stopped evading");
                _droneListener.SetAction(EDroneCombatAction.AttackDrone);
            }
        }
    }
}
#endif