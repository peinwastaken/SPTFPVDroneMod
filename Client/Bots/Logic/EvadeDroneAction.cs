#if !UNITY_EDITOR
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using FPVDroneMod.Components;
using FPVDroneMod.Helpers;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace FPVDroneMod.Bots.Logic
{
    public class EvadeDroneAction : CustomLogic
    {
        private BotDroneListener _droneListener;
        private bool _canEvade = false;
        private bool _isEvading = false;
        private float _timeSinceLastEvade = 0f;
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
            BotOwner.SetPose(1f);
            BotOwner.BotLay.GetUp(true);
            BotOwner.SetTargetMoveSpeed(1f);
            BotOwner.Sprint(true);
            base.Stop();
        }

        public override void Stop()
        {
            BotOwner.Sprint(false);
            base.Stop();
        }
        
        public override void BuildDebugText(StringBuilder stringBuilder)
        {
            
        }

        public override void Update(CustomLayer.ActionData data)
        {
            _timeSinceLastEvade += Time.deltaTime;

            if (!_canEvade && !_isEvading && _timeSinceLastEvade > 1f)
            {
                _canEvade = true;
            }

            if (_canEvade && !_isEvading)
            {
                DebugLogger.LogInfo("pick new position");
                _isEvading = true;
                GetEvadePosition(out Vector3 position);
                BotOwner.Sprint(true);
                BotOwner.GoToPoint(position, false);
                _lastEvadePos = position;
                
                _canEvade = false;
                _timeSinceLastEvade = 0f;
            }

            if (_isEvading && Vector3.Distance(BotOwner.Position, _lastEvadePos) < 1f)
            {
                _isEvading = false;
                _droneListener.JustEvaded = true;
                Stop();
                DebugLogger.LogInfo("stopped evading");
            }
        }
    }
}
#endif