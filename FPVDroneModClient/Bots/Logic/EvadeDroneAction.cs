using DrakiaXYZ.BigBrain.Brains;
using EFT;
using FPVDroneModClient.Components;
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Enum;
using FPVDroneModClient.Helpers;
using UnityEngine;
using UnityEngine.AI;

namespace FPVDroneModClient.Bots.Logic
{
    public class EvadeDroneAction : CustomLogic
    {
        private readonly BotDroneListener _droneListener;
        private readonly float _maxEvadeTime = 5f;
        private bool _canStartEvade = true;
        private bool _isEvading;
        private Vector3 _lastEvadePos;
        private float _timeSpentEvading;

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

            BaseDroneController controller = _droneListener.ClosestDroneData.Controller;
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
        }

        public override void Stop()
        {
            DebugLogger.LogInfo("stop evade action");
            BotOwner.Sprint(false);
            _canStartEvade = true;
            _isEvading = false;
            _timeSpentEvading = 0f;
            _lastEvadePos = Vector3.up * 999f;
        }

        public override void Update(CustomLayer.ActionData data)
        {
            if (_canStartEvade)
            {
                DebugLogger.LogInfo("pick new position");
                GetEvadePosition(out Vector3 position);
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

