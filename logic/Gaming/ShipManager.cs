using GameClass.GameObj;
using GameClass.GameObj.Map;
using Preparation.Utility;
using Preparation.Utility.Logging;
using Preparation.Utility.Value;
using System.Threading;

namespace Gaming
{
    public partial class Game
    {
        private readonly ShipManager shipManager;
        private class ShipManager(Game game, Map gameMap)
        {
            private readonly Game game = game;
            private readonly Map gameMap = gameMap;
            public Ship? AddShip(long teamID, long playerID, ShipType shipType, MoneyPool moneyPool)
            {
                Ship newShip = new(GameData.ShipRadius, shipType, moneyPool);
                newShip.TeamID.SetROri(teamID);
                newShip.PlayerID.SetROri(playerID);
                ShipManagerLogging.logger.ConsoleLogDebug(
                    LoggingFunctional.ShipLogInfo(newShip)
                    + " created");
                ShipManagerLogging.logger.ConsoleLogDebug(
                    $"Added ship: {newShip.ShipType} with "
                    + $"{newShip.ProducerModuleType}, "
                    + $"{newShip.ConstructorModuleType}, "
                    + $"{newShip.ArmorModuleType}, "
                    + $"{newShip.ShieldModuleType}, "
                    + $"{newShip.WeaponModuleType}");
                return newShip;
            }
            public bool ActivateShip(Ship ship, XY pos)
            {
                if (ship.ShipState != ShipStateType.Deceased)
                {
                    return false;
                }
                gameMap.Add(ship);
                ship.ReSetPos(pos);
                ship.SetShipState(RunningStateType.Null, ShipStateType.Null);
                ShipManagerLogging.logger.ConsoleLogDebug(
                    LoggingFunctional.ShipLogInfo(ship)
                    + " is activated!");
                return true;
            }
            public void BeAttacked(Ship ship, Bullet bullet)
            {
                ShipManagerLogging.logger.ConsoleLogDebug(
                    LoggingFunctional.ShipLogInfo(ship)
                    + " is attacked!");
                ShipManagerLogging.logger.ConsoleLogDebug(
                    LoggingFunctional.BulletLogInfo(bullet)
                    + $" 's AP is {bullet.AP}");
                ShipManagerLogging.logger.ConsoleLogDebug(
                    Logger.ObjInfo(typeof(Base), bullet.Parent!.TeamID.ToString())
                    + " is the attacker's TeamID");
                ShipManagerLogging.logger.ConsoleLogDebug(
                    Logger.ObjInfo(typeof(Base), ship.TeamID.ToString())
                    + " is the attacked's TeamID");
                if (bullet!.Parent!.TeamID.Get() == ship.TeamID.Get())
                {
                    return;
                }
                long subHP = bullet.AP;
                if (bullet.TypeOfBullet != BulletType.Missile && ship.Shield > 0)
                {
                    ship.Shield.SubPositiveV((long)(subHP * bullet.ShieldModifier));
                    ShipManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.ShipLogInfo(ship)
                        + $" 's shield is {ship.Shield}");
                }
                else if (ship.Armor > 0)
                {
                    ship.Armor.SubPositiveV((long)(subHP * bullet.ArmorModifier));
                    ShipManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.ShipLogInfo(ship)
                        + $" 's armor is {ship.Armor}");
                }
                else
                {
                    ship.HP.SubPositiveV(subHP);
                    ShipManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.ShipLogInfo(ship)
                        + $" 's HP is {ship.HP}");
                }
                if (ship.HP == 0)
                {
                    ShipManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.ShipLogInfo(ship)
                        + " is destroyed!");
                    var money = (long)(ship.GetCost() * 0.2);
                    bullet.Parent.AddMoney(money);
                    ShipManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.ShipLogInfo((Ship)bullet.Parent)
                        + $" get {money} money because of destroying "
                        + LoggingFunctional.ShipLogInfo(ship));
                    Remove(ship);
                }
            }
            public void BeAttacked(Ship ship, long AP, long teamID)
            {
                ShipManagerLogging.logger.ConsoleLogDebug(
                    LoggingFunctional.ShipLogInfo(ship)
                    + " is attacked!");
                ShipManagerLogging.logger.ConsoleLogDebug($"AP is {AP}");
                if (AP <= 0)
                {
                    return;
                }
                if (ship.Shield > 0)
                {
                    ship.Shield.SubPositiveV(AP);
                    ShipManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.ShipLogInfo(ship)
                        + $" 's shield is {ship.Shield}");
                }
                else if (ship.Armor > 0)
                {
                    ship.Armor.SubPositiveV(AP);
                    ShipManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.ShipLogInfo(ship)
                        + $" 's armor is {ship.Armor}");
                }
                else
                {
                    ship.HP.SubPositiveV(AP);
                    ShipManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.ShipLogInfo(ship)
                        + $" 's HP is {ship.HP}");
                }
                if (ship.HP == 0)
                {
                    ShipManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.ShipLogInfo(ship)
                        + " is destroyed!");
                    var money = ship.GetCost();
                    var team = game.TeamList[(int)teamID];
                    team.AddMoney(money);
                    ShipManagerLogging.logger.ConsoleLogDebug(
                        Logger.ObjInfo(typeof(Base), teamID.ToString())
                        + $" get {money} money because of destroying "
                        + LoggingFunctional.ShipLogInfo(ship));
                    Remove(ship);
                }
            }
            public static long BeStunned(Ship ship, int time)
            {
                long stateNum = ship.SetShipState(RunningStateType.RunningForcibly, ShipStateType.Stunned);
                if (stateNum == -1)
                {
                    return -1;
                }
                new Thread
                (() =>
                {
                    ShipManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.ShipLogInfo(ship)
                        + $" is stunned for {time} ms");
                    Thread.Sleep(time);
                    ship.ResetShipState(stateNum);
                }
                )
                { IsBackground = true }.Start();
                return stateNum;
            }
            public static bool BackSwing(Ship ship, int time)
            {
                if (time <= 0)
                {
                    return false;
                }
                long stateNum = ship.SetShipState(RunningStateType.RunningForcibly, ShipStateType.Swinging);
                if (stateNum == -1)
                {
                    return false;
                }
                new Thread
                (() =>
                {
                    ShipManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.ShipLogInfo(ship)
                        + $" is swinging for {time} ms");
                    Thread.Sleep(time);
                    ship.ResetShipState(stateNum);
                }
                )
                { IsBackground = true }.Start();
                return true;
            }
            public static bool Recover(Ship ship, long recover)
            {
                if (recover <= 0)
                {
                    return false;
                }
                return ship.MoneyPool.Money.SubVLimitedByAddingOtherRChange(recover, ship.HP, 1.2) > 0;
            }
            public bool Recycle(Ship ship)
            {
                long shipValue =
                    ship.ProducerModule.Get().Cost + ship.ConstructorModule.Get().Cost +
                    ship.ArmorModule.Get().Cost + ship.ShieldModule.Get().Cost + ship.WeaponModule.Get().Cost;
                switch (ship.ShipType)
                {
                    case ShipType.CivilShip:
                        shipValue += GameData.CivilShipCost;
                        break;
                    case ShipType.WarShip:
                        shipValue += GameData.WarShipCost;
                        break;
                    case ShipType.FlagShip:
                        shipValue += GameData.FlagShipCost;
                        break;
                    default:
                        return false;
                }
                ShipManagerLogging.logger.ConsoleLogDebug(
                    LoggingFunctional.ShipLogInfo(ship)
                    + $" 's value is {shipValue}");
                ship.AddMoney((long)(shipValue * 0.5 * ship.HP.GetDivideValueByMaxV()));
                ShipManagerLogging.logger.ConsoleLogDebug(
                    LoggingFunctional.ShipLogInfo(ship)
                    + " is recycled!");
                Remove(ship);
                return false;
            }
            public void Remove(Ship ship)
            {
                if (!ship.TryToRemoveFromGame(ShipStateType.Deceased))
                {
                    ShipManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.ShipLogInfo(ship)
                        + " hasn't been removed from game!");
                    return;
                }
                ShipManagerLogging.logger.ConsoleLogDebug(
                    LoggingFunctional.ShipLogInfo(ship)
                    + " hasn been removed from game!");
                gameMap.Remove(ship);
            }
        }
    }
}
