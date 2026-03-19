using FPVDroneModServer.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;

namespace FPVDroneModServer.Services
{
    [Injectable(InjectionType.Singleton)]
    public class TankDeathService(ISptLogger<TankDeathService> logger, ModHelper modHelper, JsonUtil jsonUtil)
    {
        private TankDeathState _tankDeathState = new TankDeathState
        {
            IsDead = false,
            DeathMap = "",
            DeathPosition = new Vector() {X = 0, Y = 0, Z = 0},
            DeathAngle = new Vector() {X = 0, Y = 0, Z = 0}
        };
        private string _configPath = "";
    
        public TankDeathState GetTankDeathState()
        {
            return _tankDeathState;
        }

        public void LoadTankStateConfig(string path, string fileName)
        {
            _tankDeathState = modHelper.GetJsonDataFromFile<TankDeathState>(path, fileName);
            _configPath = Path.Combine(path, fileName);
        }

        public TankDeathState SetTankState(bool isDead, string deathMap, Vector deathPosition, Vector deathAngle)
        {
            _tankDeathState.IsDead = isDead;
            _tankDeathState.DeathMap = deathMap;
            _tankDeathState.DeathPosition = deathPosition;
            _tankDeathState.DeathAngle = deathAngle;
            
            Save();

            return _tankDeathState;
        }

        private void Save()
        {
            logger.Info("Saved tank death state!");
            File.WriteAllText(_configPath, jsonUtil.Serialize(_tankDeathState));
        }
    }
}