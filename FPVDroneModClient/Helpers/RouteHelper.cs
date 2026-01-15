using System;
using FPVDroneModClient.Models;
using Newtonsoft.Json;
using SPT.Common.Http;
using UnityEngine;

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

    public static TankDeathState UpdateTankDeathState(bool isDead, string deathMap, Vector3 deathPos, Vector3 deathAng)
    {
        try
        {
            string route = "/fpv/tankstate/update";
            string data = JsonConvert.SerializeObject(new TankDeathState()
            {
                IsDead = isDead,
                DeathMap = deathMap,
                DeathPosition = deathPos,
                DeathAngle = deathAng
            });
            
            string response = RequestHandler.PostJson(route, data);
            TankDeathState state = JsonConvert.DeserializeObject<TankDeathState>(response);

            Plugin.TankDeathState = state;

            return state;
        }
        catch (Exception e)
        {
            DebugLogger.LogError(e.Message);
            throw;
        }
    }
}