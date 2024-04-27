using System;
using Preparation.Utility.Logging;

namespace GameClass.GameObj
{
    public static class LoggingFunctional
    {
        public static string ShipLogInfo(Ship ship)
                        => Logger.ObjInfo(typeof(Ship), $"{ship.TeamID} {ship.PlayerID}");
        public static string ShipLogInfo(long teamId, long shipId)
            => Logger.ObjInfo(typeof(Ship), $"{teamId} {shipId}");
        public static string BulletLogInfo(Bullet bullet)
        {
            try
            {
                if (bullet.Parent is null)
                    return Logger.ObjInfo(bullet, "null");
                return Logger.ObjInfo(bullet, ShipLogInfo((Ship)bullet.Parent));
            }
            catch
            {
                return Logger.ObjInfo(bullet, "null");
            }
        }
        public static string AutoLogInfo(object obj)
        {
            Type tp = obj.GetType();
            if (tp == typeof(Ship))
                return ShipLogInfo((Ship)obj);
            if (tp == typeof(Bullet))
                return BulletLogInfo((Bullet)obj);
            else
                return Logger.ObjInfo(obj);
        }
    }

    public static class ShipLogging
    {
        public static readonly Logger logger = new("Ship");
    }
}

namespace GameClass.GameObj.Map
{
    public static class MapLogging
    {
        public static readonly Logger logger = new("Map");
    }
}