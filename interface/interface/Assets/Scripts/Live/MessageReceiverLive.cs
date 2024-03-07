using Grpc.Core;
using Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MessageReceiverLive : SingletonDontDestory<MessageReceiverLive>
{
    public static string IP = "localhost";
    public static string Port = "8888";
    public static string filename = null;

    // Start is called before the first frame update
    async void Start()
    {
        try
        {
            var channel = new Channel(IP + ":" + Port, ChannelCredentials.Insecure);
            var client = new AvailableService.AvailableServiceClient(channel);
            // Debug.Log(channel);
            // Debug.Log(client);
            PlayerMsg msg = new PlayerMsg()
            {
                PlayerId = 2024,
                ShipType = ShipType.NullShipType,
                TeamId = -1,
                X = 0,
                Y = 0,
            };

            Debug.Log("ReadyAddSpectator");
            var response = client.AddPlayer(msg);
            Debug.Log("AddSpectatorSuccess");
            MapControl.GetInstance().DrawMap(client.GetMap(new NullRequest()));
            if (await response.ResponseStream.MoveNext())
            {
                Debug.Log("ReadyDrawMap");
                var responseVal = response.ResponseStream.Current;
                ParaDefine.GetInstance().map = responseVal.ObjMessage[0].MapMessage;
                MapControl.GetInstance().DrawMap(ParaDefine.GetInstance().map);
                Debug.Log("DrawMapSuccess");
            }
            while (await response.ResponseStream.MoveNext())
            {
                var responseVal = response.ResponseStream.Current;
                Receive(responseVal);
            }
            IP = null;
            Port = null;
        }
        catch (RpcException)
        {
            Debug.Log("net work error: ");
            IP = null;
            Port = null;
        }
    }
    private void Receive(MessageToClient message)
    {
        foreach (var messageOfObj in message.ObjMessage)
        {
            switch (messageOfObj.MessageOfObjCase)
            {
                case MessageOfObj.MessageOfObjOneofCase.ShipMessage:
                    if (MessageManager.GetInstance().ShipG[messageOfObj.ShipMessage.Guid] == null)
                    {
                        MessageManager.GetInstance().ShipG[messageOfObj.ShipMessage.Guid] =
                            ObjectCreater.GetInstance().CreateObject(messageOfObj.ShipMessage.ShipType, new Vector3(messageOfObj.ShipMessage.X, messageOfObj.ShipMessage.Y), Quaternion.identity, GameObject.Find("Ship").transform, (int)messageOfObj.ShipMessage.TeamId);
                        MessageManager.GetInstance().Ship[messageOfObj.ShipMessage.Guid] = messageOfObj.ShipMessage;
                    }
                    break;
                case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
                    if (MessageManager.GetInstance().BulletG[messageOfObj.BulletMessage.Guid] == null)
                    {
                        MessageManager.GetInstance().BulletG[messageOfObj.BulletMessage.Guid] =
                            ObjectCreater.GetInstance().CreateObject(messageOfObj.BulletMessage.Type, new Vector3(messageOfObj.BulletMessage.X, messageOfObj.BulletMessage.Y), Quaternion.identity, GameObject.Find("Bullet").transform);
                        MessageManager.GetInstance().Bullet[messageOfObj.BulletMessage.Guid] = messageOfObj.BulletMessage;
                    }
                    break;
                case MessageOfObj.MessageOfObjOneofCase.FactoryMessage:
                    break;
                case MessageOfObj.MessageOfObjOneofCase.CommunityMessage:
                    break;
                case MessageOfObj.MessageOfObjOneofCase.FortMessage:
                    break;
                case MessageOfObj.MessageOfObjOneofCase.WormholeMessage:
                    break;
                case MessageOfObj.MessageOfObjOneofCase.HomeMessage:
                    break;
                case MessageOfObj.MessageOfObjOneofCase.ResourceMessage:
                    break;
                default:
                    break;
            }
        }
    }
}