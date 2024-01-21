using Grpc.Core;
using Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MessageReceiverPlay : SingletonDontDestory<MessageReceiverPlay>
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
            Debug.Log(channel);
            Debug.Log(client);
            PlayerMsg msg = new PlayerMsg()
            {
                PlayerId = 0,
                TeamId = 0,
                ShipType = ShipType.CivilianShip,
                X = 16000,
                Y = 30000,
            };
            var response = client.AddPlayer(msg);
            // var client2 = new AvailableService.AvailableServiceClient(channel);
            // Debug.Log(client2);
            // PlayerMsg msg2 = new PlayerMsg() {
            //     PlayerId = 0,
            //     TeamId = 1,
            //     ShipType = ShipType.NullShipType,
            //     X = 46000,
            //     Y = 30000,
            // };
            // var response2 = client.AddPlayer(msg2);
            // var client3 = new AvailableService.AvailableServiceClient(channel);
            // Debug.Log(client3);
            // PlayerMsg msg3 = new PlayerMsg() {
            //     PlayerId = 1,
            //     TeamId = 0,
            //     ShipType = ShipType.CivilianShip,
            //     X = 30000,
            //     Y = 46000,
            // };
            // var response3 = client.AddPlayer(msg3);
            // var client4 = new AvailableService.AvailableServiceClient(channel);
            // Debug.Log(client4);
            // PlayerMsg msg4 = new PlayerMsg() {
            //     PlayerId = 1,
            //     TeamId = 0,
            //     ShipType = ShipType.CivilianShip,
            //     X = 30000,
            //     Y = 16000,
            // };
            // var response4 = client.AddPlayer(msg4);
            MapControl.GetInstance().DrawMap(client.GetMap(new NullRequest()));
            if (await response.ResponseStream.MoveNext())
            {
                var responseVal = response.ResponseStream.Current;
                Debug.Log("recieve further info");
                ParaDefine.GetInstance().map = responseVal.ObjMessage[0].MapMessage;
                MapControl.GetInstance().DrawMap(ParaDefine.GetInstance().map);
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
                            Instantiate(ParaDefine.GetInstance().PT(messageOfObj.ShipMessage.ShipType), new Vector3(messageOfObj.ShipMessage.X, messageOfObj.ShipMessage.Y), Quaternion.identity, GameObject.Find("Ship").transform);
                        MessageManager.GetInstance().Ship[messageOfObj.ShipMessage.Guid] = messageOfObj.ShipMessage;
                    }
                    break;
                case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
                    if (MessageManager.GetInstance().BulletG[messageOfObj.BulletMessage.Guid] == null)
                    {
                        MessageManager.GetInstance().BulletG[messageOfObj.BulletMessage.Guid] =
                            Instantiate(ParaDefine.GetInstance().PT(messageOfObj.BulletMessage.Type), new Vector3(messageOfObj.BulletMessage.X, messageOfObj.BulletMessage.Y), Quaternion.identity, GameObject.Find("Bullet").transform);
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