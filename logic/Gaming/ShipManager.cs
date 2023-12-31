﻿using System.Threading;
using GameClass.GameObj;
using Preparation.Utility;

namespace Gaming
{
    public partial class Game
    {
        private readonly ShipManager shipManager;
        private class ShipManager(Map gameMap)
        {
            readonly Map gameMap = gameMap;
            public Ship? AddShip(XY pos, long teamID, long shipID, ShipType shipType, MoneyPool moneyPool)
            {
                Ship newShip = new(pos, GameData.ShipRadius, shipType, moneyPool);
                gameMap.Add(newShip);
                newShip.TeamID.SetReturnOri(teamID);
                newShip.ShipID.SetReturnOri(shipID);
                return newShip;
            }

            public void BeAttacked(Ship ship, Bullet bullet)
            {
                if (bullet!.Parent!.TeamID == ship.TeamID)
                {
                    return;
                }
                long subHP = bullet.AP;
                if (bullet.TypeOfBullet != BulletType.Missile && ship.Shield > 0)
                {
                    ship.Shield.SubPositiveV((long)(subHP * bullet.ShieldModifier));
                }
                else if (ship.Armor > 0)
                {
                    ship.Armor.SubPositiveV((long)(subHP * bullet.ArmorModifier));
                }
                else
                {
                    ship.HP.SubPositiveV(subHP);
                }
                if (ship.HP == 0)
                {
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
                long actualRecover = ship.HP.AddPositiveV(recover);
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
                ship.AddMoney((long)(shipValue * 0.5 * ship.HP / ship.HP.GetMaxV()));
                Remove(ship);
                return false;
            }
            public void Remove(Ship ship)
            {
                if (!ship.TryToRemoveFromGame(ShipStateType.Deceased))
                {
                    return;
                }
                gameMap.Remove(ship); // TODO
            }
        }
    }
}
