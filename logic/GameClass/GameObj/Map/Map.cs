﻿using System.Collections.Generic;
using System.Threading;
using Preparation.Interface;
using Preparation.Utility;
using System;
using GameClass.GameObj.Areas;
using System.Linq;

namespace GameClass.GameObj
{
    public partial class Map : IMap
    {
        private Dictionary<GameObjType, IList<IGameObj>> gameObjDict;
        public Dictionary<GameObjType, IList<IGameObj>> GameObjDict => gameObjDict;
        private Dictionary<GameObjType, ReaderWriterLockSlim> gameObjLockDict;
        public Dictionary<GameObjType, ReaderWriterLockSlim> GameObjLockDict => gameObjLockDict;
        public readonly uint[,] protoGameMap;
        public uint[,] ProtoGameMap => protoGameMap;
        public PlaceType GetPlaceType(IGameObj obj)
        {
            try
            {
                return (PlaceType)protoGameMap[obj.Position.x / GameData.NumOfPosGridPerCell, obj.Position.y / GameData.NumOfPosGridPerCell];
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
                return (PlaceType)protoGameMap[pos.x / GameData.NumOfPosGridPerCell, pos.y / GameData.NumOfPosGridPerCell];
            }
            catch
            {
                return PlaceType.Null;
            }
        }
        public bool IsOutOfBound(IGameObj obj)
        {
            return obj.Position.x >= GameData.MapLength - obj.Radius || obj.Position.x <= obj.Radius || obj.Position.y >= GameData.MapLength - obj.Radius || obj.Position.y <= obj.Radius;
        }
        public IOutOfBound GetOutOfBound(XY pos)
        {
            return new Areas.OutOfBoundBlock(pos);
        }
        public Ship? FindShipInID(long ID)
        {
            Ship? ship = null;
            gameObjLockDict[GameObjType.Ship].EnterReadLock();
            try
            {
                foreach (Ship s in gameObjDict[GameObjType.Ship])
                {
                    if (s.ID == ID)
                    {
                        ship = s;
                        break;
                    }
                }
            }
            finally
            {
                gameObjLockDict[GameObjType.Ship].ExitReadLock();
            }
            return ship;
        }
        public Ship? FindShipInShipID(long shipID)
        {
            Ship? ship = null;
            gameObjLockDict[GameObjType.Ship].EnterReadLock();
            try
            {
                foreach (Ship s in gameObjDict[GameObjType.Ship])
                {
                    if (s.ShipID == shipID)
                    {
                        ship = s;
                        break;
                    }
                }
            }
            finally
            {
                gameObjLockDict[GameObjType.Ship].ExitReadLock();
            }
            return ship;
        }
        public GameObj? OneForInteract(XY Pos, GameObjType gameObjType)
        {
            GameObj? GameObjForInteract = null;
            GameObjLockDict[gameObjType].EnterReadLock();
            try
            {
                foreach (GameObj gameObj in GameObjDict[gameObjType])
                {
                    if (gameObjType == GameObjType.Wormhole)
                    {
                        bool flag = false;
                        foreach (XY xy in ((Wormhole)gameObj).Grids)
                        {
                            if (GameData.ApproachToInteract(xy, Pos))
                            {
                                GameObjForInteract = gameObj;
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (GameData.ApproachToInteract(gameObj.Position, Pos))
                        {
                            GameObjForInteract = gameObj;
                            break;
                        }
                    }
                }
            }
            finally
            {
                GameObjLockDict[gameObjType].ExitReadLock();
            }
            return GameObjForInteract;
        }
        public GameObj? OneInTheSameCell(XY Pos, GameObjType gameObjType)
        {
            GameObj? GameObjForInteract = null;
            GameObjLockDict[gameObjType].EnterReadLock();
            try
            {
                foreach (GameObj gameObj in GameObjDict[gameObjType])
                {
                    if (GameData.IsInTheSameCell(gameObj.Position, Pos))
                    {
                        GameObjForInteract = gameObj;
                        break;
                    }
                }
            }
            finally
            {
                GameObjLockDict[gameObjType].ExitReadLock();
            }
            return GameObjForInteract;
        }
        public GameObj? PartInTheSameCell(XY Pos, GameObjType gameObjType)
        {
            GameObj? GameObjForInteract = null;
            GameObjLockDict[gameObjType].EnterReadLock();
            try
            {
                foreach (GameObj gameObj in GameObjDict[gameObjType])
                {
                    if (GameData.PartInTheSameCell(gameObj.Position, Pos))
                    {
                        GameObjForInteract = gameObj;
                        break;
                    }
                }
            }
            finally
            {
                GameObjLockDict[gameObjType].ExitReadLock();
            }
            return GameObjForInteract;
        }
        public GameObj? OneForInteractInACross(XY Pos, GameObjType gameObjType)
        {
            GameObj? GameObjForInteract = null;
            GameObjLockDict[gameObjType].EnterReadLock();
            try
            {
                foreach (GameObj gameObj in GameObjDict[gameObjType])
                {
                    if (GameData.ApproachToInteractInACross(gameObj.Position, Pos))
                    {
                        GameObjForInteract = gameObj;
                        break;
                    }
                }
            }
            finally
            {
                GameObjLockDict[gameObjType].ExitReadLock();
            }
            return GameObjForInteract;
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
                if (GetPlaceType(pos1) == PlaceType.Shadow && GetPlaceType(pos2) == PlaceType.Shadow)
                {
                    for (int x = GameData.PosGridToCellX(pos1) + GameData.NumOfPosGridPerCell; x < GameData.PosGridToCellX(pos2); x += GameData.NumOfPosGridPerCell)
                    {
                        if (GetPlaceType(pos1 + del * (x / del.x)) != PlaceType.Shadow)
                            return false;
                    }
                }
                else
                {
                    for (int x = GameData.PosGridToCellX(pos1) + GameData.NumOfPosGridPerCell; x < GameData.PosGridToCellX(pos2); x += GameData.NumOfPosGridPerCell)
                    {
                        if (GetPlaceType(pos1 + del * (x / del.x)) == PlaceType.Ruin)
                            return false;
                    }
                }
            }
            else
            {
                if (GetPlaceType(pos1) == PlaceType.Shadow && GetPlaceType(pos2) == PlaceType.Shadow)
                {
                    for (int y = GameData.PosGridToCellY(pos1) + GameData.NumOfPosGridPerCell; y < GameData.PosGridToCellY(pos2); y += GameData.NumOfPosGridPerCell)
                    {
                        if (GetPlaceType(pos1 + del * (y / del.y)) != PlaceType.Shadow)
                            return false;
                    }
                }
                else
                {
                    for (int y = GameData.PosGridToCellY(pos1) + GameData.NumOfPosGridPerCell; y < GameData.PosGridToCellY(pos2); y += GameData.NumOfPosGridPerCell)
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
            GameObj? ToDel = null;
            GameObjLockDict[gameObj.Type].EnterWriteLock();
            try
            {
                foreach (GameObj obj in GameObjDict[gameObj.Type])
                {
                    if (gameObj.ID == obj.ID)
                    {
                        ToDel = obj;
                        break;
                    }
                }
                if (ToDel != null)
                {
                    GameObjDict[gameObj.Type].Remove(ToDel);
                    ToDel.TryToRemove();
                }
            }
            finally
            {
                GameObjLockDict[gameObj.Type].ExitWriteLock();
            }
            return ToDel != null;
        }
        public bool RemoveJustFromMap(GameObj gameObj)
        {
            GameObjLockDict[gameObj.Type].EnterWriteLock();
            try
            {
                if (GameObjDict[gameObj.Type].Remove(gameObj))
                {
                    gameObj.TryToRemove();
                    return true;
                }
                return false;
            }
            finally
            {
                GameObjLockDict[gameObj.Type].ExitWriteLock();
            }
        }
        public void Add(IGameObj gameObj)
        {
            GameObjLockDict[gameObj.Type].EnterWriteLock();
            try
            {
                GameObjDict[gameObj.Type].Add(gameObj);
            }
            finally
            {
                GameObjLockDict[gameObj.Type].ExitWriteLock();
            }
        }
        public Map(uint[,] mapResource)
        {
            gameObjDict = [];
            gameObjLockDict = [];
            foreach (GameObjType idx in Enum.GetValues(typeof(GameObjType)))
            {
                if (idx != GameObjType.Null)
                {
                    gameObjDict.Add(idx, new List<IGameObj>());
                    gameObjLockDict.Add(idx, new ReaderWriterLockSlim());
                }
            }
            protoGameMap = new uint[mapResource.GetLength(0), mapResource.GetLength(1)];
            Array.Copy(mapResource, protoGameMap, mapResource.Length);
            for (int i = 0; i < GameData.MapRows; ++i)
            {
                for (int j = 0; j < GameData.MapCols; ++j)
                {
                    bool hasWormhole = false;
                    switch ((PlaceType)mapResource[i, j])
                    {
                        case PlaceType.Resource:
                            Add(new Resource(GameData.GetCellCenterPos(i, j)));
                            break;
                        case PlaceType.Construction:
                            Add(new Construction(GameData.GetCellCenterPos(i, j)));
                            break;
                        case PlaceType.Wormhole:
                            foreach (Wormhole wormhole in GameObjDict[GameObjType.Wormhole].Cast<Wormhole>())
                            {
                                if (wormhole.Grids.Contains(new XY(i, j)))
                                {
                                    hasWormhole = true;
                                    break;
                                }
                                else
                                {
                                    foreach (XY xy in wormhole.Grids)
                                    {
                                        if (Math.Abs(xy.x - i) <= 1 && Math.Abs(xy.y - j) <= 1)
                                        {
                                            wormhole.Grids.Add(new XY(i, j));
                                            hasWormhole = true;
                                            break;
                                        }
                                    }
                                    if (hasWormhole)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (!hasWormhole)
                            {
                                List<XY> grids = [new XY(i, j)];
                                Add(new Wormhole(GameData.GetCellCenterPos(i, j), grids));
                            }
                            break;
                        case PlaceType.Home:
                            Add(new Home(GameData.GetCellCenterPos(i, j)));
                            break;
                    }
                }
            }
        }
    }
}