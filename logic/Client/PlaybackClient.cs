using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Protobuf;
using Playback;
using System.Threading;
using System.Windows;
using Timothy.FrameRateTask;
using System.Runtime.CompilerServices;
using Client.Util;
using Client.Model;

namespace Client
{
    public class PlaybackClient
    {
        private readonly string fileName;
        private readonly double playbackSpeed;
        private readonly int frameTimeInMilliseconds;
        public MessageReader? Reader;
        private SemaphoreSlim sema;
        public SemaphoreSlim Sema => sema;
        public PlaybackClient(string fileName, double playbackSpeed = 1.0, int frameTimeInMilliseconds = 50)
        {
            this.fileName = fileName;
            this.playbackSpeed = playbackSpeed;
            this.frameTimeInMilliseconds = frameTimeInMilliseconds;
            sema = new SemaphoreSlim(1, 1);
            try
            {
                Reader = new MessageReader(this.fileName);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Reader = null;
                return;
            }
        }


        public int[,]? ReadDataFromFile(
            List<MessageOfAll> listOfAll,
            List<MessageOfShip> listOfShip,
            List<MessageOfBullet> listOfBullet,
            List<MessageOfBombedBullet> listOfBombedBullet,
            List<MessageOfFactory> listOfFactory,
            List<MessageOfCommunity> listOfCommunity,
            List<MessageOfFort> listOfFort,
            List<MessageOfResource> listOfResource,
            List<MessageOfHome> listOfHome,
            List<MessageOfWormhole> listOfWormhole,
            object? datalock)
        {
            if (Reader == null)
            {
                return null;
            }
            Sema.Wait();
            bool endFile = false;
            bool mapFlag = false;  // 是否获取了地图
            int[,] map = new int[50, 50];
            long frame = (long)(this.frameTimeInMilliseconds / this.playbackSpeed);
            var mapCollecter = new MessageReader(this.fileName);
            while (!mapFlag)
            {
                var msg = mapCollecter.ReadOne();
                if (msg == null)
                    throw new Exception("Map message is not in the playback file!");
                foreach (var obj in msg.ObjMessage)
                {
                    if (obj.MessageOfObjCase == MessageOfObj.MessageOfObjOneofCase.MapMessage)
                    {
                        try
                        {
                            for (int i = 0; i < 50; i++)
                            {
                                for (int j = 0; j < 50; j++)
                                {
                                    switch (obj.MapMessage.Rows[i].Cols[j])
                                    {
                                        case PlaceType.NullPlaceType:
                                            map[i, j] = (int)MapPatchType.Null;
                                            break;
                                        case PlaceType.Space:
                                            map[i, j] = (int)MapPatchType.Space;
                                            break;
                                        case PlaceType.Ruin:
                                            map[i, j] = (int)MapPatchType.Ruin;
                                            break;
                                        case PlaceType.Shadow:
                                            map[i, j] = (int)MapPatchType.Shadow;
                                            break;
                                        case PlaceType.Asteroid:
                                            map[i, j] = (int)MapPatchType.Asteroid;
                                            break;
                                        case PlaceType.Resource:
                                            map[i, j] = (int)MapPatchType.Resource;
                                            break;
                                        case PlaceType.Construction:
                                            map[i, j] = (int)MapPatchType.Factory;
                                            break;
                                        case PlaceType.Wormhole:
                                            map[i, j] = (int)MapPatchType.WormHole;
                                            break;
                                        case PlaceType.Home:
                                            map[i, j] = (int)MapPatchType.RedHome;
                                            break;
                                        default:
                                            map[i, j] = (int)MapPatchType.Null;
                                            break;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            mapFlag = false;
                        }
                        finally
                        {
                            mapFlag = true;
                        }
                        break;
                    }
                }
            }
            new Thread(() =>
            {
                try
                {
                    new FrameRateTaskExecutor<int>
                    (
                       () => !endFile,
                       () =>
                       {
                           var content = Reader.ReadOne();
                           if (content == null)
                           {
                               System.Diagnostics.Debug.WriteLine("============= endFile! ================");
                               endFile = true;
                           }
                           else
                           {
                               System.Diagnostics.Debug.WriteLine("============= enter game! ================");

                               lock (datalock)
                               {
                                   System.Diagnostics.Debug.WriteLine("============= enter datalock! ================");

                                   listOfAll.Clear();
                                   listOfShip.Clear();
                                   listOfBullet.Clear();
                                   listOfBombedBullet.Clear();
                                   listOfFactory.Clear();
                                   listOfCommunity.Clear();
                                   listOfFort.Clear();
                                   listOfResource.Clear();
                                   listOfHome.Clear();
                                   listOfWormhole.Clear();
                                   switch (content.GameState)
                                   {
                                       case GameState.GameStart:
                                           System.Diagnostics.Debug.WriteLine("============= GameState: Game Start ================");
                                           foreach (var obj in content.ObjMessage)
                                           {
                                               switch (obj.MessageOfObjCase)
                                               {
                                                   case MessageOfObj.MessageOfObjOneofCase.ShipMessage:
                                                       listOfShip.Add(obj.ShipMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
                                                       listOfBullet.Add(obj.BulletMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.BombedBulletMessage:
                                                       listOfBombedBullet.Add(obj.BombedBulletMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.FactoryMessage:
                                                       listOfFactory.Add(obj.FactoryMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.CommunityMessage:
                                                       listOfCommunity.Add(obj.CommunityMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.FortMessage:
                                                       listOfFort.Add(obj.FortMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.ResourceMessage:
                                                       listOfResource.Add(obj.ResourceMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.HomeMessage:
                                                       listOfHome.Add(obj.HomeMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.WormholeMessage:
                                                       listOfWormhole.Add(obj.WormholeMessage);
                                                       break;

                                                       //case MessageOfObj.MessageOfObjOneofCase.MapMessage:
                                                       //    mapMassage = obj.MapMessage;
                                                       //    break;
                                               }
                                           }
                                           listOfAll.Add(content.AllMessage);
                                           //countMap.Clear();
                                           //countMap.Add((int)MapPatchType.Resource, listOfResource.Count);
                                           //countMap.Add((int)MapPatchType.Factory, listOfFactory.Count);
                                           //countMap.Add((int)MapPatchType.Community, listOfCommunity.Count);
                                           //countMap.Add((int)MapPatchType.Fort, listOfFort.Count);
                                           //GetMap(mapMassage);
                                           break;
                                       case GameState.GameRunning:
                                           System.Diagnostics.Debug.WriteLine("============= GameState: Game Running ================");
                                           foreach (var obj in content.ObjMessage)
                                           {
                                               switch (obj.MessageOfObjCase)
                                               {
                                                   case MessageOfObj.MessageOfObjOneofCase.ShipMessage:
                                                       System.Diagnostics.Debug.WriteLine(String.Format("============= ShipOrd: {0},{1} ============", obj.ShipMessage.X, obj.ShipMessage.Y));
                                                       listOfShip.Add(obj.ShipMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.FactoryMessage:
                                                       listOfFactory.Add(obj.FactoryMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.CommunityMessage:
                                                       listOfCommunity.Add(obj.CommunityMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.FortMessage:
                                                       listOfFort.Add(obj.FortMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
                                                       System.Diagnostics.Debug.WriteLine(String.Format("============= BulletOrd: {0},{1} ============", obj.BulletMessage.X, obj.BulletMessage.Y));
                                                       listOfBullet.Add(obj.BulletMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.BombedBulletMessage:
                                                       listOfBombedBullet.Add(obj.BombedBulletMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.ResourceMessage:
                                                       listOfResource.Add(obj.ResourceMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.HomeMessage:
                                                       listOfHome.Add(obj.HomeMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.WormholeMessage:
                                                       listOfWormhole.Add(obj.WormholeMessage);
                                                       break;

                                                       //case MessageOfObj.MessageOfObjOneofCase.MapMessage:
                                                       //    mapMassage = obj.MapMessage;
                                                       //    mapMessageExist = true;
                                                       //    break;
                                               }
                                           }
                                           listOfAll.Add(content.AllMessage);
                                           //if (mapMessageExist)
                                           //{
                                           //    countMap.Clear();
                                           //    countMap.Add((int)MapPatchType.Resource, listOfResource.Count);
                                           //    countMap.Add((int)MapPatchType.Factory, listOfFactory.Count);
                                           //    countMap.Add((int)MapPatchType.Community, listOfCommunity.Count);
                                           //    countMap.Add((int)MapPatchType.Fort, listOfFort.Count);
                                           //    GetMap(mapMassage);
                                           //    mapMessageExist = false;
                                           //}
                                           break;
                                       case GameState.GameEnd:
                                           System.Diagnostics.Debug.WriteLine("============= GameState: Game End ================");
                                           //DisplayAlert("Info", "Game End", "OK");
                                           foreach (var obj in content.ObjMessage)
                                           {
                                               switch (obj.MessageOfObjCase)
                                               {
                                                   case MessageOfObj.MessageOfObjOneofCase.ShipMessage:
                                                       listOfShip.Add(obj.ShipMessage);
                                                       break;

                                                   //case MessageOfObj.MessageOfObjOneofCase.BuildingMessage:
                                                   //    listOfBuilding.Add(obj.BuildingMessage);
                                                   //    break;

                                                   case MessageOfObj.MessageOfObjOneofCase.FactoryMessage:
                                                       listOfFactory.Add(obj.FactoryMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.CommunityMessage:
                                                       listOfCommunity.Add(obj.CommunityMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.FortMessage:
                                                       listOfFort.Add(obj.FortMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
                                                       listOfBullet.Add(obj.BulletMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.BombedBulletMessage:
                                                       listOfBombedBullet.Add(obj.BombedBulletMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.ResourceMessage:
                                                       listOfResource.Add(obj.ResourceMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.HomeMessage:
                                                       listOfHome.Add(obj.HomeMessage);
                                                       break;

                                                   case MessageOfObj.MessageOfObjOneofCase.WormholeMessage:
                                                       listOfWormhole.Add(obj.WormholeMessage);
                                                       break;

                                                       //case MessageOfObj.MessageOfObjOneofCase.MapMessage:
                                                       //    mapMassage = obj.MapMessage;
                                                       //    break;
                                               }
                                           }
                                           listOfAll.Add(content.AllMessage);
                                           //if (mapMessageExist)
                                           //{
                                           //    countMap.Clear();
                                           //    countMap.Add((int)MapPatchType.Resource, listOfResource.Count);
                                           //    countMap.Add((int)MapPatchType.Factory, listOfFactory.Count);
                                           //    countMap.Add((int)MapPatchType.Community, listOfCommunity.Count);
                                           //    countMap.Add((int)MapPatchType.Fort, listOfFort.Count);
                                           //    GetMap(mapMassage);
                                           //    mapMessageExist = false;
                                           //}
                                           break;
                                   }
                               }
                           }
                       },
                       frame,
                       () =>
                       {
                           Sema.Release();
                           return 1;
                       }
                       )
                    { AllowTimeExceed = true }.Start();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            })
            { IsBackground = true }.Start();
            return map;
        }
    }
}
