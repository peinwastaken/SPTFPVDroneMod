using FPVDroneModServer.Models;
using FPVDroneModServer.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;

namespace FPVDroneModServer.Routes
{
    [Injectable]
    public class TankStateRouter(
        ISptLogger<TankStateRouter> logger,
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
    public class TankStateRouterCallback(ISptLogger<TankStateRouterCallback> logger, TankDeathService tankDeathService, HttpResponseUtil responseUtil)
    {
        public ValueTask<string> GetTankState()
        {
            TankDeathState state = tankDeathService.GetTankDeathState();
            return new ValueTask<string>(responseUtil.GetBody(state));
        }

        public ValueTask<string> UpdateTankState(TankStateRequestData info)
        {
            TankDeathState newState = tankDeathService.SetTankState(info.isDead, info.deathMap, info.deathPosition, info.deathAngle);
            return new ValueTask<string>(responseUtil.GetBody(newState));
        }
    }

    public class TankStateRequestData : IRequestData
    {
        public bool isDead { get; set; }
        public string deathMap { get; set; }
        public Vector deathPosition { get; set; }
        public Vector deathAngle { get; set; }
    }
}
