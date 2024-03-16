using System.Collections.Generic;
using System.Threading;
using Preparation.Interface;
using Preparation.Utility;
using System;
using GameClass.GameObj.Areas;
using System.Linq;
using GameClass.MapGenerator;

namespace GameClass.GameObj
{
    public partial class Map : IMap
    {
        private readonly Dictionary<GameObjType, LockedClassList<IGameObj>> gameObjDict;
        public Dictionary<GameObjType, LockedClassList<IGameObj>> GameObjDict => gameObjDict;
        private readonly uint height;
        public uint Height => height;
        private readonly uint width;
        public uint Width => width;
        public readonly PlaceType[,] protoGameMap;
        public PlaceType[,] ProtoGameMap => protoGameMap;

        #region 大本营相关
        public List<Home> Homes { get; }
        private readonly long currentHomeNum = 0;
        public bool TeamExists(long teamID)
        {
            return teamID < currentHomeNum;
        }
        #endregion

        #region GetPlaceType
        public PlaceType GetPlaceType(IGameObj obj)
        {
            try
            {
                var (x, y) = GameData.PosGridToCellXY(obj.Position);
                return protoGameMap[x, y];
            }
            catch
            {
                return PlaceType.Null;
            }
        }
        public PlaceType GetPlaceType(XY pos)
        {
            try
            {
                var (x, y) = GameData.PosGridToCellXY(pos);
                return protoGameMap[x, y];
            }
            catch
            {
                return PlaceType.Null;
            }
        }
        #endregion

        public bool IsOutOfBound(IGameObj obj)
        {
            return obj.Position.x >= GameData.MapLength - obj.Radius
                || obj.Position.x <= obj.Radius
                || obj.Position.y >= GameData.MapLength - obj.Radius
                || obj.Position.y <= obj.Radius;
        }
        public IOutOfBound GetOutOfBound(XY pos)
        {
            return new OutOfBoundBlock(pos);
        }

        public Ship? FindShipInID(long ID)
        {
            return (Ship?)GameObjDict[GameObjType.Ship].Find(gameObj => (ID == ((Ship)gameObj).ID));
        }
        public Ship? FindShipInShipID(long shipID)
        {
            return (Ship?)GameObjDict[GameObjType.Ship].Find(gameObj => (shipID == ((Ship)gameObj).ShipID));
        }

        public static bool WormholeInteract(Wormhole gameObj, XY Pos)
        {
            foreach (XY xy in gameObj.Grids)
            {
                if (GameData.ApproachToInteract(xy, Pos))
                    return true;
            }
            return false;
        }
        public GameObj? OneForInteract(XY Pos, GameObjType gameObjType)
        {
            return (GameObj?)GameObjDict[gameObjType].Find(gameObj =>
                ((GameData.ApproachToInteract(gameObj.Position, Pos)) ||
                (gameObjType == GameObjType.Wormhole && WormholeInteract((Wormhole)gameObj, Pos)))
                );
        }

        public GameObj? OneInTheSameCell(XY Pos, GameObjType gameObjType)
        {
            return (GameObj?)GameObjDict[gameObjType].Find(gameObj => (GameData.IsInTheSameCell(gameObj.Position, Pos)));
        }
        public GameObj? PartInTheSameCell(XY Pos, GameObjType gameObjType)
        {
            return (GameObj?)GameObjDict[gameObjType].Find(gameObj => (GameData.PartInTheSameCell(gameObj.Position, Pos)));
        }
        public GameObj? OneForInteractInACross(XY Pos, GameObjType gameObjType)
        {
            return (GameObj?)GameObjDict[gameObjType].Find(gameObj =>
                GameData.ApproachToInteractInACross(gameObj.Position, Pos));
        }
        public bool CanSee(Ship ship, GameObj gameObj)
        {
            XY pos1 = ship.Position;
            XY pos2 = gameObj.Position;
            XY del = pos1 - pos2;
            if (del * del > ship.ViewRange * ship.ViewRange)
                return false;
            if (del.x > del.y)
            {
                var beginx = GameData.PosGridToCellX(pos1) + GameData.NumOfPosGridPerCell;
                var endx = GameData.PosGridToCellX(pos2);
                if (GetPlaceType(pos1) == PlaceType.Shadow && GetPlaceType(pos2) == PlaceType.Shadow)
                {
                    for (int x = beginx; x < endx; x += GameData.NumOfPosGridPerCell)
                    {
                        if (GetPlaceType(pos1 + del * (x / del.x)) != PlaceType.Shadow)
                            return false;
                    }
                }
                else
                {
                    for (int x = beginx; x < endx; x += GameData.NumOfPosGridPerCell)
                    {
                        if (GetPlaceType(pos1 + del * (x / del.x)) == PlaceType.Ruin)
                            return false;
                    }
                }
            }
            else
            {
                var beginy = GameData.PosGridToCellY(pos1) + GameData.NumOfPosGridPerCell;
                var endy = GameData.PosGridToCellY(pos2);
                if (GetPlaceType(pos1) == PlaceType.Shadow && GetPlaceType(pos2) == PlaceType.Shadow)
                {
                    for (int y = beginy; y < endy; y += GameData.NumOfPosGridPerCell)
                    {
                        if (GetPlaceType(pos1 + del * (y / del.y)) != PlaceType.Shadow)
                            return false;
                    }
                }
                else
                {
                    for (int y = beginy; y < endy; y += GameData.NumOfPosGridPerCell)
                    {
                        if (GetPlaceType(pos1 + del * (y / del.y)) == PlaceType.Ruin)
                            return false;
                    }
                }
            }
            return true;
        }
        public bool Remove(GameObj gameObj)
        {
            GameObj? ans = (GameObj?)GameObjDict[gameObj.Type].RemoveOne(obj => gameObj.ID == obj.ID);
            if (ans != null)
            {
                ans.TryToRemove();
                return true;
            }
            return false;
        }
        public bool RemoveJustFromMap(GameObj gameObj)
        {
            if (GameObjDict[gameObj.Type].Remove(gameObj))
            {
                gameObj.TryToRemove();
                return true;
            }
            return false;
        }
        public void Add(IGameObj gameObj)
        {
            GameObjDict[gameObj.Type].Add(gameObj);
        }
        public Map(MapStruct mapResource)
        {
            gameObjDict = [];
            foreach (GameObjType idx in Enum.GetValues(typeof(GameObjType)))
            {
                if (idx != GameObjType.Null)
                    gameObjDict.TryAdd(idx, new LockedClassList<IGameObj>());
            }
            height = mapResource.height;
            width = mapResource.width;
            protoGameMap = mapResource.map;
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    switch (mapResource.map[i, j])
                    {
                        case PlaceType.Resource:
                            Add(new Resource(GameData.GetCellCenterPos(i, j)));
                            break;
                        case PlaceType.Construction:
                            Add(new Construction(GameData.GetCellCenterPos(i, j)));
                            break;
                        case PlaceType.Wormhole:
                            Func<Wormhole, bool> HasWormhole = (Wormhole wormhole) =>
                            {
                                if (wormhole.Grids.Contains(new XY(i, j)))
                                    return true;
                                foreach (XY xy in wormhole.Grids)
                                {
                                    if (Math.Abs(xy.x - i) <= 1 && Math.Abs(xy.y - j) <= 1)
                                    {
                                        wormhole.Grids.Add(new XY(i, j));
                                        return true;
                                    }
                                }
                                return false;
                            };

                            if (GameObjDict[GameObjType.Wormhole].Cast<Wormhole>().Find(wormhole => HasWormhole(wormhole)) == null)
                            {
                                List<XY> grids = [new XY(i, j)];
                                Add(new Wormhole(GameData.GetCellCenterPos(i, j), grids));
                            }
                            break;
                        case PlaceType.Home:
                            // 在生成地图时就把Home和TeamID获取, 生成Team时绑定上去即可
                            Add(new Home(GameData.GetCellCenterPos(i, j), currentHomeNum++));
                            break;
                    }
                }
            }
            Homes = GameObjDict[GameObjType.Home].Cast<Home>().ToNewList();
        }
    }
}