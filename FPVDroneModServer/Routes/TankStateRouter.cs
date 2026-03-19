using FPVDroneModServer.Models;
using FPVDroneModServer.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;

namespace FPVDroneModServer.Routes
{
    [Injectable]
    public class TankStateRouter(
        JsonUtil jsonUtil,
        TankStateRouterCallback callback) : StaticRouter(jsonUtil, [
            new RouteAction(
                "/fpv/tankstate",
                async (url, info, sessionId, output) => await callback.GetTankState()
            ),
            new RouteAction<TankStateRequestData>(
                "/fpv/tankstate/update",
                async (url, info, sessionId, output) => await callback.UpdateTankState(info)
            )
        ])
    {}

    [Injectable]
    public class TankStateRouterCallback(JsonUtil jsonUtil, TankDeathService tankDeathService)
    {
        public ValueTask<string> GetTankState()
        {
            TankDeathState state = tankDeathService.GetTankDeathState();
            return new ValueTask<string>(jsonUtil.Serialize(state) ?? "{}");
        }

        public ValueTask<string> UpdateTankState(TankStateRequestData info)
        {
            TankDeathState newState = tankDeathService.SetTankState(info.isDead, info.deathMap, info.deathPosition, info.deathAngle);
            return new ValueTask<string>(jsonUtil.Serialize(newState) ?? "{}");
        }
    }

    public class TankStateRequestData : IRequestData
    {
        public required bool isDead { get; set; }
        public required string deathMap { get; set; } 
        public required Vector deathPosition { get; set; }
        public required Vector deathAngle { get; set; }
    }
}
