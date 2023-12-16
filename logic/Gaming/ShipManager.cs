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
            public Ship? AddShip(XY pos, long teamID, long shipID, ShipType shipType)
            {
                Ship newShip = new(pos, GameData.ShipRadius, shipType);
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
                switch (bullet.TypeOfBullet)
                {
                    case BulletType.Laser:
                        if (ship.Shield > 0)
                        {
                            ship.Shield.SubPositiveV((long)(subHP * GameData.LaserShieldModifier));
                        }
                        else if (ship.Armor > 0)
                        {
                            ship.Armor.SubPositiveV((long)(subHP * GameData.LaserArmorModifier));
                        }
                        else
                        {
                            ship.HP.SubPositiveV(subHP);
                        }
                        break;
                    case BulletType.Plasma:
                        if (ship.Shield > 0)
                        {
                            ship.Shield.SubPositiveV((long)(subHP * GameData.PlasmaShieldModifier));
                        }
                        else if (ship.Armor > 0)
                        {
                            ship.Armor.SubPositiveV((long)(subHP * GameData.PlasmaArmorModifier));
                        }
                        else
                        {
                            ship.HP.SubPositiveV(subHP);
                        }
                        break;
                    case BulletType.Shell:
                        if (ship.Shield > 0)
                        {
                            ship.Shield.SubPositiveV((long)(subHP * GameData.ShellShieldModifier));
                        }
                        else if (ship.Armor > 0)
                        {
                            ship.Armor.SubPositiveV((long)(subHP * GameData.ShellArmorModifier));
                        }
                        else
                        {
                            ship.HP.SubPositiveV(subHP);
                        }
                        break;
                    case BulletType.Missile:
                        if (ship.Armor > 0)
                        {
                            ship.Armor.SubPositiveV((long)(subHP * GameData.MissileArmorModifier));
                        }
                        else
                        {
                            ship.HP.SubPositiveV(subHP);
                        }
                        break;
                    case BulletType.Arc:
                        if (ship.Shield > 0)
                        {
                            ship.Shield.SubPositiveV((long)(subHP * GameData.ArcShieldModifier));
                        }
                        else if (ship.Armor > 0)
                        {
                            ship.Armor.SubPositiveV((long)(subHP * GameData.ArcArmorModifier));
                        }
                        else
                        {
                            ship.HP.SubPositiveV(subHP);
                        }
                        break;
                    default:
                        break;
                }
                if (ship.HP == 0)
                {
                    Remove(ship);
                }
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
