using GameClass.GameObj.Areas;
using GameClass.MapGenerator;
using Preparation.Interface;
using Preparation.Utility;
using Preparation.Utility.Value;
using Preparation.Utility.Value.SafeValue;
using System;
using System.Collections.Generic;

namespace GameClass.GameObj.Map
{
    public partial class Map : IMap
    {
        private readonly Dictionary<GameObjType, LockedClassList<IGameObj>> gameObjDict;
        public Dictionary<GameObjType, LockedClassList<IGameObj>> GameObjDict => gameObjDict;
        private readonly List<Wormhole> wormholes = [];
        private readonly uint height;
        public uint Height => height;
        private readonly uint width;
        public uint Width => width;
        public readonly PlaceType[,] protoGameMap;
        public PlaceType[,] ProtoGameMap => protoGameMap;

        // xfgg说：爱因斯坦说，每个坐标系都有与之绑定的时钟，(x, y, z, ict) 构成四维时空坐标，在洛伦兹变换下满足矢量性（狗头）
        private readonly MyTimer timer = new();
        public IMyTimer Timer => timer;

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
        public Ship? FindShipInPlayerID(long teamID, long playerID)
        {
            return (Ship?)GameObjDict[GameObjType.Ship].Find(gameObj => (teamID == ((Ship)gameObj).TeamID) && playerID == ((Ship)gameObj).PlayerID);
        }
        public GameObj? OneForInteract(XY Pos, GameObjType gameObjType)
        {
            return (GameObj?)GameObjDict[gameObjType].Find(gameObj => GameData.ApproachToInteract(gameObj.Position, Pos));
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
        public GameObj? OneInTheRange(XY Pos, int range, GameObjType gameObjType)
        {
            return (GameObj?)GameObjDict[gameObjType].Find(gameObj =>
                GameData.IsInTheRange(gameObj.Position, Pos, range));
        }
        public List<Ship>? ShipInTheRangeNotTeamID(XY Pos, int range, long teamID)
        {
            return GameObjDict[GameObjType.Ship].Cast<Ship>()?.FindAll(ship =>
                (GameData.IsInTheRange(ship.Position, Pos, range) && ship.TeamID != teamID));
        }
        public List<Ship>? ShipInTheList(List<CellXY> PosList)
        {
            return GameObjDict[GameObjType.Ship].Cast<Ship>()?.FindAll(ship =>
                PosList.Contains(GameData.PosGridToCellXY(ship.Position)));
        }
        public List<Ship>? ShipInTheList(List<WormholeCell> ObjList)
        {
            return GameObjDict[GameObjType.Ship].Cast<Ship>()?.FindAll(ship =>
            {
                foreach (var obj in ObjList)
                {
                    if (GameData.IsInTheSameCell(ship.Position, obj.Position))
                        return true;
                }
                return false;
            });
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
            MapLogging.logger.ConsoleLogDebug($"Add {gameObj.Type} at {gameObj.Position}");
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
                        case PlaceType.Ruin:
                            Add(new Ruin(GameData.GetCellCenterPos(i, j)));
                            break;
                        case PlaceType.Shadow:
                            Add(new Shadow(GameData.GetCellCenterPos(i, j)));
                            break;
                        case PlaceType.Asteroid:
                            Add(new Asteroid(GameData.GetCellCenterPos(i, j)));
                            break;
                        case PlaceType.Resource:
                            Add(new Resource(GameData.GetCellCenterPos(i, j)));
                            break;
                        case PlaceType.Construction:
                            Add(new Construction(GameData.GetCellCenterPos(i, j)));
                            break;
                        case PlaceType.Wormhole:
                            Func<Wormhole, bool> WormholeHasCell = (Wormhole wormhole) =>
                            {
                                if (wormhole.Cells.Find(cell => GameData.PosGridToCellXY(cell.Position) == new CellXY(i, j)) != null)
                                    return true;
                                foreach (WormholeCell cell in wormhole.Cells)
                                {
                                    var xy = GameData.PosGridToCellXY(cell.Position);
                                    if (Math.Abs(xy.x - i) <= 1 && Math.Abs(xy.y - j) <= 1)
                                    {
                                        var newCell = new WormholeCell(GameData.GetCellCenterPos(i, j), wormhole);
                                        Add(newCell);
                                        wormhole.Cells.Add(newCell);
                                        return true;
                                    }
                                }
                                return false;
                            };

                            if (wormholes.Find(wormhole => WormholeHasCell(wormhole)) == null)
                            {
                                var newWormhole = new Wormhole([], wormholes.Count);
                                var newCell = new WormholeCell(GameData.GetCellCenterPos(i, j), newWormhole);
                                Add(newCell);
                                newWormhole.Cells.Add(newCell);
                                wormholes.Add(newWormhole);
                            }
                            break;
                        case PlaceType.Home:
                            // 在生成地图时就把Home和TeamID获取, 生成Team时绑定上去即可
                            Add(new Home(GameData.GetCellCenterPos(i, j), currentHomeNum++));
                            break;
                    }
                }
            }
            Homes = GameObjDict[GameObjType.Home].Cast<Home>()?.ToNewList()!;
            for (var i = 0; i < wormholes.Count; i++)
            {
                if (i != wormholes.Count / 2)
                {
                    wormholes[i].HP.SetRNow(0);
                }
            }
        }
    }
}