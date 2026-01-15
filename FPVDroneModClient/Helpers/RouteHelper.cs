using System;
using FPVDroneModClient.Models;
using Newtonsoft.Json;
using SPT.Common.Http;

namespace FPVDroneModClient.Helpers;

public static class RouteHelper
{
    public static TankDeathState FetchTankDeathState()
    {
        try
        {
            string route = "/fpv/tankstate";
            string response = RequestHandler.GetJson(route);
            DebugLogger.LogInfo(response.ToString());
            return JsonConvert.DeserializeObject<TankDeathState>(response);
        }
        catch (Exception e)
        {
            DebugLogger.LogError(e.Message);
            return null;
        }
    }
}