using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protobuf;
using System;
using TMPro;
using System.Linq;
using UnityEditor;

public class RenderManager : SingletonMono<RenderManager>
{
    public TextMeshProUGUI gameTime, score;
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
        while (true)
        {
            if (!CoreParam.initialized && CoreParam.firstFrame != null)
            {
                DealFrame(CoreParam.firstFrame);
                ShowFrame();
            }
            while (CoreParam.frameQueue.GetSize() == 0)
                yield return 0;
            CoreParam.currentFrame = CoreParam.frameQueue.GetValue();

            if (CoreParam.currentFrame == null)
                Debug.Log("aa");
            if (CoreParam.currentFrame != null)
            {
                DealFrame(CoreParam.currentFrame);
                ShowFrame();
            }
            yield return 0;
        }
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
                CoreParam.ships[obj.ShipMessage.TeamId * 4 + obj.ShipMessage.PlayerId - 1] = obj;
                break;
            case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
                CoreParam.bullets[obj.BulletMessage.Guid] = obj.BulletMessage;
                break;
            case MessageOfObj.MessageOfObjOneofCase.HomeMessage:
                CoreParam.homes[obj.HomeMessage.TeamId] = obj;
                if (CoreParam.homesG[obj.HomeMessage.TeamId])
                    CoreParam.homesG[obj.HomeMessage.TeamId].GetComponent<InteractBase>().messageOfObject = obj;
                break;
            case MessageOfObj.MessageOfObjOneofCase.TeamMessage:
                CoreParam.teams[obj.TeamMessage.TeamId] = obj.TeamMessage;
                break;
            case MessageOfObj.MessageOfObjOneofCase.ResourceMessage:
                CoreParam.resources[Tool.GetInstance().GridToCell(obj.ResourceMessage.X, obj.ResourceMessage.Y)] = obj;
                if (CoreParam.resourcesG.ContainsKey(Tool.GetInstance().GridToCell(obj.ResourceMessage.X, obj.ResourceMessage.Y)))
                    CoreParam.resourcesG[Tool.GetInstance().GridToCell(obj.ResourceMessage.X, obj.ResourceMessage.Y)].GetComponent<InteractBase>().messageOfObject = obj;
                break;
            case MessageOfObj.MessageOfObjOneofCase.WormholeMessage:
                CoreParam.wormholes[new Tuple<int, int>(obj.WormholeMessage.X, obj.WormholeMessage.Y)] = obj.WormholeMessage;
                break;
            case MessageOfObj.MessageOfObjOneofCase.CommunityMessage:
                CoreParam.communities[new Tuple<int, int>(obj.CommunityMessage.X, obj.CommunityMessage.Y)] = obj;
                break;
            case MessageOfObj.MessageOfObjOneofCase.FactoryMessage:
                CoreParam.factories[new Tuple<int, int>(obj.FactoryMessage.X, obj.FactoryMessage.Y)] = obj;
                break;
            case MessageOfObj.MessageOfObjOneofCase.FortMessage:
                CoreParam.forts[new Tuple<int, int>(obj.FortMessage.X, obj.FortMessage.Y)] = obj;
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
            {
                if (map.Rows[row].Cols[col] == PlaceType.Home)
                {
                    CoreParam.homesG[row < 25 ? 0 : 1] = ObjCreater.GetInstance().CreateObj(map.Rows[row].Cols[col], Tool.GetInstance().CellToUxy(row, col));
                    RendererControl.GetInstance().SetColToChild(row < 25 ? PlayerTeam.Red : PlayerTeam.Blue, CoreParam.homesG[row < 25 ? 0 : 1].transform);
                }
                else if (map.Rows[row].Cols[col] == PlaceType.Resource)
                {
                    CoreParam.resourcesG.Add(new Tuple<int, int>(row + 1, col + 1),
                        ObjCreater.GetInstance().CreateObj(map.Rows[row].Cols[col], Tool.GetInstance().CellToUxy(row, col)));
                    // Debug.Log(row + 1 + "  " + col + 1);
                }
                else
                    ObjCreater.GetInstance().CreateObj(map.Rows[row].Cols[col], Tool.GetInstance().CellToUxy(row, col));
            }
    }
    void ShowShip(Dictionary<long, MessageOfObj> ships)
    {
        foreach (KeyValuePair<long, MessageOfObj> ship in ships)
        {
            if (ship.Value != null)
            {
                if (!CoreParam.shipsG.ContainsKey(ship.Key))
                {
                    CoreParam.shipsG[ship.Key] =
                        ObjCreater.GetInstance().CreateObj(ship.Value.ShipMessage.ShipType,
                            Tool.GetInstance().GridToUxy(ship.Value.ShipMessage.X, ship.Value.ShipMessage.Y));
                    RendererControl.GetInstance().SetColToChild((PlayerTeam)(ship.Value.ShipMessage.TeamId + 1),
                        CoreParam.shipsG[ship.Key].transform);
                    CoreParam.shipsG[ship.Key].GetComponent<InteractBase>().messageOfObject =
                        ship.Value;
                }
                else
                {
                    // Debug.Log(CoreParam.shipsG[ship.Key]);
                    CoreParam.shipsG[ship.Key].transform.position =
                        Tool.GetInstance().GridToUxy(ship.Value.ShipMessage.X, ship.Value.ShipMessage.Y);
                    CoreParam.shipsG[ship.Key].transform.rotation =
                        Quaternion.AngleAxis((float)ship.Value.ShipMessage.FacingDirection * Mathf.Rad2Deg + 180, Vector3.forward);
                    CoreParam.shipsG[ship.Key].GetComponent<InteractBase>().messageOfObject =
                        ship.Value;
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
                    CoreParam.shipsG.Remove(shipG.Key);
                    Destroy(shipG.Value);
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
    void ShowFactory(Dictionary<Tuple<int, int>, MessageOfObj> factories)
    {
        foreach (KeyValuePair<Tuple<int, int>, MessageOfObj> factory in factories)
        {
            if (factory.Value != null)
            {
                if (!CoreParam.factoriesG.ContainsKey(factory.Key))
                {
                    CoreParam.factoriesG[factory.Key] =
                        ObjCreater.GetInstance().CreateObj(ConstructionType.Factory,
                            Tool.GetInstance().GridToUxy(factory.Value.FactoryMessage.X, factory.Value.FactoryMessage.Y));
                    // RendererControl.GetInstance().SetColToChild((PlayerTeam)(factory.Value.FactoryMessage.TeamId + 1),
                    //     CoreParam.factoriesG[factory.Key].transform, 5);
                    CoreParam.factoriesG[factory.Key].GetComponent<InteractBase>().messageOfObject =
                        factory.Value;

                }
                else
                {
                    CoreParam.factoriesG[factory.Key].transform.position = Tool.GetInstance().GridToUxy(factory.Value.FactoryMessage.X, factory.Value.FactoryMessage.Y);
                    // CoreParam.factoriesG[factory.Key].transform.rotation =
                    //     Quaternion.AngleAxis((float)factory.Value.FactoryMessage.FacingDirection * Mathf.Rad2Deg + 180, Vector3.forward);
                    CoreParam.factoriesG[factory.Key].GetComponent<InteractBase>().messageOfObject =
                        factory.Value;
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
    void ShowCommunity(Dictionary<Tuple<int, int>, MessageOfObj> communities)
    {
        foreach (KeyValuePair<Tuple<int, int>, MessageOfObj> community in communities)
        {
            if (community.Value != null)
            {
                if (!CoreParam.communitiesG.ContainsKey(community.Key))
                {
                    CoreParam.communitiesG[community.Key] =
                        ObjCreater.GetInstance().CreateObj(ConstructionType.Community,
                            Tool.GetInstance().GridToUxy(community.Value.CommunityMessage.X, community.Value.CommunityMessage.Y));
                    // RendererControl.GetInstance().SetColToChild((PlayerTeam)(community.Value.CommunityMessage.TeamId + 1),
                    //     CoreParam.communitiesG[community.Key].transform);
                    CoreParam.communitiesG[community.Key].GetComponent<InteractBase>().messageOfObject =
                        community.Value;

                }
                else
                {
                    CoreParam.communitiesG[community.Key].transform.position = Tool.GetInstance().GridToUxy(community.Value.CommunityMessage.X, community.Value.CommunityMessage.Y);
                    // CoreParam.communitiesG[community.Key].transform.rotation =
                    //     Quaternion.AngleAxis((float)community.Value.CommunityMessage.FacingDirection * Mathf.Rad2Deg + 180, Vector3.forward);
                    CoreParam.communitiesG[community.Key].GetComponent<InteractBase>().messageOfObject =
                        community.Value;
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
    void ShowFort(Dictionary<Tuple<int, int>, MessageOfObj> forts)
    {
        foreach (KeyValuePair<Tuple<int, int>, MessageOfObj> fort in forts)
        {
            if (fort.Value != null)
            {
                if (!CoreParam.fortsG.ContainsKey(fort.Key))
                {
                    CoreParam.fortsG[fort.Key] =
                        ObjCreater.GetInstance().CreateObj(ConstructionType.Fort,
                            Tool.GetInstance().GridToUxy(fort.Value.FortMessage.X, fort.Value.FortMessage.Y), fort.Value.FortMessage.TeamId == 1);
                    RendererControl.GetInstance().SetColToChild((PlayerTeam)(fort.Value.FortMessage.TeamId + 1),
                        CoreParam.fortsG[fort.Key].transform);
                    CoreParam.fortsG[fort.Key].GetComponent<InteractBase>().messageOfObject =
                        fort.Value;

                }
                else
                {
                    CoreParam.fortsG[fort.Key].transform.position = Tool.GetInstance().GridToUxy(fort.Value.FortMessage.X, fort.Value.FortMessage.Y);
                    // CoreParam.fortsG[fort.Key].transform.rotation =
                    //     Quaternion.AngleAxis((float)fort.Value.FortMessage.FacingDirection * Mathf.Rad2Deg + 180, Vector3.forward);
                    CoreParam.fortsG[fort.Key].GetComponent<InteractBase>().messageOfObject =
                        fort.Value;
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
    }
}
