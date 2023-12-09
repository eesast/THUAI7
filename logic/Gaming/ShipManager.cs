using System.Threading;
using GameClass.GameObj;
using Preparation.Utility;
using Preparation.Interface;

namespace Gaming
{
    public partial class Game
    {
        private readonly ShipManager shipManager;
        private class ShipManager(Map gameMap)
        {
            readonly Map gameMap = gameMap;
            public Ship? AddShip(XY pos, long teamID, long shipID, ShipType shipType)
            {
                Ship newShip = new(pos, GameData.ShipRadius, shipType);
                gameMap.Add(newShip);
                newShip.TeamID.SetReturnOri(teamID);
                newShip.ShipID.SetReturnOri(shipID);
                return newShip;
            }
        }
    }
}
