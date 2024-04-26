using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protobuf;
using System;

public class RenderManager : SingletonMono<RenderManager>
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        DealFrame(CoreParam.frameQueue.GetValue(0));
        ShowFrame();
    }
    void DealFrame(MessageToClient info)
    {
        if (info.GameState == GameState.GameRunning)
        {
            foreach (MessageOfObj obj in info.ObjMessage)
            {
                DealObj(obj);
            }
        }
    }
    void DealObj(MessageOfObj obj)
    {
        switch (obj.MessageOfObjCase)
        {
            case MessageOfObj.MessageOfObjOneofCase.MapMessage:
                CoreParam.map = obj.MapMessage;
                break;
            case MessageOfObj.MessageOfObjOneofCase.ShipMessage:
                Debug.Log("receive shipmessage" + obj.ShipMessage);
                CoreParam.ships[obj.ShipMessage.TeamId * 4 + obj.ShipMessage.PlayerId] = obj.ShipMessage;
                break;
            case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
                CoreParam.bullets[obj.BulletMessage.Guid] = obj.BulletMessage;
                break;
            case MessageOfObj.MessageOfObjOneofCase.BombedBulletMessage:
                CoreParam.bombedBullets[obj.BombedBulletMessage.MappingId] = obj.BombedBulletMessage;
                break;
            case MessageOfObj.MessageOfObjOneofCase.HomeMessage:
                CoreParam.homes[obj.HomeMessage.TeamId] = obj.HomeMessage;
                break;
            case MessageOfObj.MessageOfObjOneofCase.TeamMessage:
                CoreParam.teams[obj.TeamMessage.TeamId] = obj.TeamMessage;
                break;
            case MessageOfObj.MessageOfObjOneofCase.ResourceMessage:
                CoreParam.resources[new Tuple<int, int>(obj.ResourceMessage.X, obj.ResourceMessage.Y)] = obj.ResourceMessage;
                break;
            case MessageOfObj.MessageOfObjOneofCase.WormholeMessage:
                break;
            case MessageOfObj.MessageOfObjOneofCase.CommunityMessage:
                break;
            case MessageOfObj.MessageOfObjOneofCase.FactoryMessage:
                break;
            case MessageOfObj.MessageOfObjOneofCase.FortMessage:
                break;
            case MessageOfObj.MessageOfObjOneofCase.NewsMessage:
                break;
            default:
                break;
        }
    }
    void ShowFrame()
    {
        if (!CoreParam.initialized)
        {
            ShowShip(CoreParam.ships);
            ShowMap(CoreParam.map);
            CoreParam.initialized = true;
        }
    }
    void ShowMap(MessageOfMap map)
    {
        for (int row = 1; row <= map.Height; row++)
        {
            for (int col = 1; col <= map.Width; col++)
            {
                ObjCreater.GetInstance().CreateObj(map.Rows[row].Cols[col], new Vector2(col, 50 - row));
                switch (map.Rows[row].Cols[col])
                {
                    case PlaceType.Home:
                        break;
                    case PlaceType.Space:
                        break;
                    case PlaceType.Shadow:
                        break;
                    case PlaceType.Construction:
                        break;
                    case PlaceType.Ruin:
                        break;
                    case PlaceType.Asteroid:
                        break;
                    case PlaceType.Resource:
                        break;
                    case PlaceType.Wormhole:
                        break;
                    default:
                        break;
                }
            }
        }
    }
    void ShowShip(MessageOfShip[] ships)
    {
        foreach (MessageOfShip ship in ships)
        {
            Debug.Log(ship);
            if (!CoreParam.shipsG[ship.TeamId * 4 + ship.PlayerId])
            {
                CoreParam.shipsG[ship.TeamId * 4 + ship.PlayerId] = ObjCreater.GetInstance().CreateObj(ship.ShipType, new Vector2(ship.X, ship.Y));

            }
        }
    }
}
