using GameClass.GameObj;
using Preparation.Utility;
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
                gameMap.Add(newShip);
                newShip.TeamID.SetROri(teamID);
                newShip.PlayerID.SetROri(playerID);
                Debugger.Output(
                    "Added ship: " + newShip.ShipType + " with "
                    + newShip.ProducerModuleType + ", "
                    + newShip.ConstructorModuleType + ", "
                    + newShip.ArmorModuleType + ", "
                    + newShip.ShieldModuleType + ", "
                    + newShip.WeaponModuleType
                );
                return newShip;
            }
            public bool ActivateShip(Ship ship, XY pos)
            {
                var activateCost = ship.ShipType switch
                {
                    ShipType.CivilShip => GameData.CivilShipCost,
                    ShipType.WarShip => GameData.WarShipCost,
                    ShipType.FlagShip => GameData.FlagShipCost,
                    _ => int.MaxValue
                };
                if (activateCost > ship.MoneyPool.Money)
                {
                    return false;
                }
                if (ship.ShipState != ShipStateType.Deceased)
                {
                    return false;
                }
                game.TeamList[(int)ship.TeamID].MoneyPool.SubMoney(activateCost);
                ship.ReSetPos(pos);
                long stateNum = ship.SetShipState(RunningStateType.RunningActively, ShipStateType.Null);
                ship.ResetShipState(stateNum);
                ship.CanMove.SetROri(true);
                Debugger.Output(ship, " is activated!");
                return true;
            }
            public void BeAttacked(Ship ship, Bullet bullet)
            {
                Debugger.Output(ship, " is attacked!");
                Debugger.Output(bullet, $" 's AP is {bullet.AP}");
                if (bullet!.Parent!.TeamID == ship.TeamID)
                {
                    return;
                }
                long subHP = bullet.AP;
                if (bullet.TypeOfBullet != BulletType.Missile && ship.Shield > 0)
                {
                    ship.Shield.SubPositiveV((long)(subHP * bullet.ShieldModifier));
                    Debugger.Output(ship, $" 's shield is {ship.Shield}");
                }
                else if (ship.Armor > 0)
                {
                    ship.Armor.SubPositiveV((long)(subHP * bullet.ArmorModifier));
                    Debugger.Output(ship, $" 's armor is {ship.Armor}");
                }
                else
                {
                    ship.HP.SubPositiveV(subHP);
                    Debugger.Output(ship, $" 's HP is {ship.HP}");
                }
                if (ship.HP == 0)
                {
                    Debugger.Output(ship, " is destroyed!");
                    var money = ship.GetCost();
                    bullet.Parent.AddMoney(money);
                    Debugger.Output(bullet.Parent, $" get {money} money because of destroying {ship}");
                    Remove(ship);
                }
            }
            public void BeAttacked(Ship ship, long AP, long teamID)
            {
                Debugger.Output(ship, " is attacked!");
                Debugger.Output($"AP is {AP}");
                if (AP <= 0)
                {
                    return;
                }
                if (ship.Armor > 0)
                {
                    ship.Armor.SubPositiveV(AP);
                    Debugger.Output(ship, $" 's armor is {ship.Armor}");
                }
                else
                {
                    ship.HP.SubPositiveV(AP);
                    Debugger.Output(ship, $" 's HP is {ship.HP}");
                }
                if (ship.HP == 0)
                {
                    Debugger.Output(ship, " is destroyed!");
                    var money = ship.GetCost();
                    game.TeamList[(int)teamID].AddMoney(money);
                    Debugger.Output(ship, $" get {money} money because of destroying {ship}");
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
                    Debugger.Output(ship, $" is stunned for {time} ms");
                    Thread.Sleep(time);
                    ship.ResetShipState(stateNum);
                }
                )
                { IsBackground = true }.Start();
                return stateNum;
            }
            public bool BackSwing(Ship ship, int time)
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
                    Debugger.Output(ship, $" is swinging for {time} ms");
                    Thread.Sleep(time);
                    ship.ResetShipState(stateNum);
                }
                )
                { IsBackground = true }.Start();
                return true;
            }
            public bool Recover(Ship ship, long recover)
            {
                if (recover <= 0)
                {
                    return false;
                }
                if (ship.MoneyPool.Money < (ship.HP.GetMaxV() - ship.HP.GetValue()) * 1.2)
                {
                    return false;
                }
                long actualRecover = ship.HP.AddPositiveVRChange(recover);
                ship.SubMoney((long)(actualRecover * 1.2));
                return true;
            }
            public bool Recycle(Ship ship)
            {
                long shipValue = 0;
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
                shipValue += ship.ProducerModule.Cost;
                shipValue += ship.ConstructorModule.Cost;
                shipValue += ship.ArmorModule.Cost;
                shipValue += ship.ShieldModule.Cost;
                shipValue += ship.WeaponModule.Cost;
                Debugger.Output(ship, $" 's value is {shipValue}");
                ship.AddMoney((long)(shipValue * 0.5 * ship.HP / ship.HP.GetMaxV()));
                Debugger.Output(ship, " is recycled!");
                Remove(ship);
                return false;
            }
            public void Remove(Ship ship)
            {
                if (!ship.TryToRemoveFromGame(ShipStateType.Deceased))
                {
                    Debugger.Output(ship, " is not removed from game!");
                    return;
                }
                Debugger.Output(ship, " is removed from game!");
                gameMap.Remove(ship);
            }
        }
    }
}
