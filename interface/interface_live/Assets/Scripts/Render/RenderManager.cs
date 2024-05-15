using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protobuf;
using System;
using Newtonsoft.Json;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;
using TMPro;
using System.Linq;
using UnityEngine.Rendering;

public class RenderManager : SingletonMono<RenderManager>
{
    // int cnt = 0;
    bool callTimeOver = false;
    public TextMeshProUGUI gameTime, score, energy, fi;
    JsonSerializerSettings jSetting = new JsonSerializerSettings
    {

        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
    };
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(updateFrame());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator updateFrame()
    {
        if (CoreParam.frameQueue.GetSize() < 50)
        {
            StartCoroutine(CalTimems(25));
            fi.text = "fi: " + 25;
        }
        else
        {
            while (CoreParam.frameQueue.GetSize() > 100)
            {
                CoreParam.frameQueue.GetValue();
            }
            StartCoroutine(CalTimems(1250 / CoreParam.frameQueue.GetSize()));
            fi.text = "fi: " + (1250 / CoreParam.frameQueue.GetSize());
        }
        if (!CoreParam.initialized && CoreParam.firstFrame != null)
        {
            DealFrame(CoreParam.firstFrame);
            ShowFrame();
        }
        CoreParam.currentFrame = CoreParam.frameQueue.GetValue();
        if (CoreParam.currentFrame != null)
        {
            DealFrame(CoreParam.currentFrame);
            ShowFrame();
        }
        while (!callTimeOver)
            yield return 0;
        StartCoroutine(updateFrame());
    }
    IEnumerator CalTimems(int count)
    {
        callTimeOver = false;
        yield return new WaitForSeconds((float)count / 1000);
        callTimeOver = true;
    }
    void DealFrame(MessageToClient info)
    {
        CoreParam.bullets.Clear();
        CoreParam.ships.Clear();
        foreach (MessageOfObj obj in info.ObjMessage)
        {
            DealObj(obj);
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
                CoreParam.ships[obj.ShipMessage.TeamId * 4 + obj.ShipMessage.PlayerId - 1] = obj.ShipMessage;
                break;
            case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
                CoreParam.bullets[obj.BulletMessage.Guid] = obj.BulletMessage;
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
                CoreParam.wormholes[new Tuple<int, int>(obj.WormholeMessage.X, obj.WormholeMessage.Y)] = obj.WormholeMessage;
                break;
            case MessageOfObj.MessageOfObjOneofCase.CommunityMessage:
                CoreParam.communities[new Tuple<int, int>(obj.CommunityMessage.X, obj.CommunityMessage.Y)] = obj.CommunityMessage;
                break;
            case MessageOfObj.MessageOfObjOneofCase.FactoryMessage:
                CoreParam.factories[new Tuple<int, int>(obj.FactoryMessage.X, obj.FactoryMessage.Y)] = obj.FactoryMessage;
                break;
            case MessageOfObj.MessageOfObjOneofCase.FortMessage:
                CoreParam.forts[new Tuple<int, int>(obj.FortMessage.X, obj.FortMessage.Y)] = obj.FortMessage;
                Debug.Log("fort");
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
            ShowMap(CoreParam.map);
            ShowAllMessage(CoreParam.firstFrame);
            CoreParam.initialized = true;
        }
        else
        {
            ShowShip(CoreParam.ships);
            ShowBullet(CoreParam.bullets);
            ShowFactory(CoreParam.factories);
            ShowCommunity(CoreParam.communities);
            ShowFort(CoreParam.forts);
            ShowAllMessage(CoreParam.currentFrame);
        }
    }
    void ShowMap(MessageOfMap map)
    {
        for (int row = 0; row < map.Height; row++)
            for (int col = 0; col < map.Width; col++)
                ObjCreater.GetInstance().CreateObj(map.Rows[row].Cols[col], Tool.GetInstance().CellToUxy(row, col));
    }
    void ShowShip(Dictionary<long, MessageOfShip> ships)
    {
        foreach (KeyValuePair<long, MessageOfShip> ship in ships)
        {
            if (ship.Value != null)
            {
                if (!CoreParam.shipsG.ContainsKey(ship.Key))
                {
                    CoreParam.shipsG[ship.Value.TeamId * 4 + ship.Value.PlayerId - 1] =
                        ObjCreater.GetInstance().CreateObj(ship.Value.ShipType,
                            Tool.GetInstance().GridToUxy(ship.Value.X, ship.Value.Y));
                    RendererControl.GetInstance().SetColToChild((PlayerTeam)(ship.Value.TeamId + 1),
                        CoreParam.shipsG[ship.Value.TeamId * 4 + ship.Value.PlayerId - 1].transform);
                }
                else
                {
                    CoreParam.shipsG[ship.Value.TeamId * 4 + ship.Value.PlayerId - 1].transform.position =
                        Tool.GetInstance().GridToUxy(ship.Value.X, ship.Value.Y);
                    CoreParam.shipsG[ship.Value.TeamId * 4 + ship.Value.PlayerId - 1].transform.rotation =
                        Quaternion.AngleAxis((float)ship.Value.FacingDirection * Mathf.Rad2Deg + 180, Vector3.forward);
                }
            }
        }
        for (int i = 0; i < CoreParam.shipsG.Count; i++)
        {
            KeyValuePair<long, GameObject> shipG = CoreParam.shipsG.ElementAt(i);
            if (shipG.Value != null)
            {
                if (!CoreParam.ships.ContainsKey(shipG.Key))
                {
                    Destroy(shipG.Value);
                    CoreParam.bulletsG.Remove(shipG.Key);
                }
            }
        }
    }
    void ShowBullet(Dictionary<long, MessageOfBullet> bullets)
    {
        foreach (KeyValuePair<long, MessageOfBullet> bullet in bullets)
        {
            if (bullet.Value != null)
            {
                if (!CoreParam.bulletsG.ContainsKey(bullet.Key))
                {
                    CoreParam.bulletsG[bullet.Key] =
                        ObjCreater.GetInstance().CreateObj(bullet.Value.Type,
                            Tool.GetInstance().GridToUxy(bullet.Value.X, bullet.Value.Y),
                            Quaternion.AngleAxis((float)bullet.Value.FacingDirection * Mathf.Rad2Deg + 180, Vector3.forward));
                    RendererControl.GetInstance().SetColToChild((PlayerTeam)(bullet.Value.TeamId + 1),
                        CoreParam.bulletsG[bullet.Key].transform);
                    // CoreParam.bulletsG[bullet.Key]

                }
                else
                {
                    CoreParam.bulletsG[bullet.Key].transform.position = Tool.GetInstance().GridToUxy(bullet.Value.X, bullet.Value.Y);
                    CoreParam.bulletsG[bullet.Key].transform.rotation =
                        Quaternion.AngleAxis((float)bullet.Value.FacingDirection * Mathf.Rad2Deg + 180, Vector3.forward);
                }
            }
        }
        for (int i = 0; i < CoreParam.bulletsG.Count; i++)
        {
            KeyValuePair<long, GameObject> bulletG = CoreParam.bulletsG.ElementAt(i);
            if (bulletG.Value != null)
            {
                if (!CoreParam.bullets.ContainsKey(bulletG.Key))
                {
                    Destroy(bulletG.Value);
                    CoreParam.bulletsG.Remove(bulletG.Key);
                }
            }
        }
    }
    void ShowFactory(Dictionary<Tuple<int, int>, MessageOfFactory> factories)
    {
        foreach (KeyValuePair<Tuple<int, int>, MessageOfFactory> factory in factories)
        {
            if (factory.Value != null)
            {
                if (!CoreParam.factoriesG.ContainsKey(factory.Key))
                {
                    CoreParam.factoriesG[factory.Key] =
                        ObjCreater.GetInstance().CreateObj(ConstructionType.Factory,
                            Tool.GetInstance().GridToUxy(factory.Value.X, factory.Value.Y));
                    // RendererControl.GetInstance().SetColToChild((PlayerTeam)(factory.Value.TeamId + 1),
                    //     CoreParam.factoriesG[factory.Key].transform, 5);

                }
                else
                {
                    CoreParam.factoriesG[factory.Key].transform.position = Tool.GetInstance().GridToUxy(factory.Value.X, factory.Value.Y);
                    // CoreParam.factoriesG[factory.Key].transform.rotation =
                    //     Quaternion.AngleAxis((float)factory.Value.FacingDirection * Mathf.Rad2Deg + 180, Vector3.forward);
                }
            }
        }
        for (int i = 0; i < CoreParam.factoriesG.Count; i++)
        {
            KeyValuePair<Tuple<int, int>, GameObject> factoryG = CoreParam.factoriesG.ElementAt(i);
            if (factoryG.Value != null)
            {
                if (!CoreParam.factories.ContainsKey(factoryG.Key))
                {
                    Destroy(factoryG.Value);
                    CoreParam.factoriesG.Remove(factoryG.Key);
                }
            }
        }
    }
    void ShowCommunity(Dictionary<Tuple<int, int>, MessageOfCommunity> communities)
    {
        foreach (KeyValuePair<Tuple<int, int>, MessageOfCommunity> community in communities)
        {
            if (community.Value != null)
            {
                if (!CoreParam.communitiesG.ContainsKey(community.Key))
                {
                    CoreParam.communitiesG[community.Key] =
                        ObjCreater.GetInstance().CreateObj(ConstructionType.Community,
                            Tool.GetInstance().GridToUxy(community.Value.X, community.Value.Y));
                    // RendererControl.GetInstance().SetColToChild((PlayerTeam)(community.Value.TeamId + 1),
                    //     CoreParam.communitiesG[community.Key].transform);

                }
                else
                {
                    CoreParam.communitiesG[community.Key].transform.position = Tool.GetInstance().GridToUxy(community.Value.X, community.Value.Y);
                    // CoreParam.communitiesG[community.Key].transform.rotation =
                    //     Quaternion.AngleAxis((float)community.Value.FacingDirection * Mathf.Rad2Deg + 180, Vector3.forward);
                }
            }
        }
        for (int i = 0; i < CoreParam.communitiesG.Count; i++)
        {
            KeyValuePair<Tuple<int, int>, GameObject> communityG = CoreParam.communitiesG.ElementAt(i);
            if (communityG.Value != null)
            {
                if (!CoreParam.communities.ContainsKey(communityG.Key))
                {
                    Destroy(communityG.Value);
                    CoreParam.communitiesG.Remove(communityG.Key);
                }
            }
        }
    }
    void ShowFort(Dictionary<Tuple<int, int>, MessageOfFort> forts)
    {
        foreach (KeyValuePair<Tuple<int, int>, MessageOfFort> fort in forts)
        {
            if (fort.Value != null)
            {
                if (!CoreParam.fortsG.ContainsKey(fort.Key))
                {
                    CoreParam.fortsG[fort.Key] =
                        ObjCreater.GetInstance().CreateObj(ConstructionType.Fort,
                            Tool.GetInstance().GridToUxy(fort.Value.X, fort.Value.Y), fort.Value.TeamId == 1);
                    RendererControl.GetInstance().SetColToChild((PlayerTeam)(fort.Value.TeamId + 1),
                        CoreParam.fortsG[fort.Key].transform);

                }
                else
                {
                    CoreParam.fortsG[fort.Key].transform.position = Tool.GetInstance().GridToUxy(fort.Value.X, fort.Value.Y);
                    // CoreParam.fortsG[fort.Key].transform.rotation =
                    //     Quaternion.AngleAxis((float)fort.Value.FacingDirection * Mathf.Rad2Deg + 180, Vector3.forward);
                }
            }
        }
        for (int i = 0; i < CoreParam.fortsG.Count; i++)
        {
            KeyValuePair<Tuple<int, int>, GameObject> fortG = CoreParam.fortsG.ElementAt(i);
            if (fortG.Value != null)
            {
                if (!CoreParam.forts.ContainsKey(fortG.Key))
                {
                    Destroy(fortG.Value);
                    CoreParam.fortsG.Remove(fortG.Key);
                }
            }
        }
    }
    void ShowAllMessage(MessageToClient messageToClient)
    {
        gameTime.text = "GameTime:" + messageToClient.AllMessage.GameTime;
        score.text = "Score(Red:Blue):" + messageToClient.AllMessage.RedTeamScore + ":" + messageToClient.AllMessage.BlueTeamScore;
        energy.text = "Energy(Red:Blue):" + messageToClient.AllMessage.RedTeamEnergy + ":" + messageToClient.AllMessage.BlueTeamEnergy;
    }
}
