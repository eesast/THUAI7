using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Client.Model;
using Protobuf;
using Grpc.Core;
using System.Diagnostics.Metrics;
using Client.Util;
using Client.Interact;
using CommandLine;
using Newtonsoft;
using System.Globalization;
using System.Collections.ObjectModel;
using installer;
using installer.Model;

namespace Client.ViewModel
{
    public partial class GeneralViewModel : BindableObject
    {
        private IDispatcherTimer timerViewModel;

        int counterViewModelTest = 0;

        private string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }


        public MapPatch testPatch;
        public MapPatch TestPatch
        {
            get
            {
                return testPatch;
            }
            set
            {
                testPatch = value;
                OnPropertyChanged();
            }
        }


        private List<Link> links;
        public List<Link> Links
        {
            get
            {
                return links ??= [];
            }
            set
            {
                links = value;
                OnPropertyChanged();
            }
        }

        private long playerID;
        private readonly string ip;
        private readonly string port;
        private readonly int shipTypeID;
        private long teamID;
        ShipType shipType;
        AvailableService.AvailableServiceClient? client;
        AsyncServerStreamingCall<MessageToClient>? responseStream;
        bool isSpectatorMode = false;
        bool isPlaybackMode = false;
        // 连接Server,comInfo[]的格式：0-ip 1- port 2-playerID 3-teamID 4-ShipType
        public void ConnectToServer(string[] comInfo)
        {
            if (isPlaybackMode) return;
            if (Convert.ToInt64(comInfo[2]) > 2023)
            {
                isSpectatorMode = true;
                myLogger.LogInfo("isSpectatorMode = true");
            }

            //if (!isSpectatorMode && comInfo.Length != 5 || isSpectatorMode && comInfo.Length != 3)
            if (comInfo.Length != 5)
            {
                throw new Exception("Error Registration Information！");
            }

            string connect = new(comInfo[0]);
            connect += ':';
            connect += comInfo[1];
            Channel channel = new(connect, ChannelCredentials.Insecure);
            client = new AvailableService.AvailableServiceClient(channel);
            PlayerMsg playerMsg = new();
            playerID = Convert.ToInt64(comInfo[2]);
            playerMsg.PlayerId = playerID;
            if (!isSpectatorMode)
            {
                teamID = Convert.ToInt64(comInfo[3]);
                playerMsg.TeamId = teamID;
                shipType = Convert.ToInt64(comInfo[4]) switch
                {
                    0 => ShipType.NullShipType,
                    1 => ShipType.CivilianShip,
                    2 => ShipType.MilitaryShip,
                    3 => ShipType.FlagShip,
                    _ => ShipType.NullShipType
                };
                playerMsg.ShipType = shipType;
            }
            responseStream = client.AddPlayer(playerMsg);
            isClientStocked = false;
        }

        string playbackFile;
        double playbackSpeed;
        private void Playback(string fileName, double pbSpeed = 2.0)
        {
            var pbClient = new PlaybackClient(fileName, pbSpeed);
            int[,]? map;
            if ((map = pbClient.ReadDataFromFile(listOfAll, listOfShip, listOfBullet, listOfBombedBullet, listOfFactory, listOfCommunity, listOfFort, listOfResource, listOfHome, listOfWormhole, drawPicLock)) != null)
            {
                isClientStocked = false;
                isPlaybackMode = true;
                defaultMap = map;
                getMapFlag = true;
            }
            else
            {
                myLogger.LogInfo("Playback failed");
                isClientStocked = true;
            }
        }

        string[] comInfo = new string[10];

        //CommandLineArgs.ArgumentOptions? options = null;
        //private void ReactToCommandline()
        //{
        //    string[] args = Environment.GetCommandLineArgs();
        //    if (args.Length == 2)
        //    {
        //        Playback(args[1]);
        //        return;
        //    }
        //    _ = Parser.Default.ParseArguments<CommandLineArgs.ArgumentOptions>(args).WithParsed(o =>
        //    { options = o; });
        //    if (options != null && Convert.ToInt64(options.PlayerID) > 2023)
        //    {
        //        isSpectatorMode = true;
        //        string[] comInfo = new string[3];
        //        comInfo[0] = options.Ip;
        //        comInfo[1] = options.Port;
        //        comInfo[2] = options.PlayerID;
        //        ConnectToServer(comInfo);
        //        OnReceive();
        //        return;
        //    }
        //    if (options == null || options.cl == false)
        //    {
        //        OnReceive();
        //    }
        //    else
        //    {
        //        if (options.PlaybackFile == DefaultArgumentOptions.FileName)
        //        {
        //            try
        //            {
        //                string[] comInfo = new string[5];
        //                comInfo[0] = options.Ip;
        //                comInfo[1] = options.Port;
        //                comInfo[2] = options.PlayerID;
        //                comInfo[3] = options.PlayerType;
        //                comInfo[4] = options.Occupation;
        //                ConnectToServer(comInfo);
        //                OnReceive();
        //            }
        //            catch
        //            {
        //                OnReceive();
        //            }
        //        }
        //        else
        //        {
        //            Playback(options.PlaybackFile, options.PlaybackSpeed);
        //        }
        //    }
        //}
        int testcounter = 0;


        private async void OnReceive()
        {
            try
            {
                myLogger.LogInfo("============= OnReceiving Server Stream ================");
                //if (responseStream != null)
                //    myLogger.LogInfo("============= responseStream != null ================");

                //if (await responseStream.ResponseStream.MoveNext())
                //    myLogger.LogInfo("============= responseStream.ResponseStream.MoveNext() ================");
                //await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    while (responseStream != null && await responseStream.ResponseStream.MoveNext())
                    {
                        myLogger.LogInfo("============= Receiving Server Stream ================");
                        lock (drawPicLock)
                        {
                            //if (ballx_receive >= 500)
                            //{
                            //    dx = -5;
                            //}
                            //else if (ballx_receive <= 0)
                            //{
                            //    dx = 5;
                            //}
                            //ballx_receive += dx;
                            //bally_receive += dx;
                            //myLogger.LogInfo(String.Format("============= Onreceive: ballx_receive:{0}, bally_receive:{1} ================", ballx_receive, bally_receive));
                            //myLogger.LogInfo(String.Format("OnReceive--cou:{0}, coud{1}", cou, Countdow));

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

                            MessageToClient content = responseStream.ResponseStream.Current;
                            MessageOfMap mapMassage = new();
                            bool mapMessageExist = false;
                            switch (content.GameState)
                            {
                                case GameState.GameStart:
                                    myLogger.LogInfo("============= GameState: Game Start ================");
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

                                            case MessageOfObj.MessageOfObjOneofCase.MapMessage:
                                                mapMassage = obj.MapMessage;
                                                break;
                                        }
                                    }
                                    listOfAll.Add(content.AllMessage);
                                    countMap.Clear();
                                    countMap.Add((int)MapPatchType.Resource, listOfResource.Count);
                                    countMap.Add((int)MapPatchType.Factory, listOfFactory.Count);
                                    countMap.Add((int)MapPatchType.Community, listOfCommunity.Count);
                                    countMap.Add((int)MapPatchType.Fort, listOfFort.Count);
                                    GetMap(mapMassage);
                                    break;
                                case GameState.GameRunning:
                                    myLogger.LogInfo("============= GameState: Game Running ================");
                                    foreach (var obj in content.ObjMessage)
                                    {
                                        switch (obj.MessageOfObjCase)
                                        {
                                            case MessageOfObj.MessageOfObjOneofCase.ShipMessage:
                                                myLogger.LogInfo(String.Format("============= ShipOrd: {0},{1} ============", obj.ShipMessage.X, obj.ShipMessage.Y));
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
                                                myLogger.LogInfo(String.Format("============= BulletOrd: {0},{1} ============", obj.BulletMessage.X, obj.BulletMessage.Y));
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

                                            case MessageOfObj.MessageOfObjOneofCase.MapMessage:
                                                mapMassage = obj.MapMessage;
                                                mapMessageExist = true;
                                                break;
                                        }
                                    }
                                    listOfAll.Add(content.AllMessage);
                                    if (mapMessageExist)
                                    {
                                        countMap.Clear();
                                        countMap.Add((int)MapPatchType.Resource, listOfResource.Count);
                                        countMap.Add((int)MapPatchType.Factory, listOfFactory.Count);
                                        countMap.Add((int)MapPatchType.Community, listOfCommunity.Count);
                                        countMap.Add((int)MapPatchType.Fort, listOfFort.Count);
                                        GetMap(mapMassage);
                                        mapMessageExist = false;
                                    }
                                    break;

                                case GameState.GameEnd:
                                    myLogger.LogInfo("============= GameState: Game End ================");
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

                                            case MessageOfObj.MessageOfObjOneofCase.MapMessage:
                                                mapMassage = obj.MapMessage;
                                                break;
                                        }
                                    }
                                    listOfAll.Add(content.AllMessage);
                                    if (mapMessageExist)
                                    {
                                        countMap.Clear();
                                        countMap.Add((int)MapPatchType.Resource, listOfResource.Count);
                                        countMap.Add((int)MapPatchType.Factory, listOfFactory.Count);
                                        countMap.Add((int)MapPatchType.Community, listOfCommunity.Count);
                                        countMap.Add((int)MapPatchType.Fort, listOfFort.Count);
                                        GetMap(mapMassage);
                                        mapMessageExist = false;
                                    }
                                    break;
                            }
                        }
                        if (responseStream == null)
                        {
                            throw new Exception("Unconnected");
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                myLogger.LogInfo("-----------------------------");
                myLogger.LogInfo(ex.Message);
                /* 
                    #TODO
                    Show the error message
                */
            }
        }

        public bool redShipsLabelIsBusy = true;

        public bool RedShipsLabelIsBusy
        {
            get
            {
                return redShipsLabelIsBusy;
            }
             
            set
            {
                redShipsLabelIsBusy = value;
            }
        }

        public bool blueShipsLabelIsBusy = true;

        public bool BlueShipsLabelIsBusy
        {
            get
            {
                return blueShipsLabelIsBusy;
            }

            set
            {
                blueShipsLabelIsBusy = value;
            }
        }


        private void Refresh(object sender, EventArgs e)
        {
            try
            {
                lock (drawPicLock)
                {
                    
                    //if (UIinitiated)
                    //{
                    //    redPlayer.SlideLengthSet();
                    //    bluePlayer.SlideLengthSet();
                    //    gameStatusBar.SlideLengthSet();
                    //}
                    if (!isClientStocked)
                    {
                        if (!hasDrawn && getMapFlag)
                        {
                            PureDrawMap(defaultMap);
                            hasDrawn = true;
                        }
                        DrawShip();
                        DrawBullet();
                        //ShipCircList[5].Text = "111";
                        //ShipCircList[5].Color = Colors.Blue;
                        //ShipCircList[5].X += 4;
                        //ShipCircList[5].Y += 4;
                        foreach (var data in listOfAll)
                        {
                            myLogger.LogInfo(String.Format("Red team Energy: {0}", Convert.ToString(data.RedTeamEnergy)));
                            myLogger.LogInfo(String.Format("Blue team Energy: {0}", Convert.ToString(data.BlueTeamEnergy)));
                            RedPlayer.Money = data.RedTeamEnergy;
                            RedPlayer.Hp = data.RedHomeHp;
                            RedPlayer.Score = data.RedTeamScore;
                            BluePlayer.Money = data.BlueTeamEnergy;
                            BluePlayer.Hp = data.BlueHomeHp;
                            BluePlayer.Score = data.BlueTeamScore;
                        }

                        myLogger.LogInfo("============= Read data of ALL ================");
                        foreach (var data in listOfHome)
                        {
                            DrawHome(data);
                            // if (data.TeamId == (long)PlayerTeam.Red)
                            if (data.TeamId == 0)
                            {
                                RedPlayer.Team = data.TeamId;
                            }
                            // else if (data.TeamId == (long)PlayerTeam.Blue)
                            else if (data.TeamId == 1)
                            {
                                BluePlayer.Team = data.TeamId;
                            }
                        }
                        myLogger.LogInfo("============= Draw Home ================");


                        //RedPlayer.Ships.Clear();
                        //BluePlayer.Ships.Clear();
                        redShipsLabelIsBusy = true;
                        blueShipsLabelIsBusy = true;
                        int RedShipCount = 0;
                        int BlueShipCount = 0;
                        for (int i = 0; i < listOfShip.Count; i++)
                        {
                            MessageOfShip data = listOfShip[i];
                            // if (data.TeamId == (long)PlayerTeam.Red)
                            if (data.TeamId == 0)
                            {
                                Ship ship = new()
                                {
                                    HP = data.Hp,
                                    Type = data.ShipType,
                                    State = data.ShipState,
                                    ArmorModule = data.ArmorType,
                                    ShieldModule = data.ShieldType,
                                    WeaponModule = data.WeaponType,
                                    ProducerModule = data.ProducerType,
                                    ConstuctorModule = data.ConstructorType,
                                };
                                myLogger.LogInfo(String.Format("RedShipCount:{0}, Redplayers.ships.count:{1}", RedShipCount, RedPlayer.Ships.Count));
                                //if (listOfShip.Count >= RedPlayer.Ships.Count)
                                {
                                    myLogger.LogInfo(String.Format("listOfShip.Count:{0}, RedPlayer.Ships.Count:{1}", listOfShip.Count, RedPlayer.Ships.Count));

                                    if (RedShipCount < RedPlayer.Ships.Count && UtilFunctions.IsShipEqual(ship, RedPlayer.Ships[RedShipCount]))
                                    {
                                        RedShipCount++;
                                        continue;
                                    }
                                    else if (RedShipCount < RedPlayer.Ships.Count && !UtilFunctions.IsShipEqual(ship, RedPlayer.Ships[RedShipCount]))
                                        RedPlayer.Ships[RedShipCount] = ship;
                                    else RedPlayer.Ships.Add(ship);
                                    RedShipCount++;
                                }
                                //else
                                //{
                                //    myLogger.LogInfo("listOfShip.Count < RedPlayer.Ships.Count");
                                //    myLogger.LogInfo(String.Format("listOfShip.Count:{0}, RedPlayer.Ships.Count:{1}", listOfShip.Count, RedPlayer.Ships.Count));
                                //    if (RedShipCount < listOfShip.Count && UtilFunctions.IsShipEqual(ship, RedPlayer.Ships[RedShipCount]))
                                //    {
                                //        RedShipCount++;
                                //        continue;
                                //    }
                                //    else if (RedShipCount < listOfShip.Count && !UtilFunctions.IsShipEqual(ship, RedPlayer.Ships[RedShipCount]))
                                //    {
                                //        RedPlayer.Ships[RedShipCount] = ship;
                                //        RedShipCount++;
                                //    }
                                //    else
                                //    {
                                //        for (int index = listOfShip.Count; index < RedPlayer.Ships.Count - 1; index++)
                                //        RedPlayer.Ships.RemoveAt(index);
                                //    }
                                //}
                            }
                            // else if (data.TeamId == (long)PlayerTeam.Blue)
                            else if (data.TeamId == 1)
                            {
                                Ship ship = new()
                                {
                                    HP = data.Hp,
                                    Type = data.ShipType,
                                    State = data.ShipState,
                                    ArmorModule = data.ArmorType,
                                    ShieldModule = data.ShieldType,
                                    WeaponModule = data.WeaponType,
                                    ProducerModule = data.ProducerType,
                                    ConstuctorModule = data.ConstructorType,
                                };
                                myLogger.LogInfo(String.Format("BlueShipCount:{0}, Blueplayer.ships.count:{1}", BlueShipCount, BluePlayer.Ships.Count));

                                if (BlueShipCount < BluePlayer.Ships.Count && UtilFunctions.IsShipEqual(ship, BluePlayer.Ships[BlueShipCount]))
                                {
                                    BlueShipCount++;
                                    continue;
                                }
                                else if (BlueShipCount < BluePlayer.Ships.Count && !UtilFunctions.IsShipEqual(ship, BluePlayer.Ships[BlueShipCount]))
                                    BluePlayer.Ships[BlueShipCount] = ship;
                                else BluePlayer.Ships.Add(ship);
                                BlueShipCount++;
                            }
                            
                            //else
                            //{
                            //    Ship ship = new Ship
                            //    {
                            //        HP = data.Hp,
                            //        Type = data.ShipType,
                            //        State = data.ShipState,
                            //        ArmorModule = data.ArmorType,
                            //        ShieldModule = data.ShieldType,
                            //        WeaponModule = data.WeaponType,
                            //        ProducerModule = data.ProducerType,
                            //        ConstuctorModule = data.ConstructorType,
                            //        //Type_s = UtilInfo.ShipTypeNameDict[data.ShipType],
                            //        //State_s = UtilInfo.ShipStateNameDict[data.ShipState],
                            //        //ArmorModule_s = UtilInfo.ShipArmorTypeNameDict[data.ArmorType],
                            //        //ShieldModule_s = UtilInfo.ShipShieldTypeNameDict[data.ShieldType],
                            //        //WeaponModule_s = UtilInfo.ShipWeaponTypeNameDict[data.WeaponType],
                            //        //ConstuctorModule_s = UtilInfo.ShipConstructorNameDict[data.ConstructorType],
                            //        //ProducerModule_s = UtilInfo.ShipProducerTypeNameDict[data.ProducerType]
                            //    };
                            //    myLogger.LogInfo(String.Format("i:{0}, Redplayers.ships.count:{1}", i, RedPlayer.Ships.Count));
                            //    if (i < RedPlayer.Ships.Count && UtilFunctions.IsShipEqual(ship, RedPlayer.Ships[i]))
                            //        continue;
                            //    else if (i < RedPlayer.Ships.Count && !UtilFunctions.IsShipEqual(ship, RedPlayer.Ships[i]))
                            //        RedPlayer.Ships[i] = ship;
                            //    else RedPlayer.Ships.Add(ship);
                            //}
                        }
                        for (int index = RedShipCount; index < RedPlayer.Ships.Count; index++)
                        {
                            RedPlayer.Ships.RemoveAt(index);
                            myLogger.LogInfo(String.Format("redRemoveIndex: {0}", index));
                        }
                        for (int index = BlueShipCount; index < BluePlayer.Ships.Count; index++)
                        {
                            BluePlayer.Ships.RemoveAt(index);
                            myLogger.LogInfo(String.Format("blueRemoveIndex: {0}", index));
                        }
                        redShipsLabelIsBusy = false;
                        blueShipsLabelIsBusy = false;
                        myLogger.LogInfo("============= Draw Ship list ================");

                        for (int i = 0; i < RedPlayer.Ships.Count; i++)
                        {
                            myLogger.LogInfo(String.Format("RedPlayer.Ships[{0}].Type:{1}", i, RedPlayer.Ships[i].Type_s));
                        }

                        for (int i = 0; i < BluePlayer.Ships.Count; i++)
                        {
                            myLogger.LogInfo(String.Format("BluePlayer.Ships[{0}].Type:{1}", i, BluePlayer.Ships[i].Type_s));
                        }

                        foreach (var data in listOfCommunity)
                        {
                            DrawCommunity(data);
                        }

                        foreach (var data in listOfFactory)
                        {
                            DrawFactory(data);
                        }

                        foreach (var data in listOfWormhole)
                        {
                            DrawWormHole(data);
                        }
                        //listOfWormhole.Sort(
                        //    delegate (MessageOfWormhole h1, MessageOfWormhole h2)
                        //    {
                        //        int re = h1.X.CompareTo(h2.X);
                        //        if (0 == re)
                        //        {
                        //            return h1.Y.CompareTo(h2.Y);
                        //        }
                        //        return re;
                        //    }
                        //);

                        foreach (var data in listOfFort)
                        {
                            DrawFort(data);
                        }

                        foreach (var data in listOfResource)
                        {
                            DrawResource(data);
                        }
                    }
                }
            }
            finally
            {

            }
            //counter++;
        }

        private void UpdateTest(object sender, EventArgs e)
        {
            lock (drawPicLock)
            {
                counterViewModelTest++;
                if (!hasDrawn && getMapFlag)
                {
                    PureDrawMap(defaultMap);
                    hasDrawn = true;
                }
                DrawShip();
                DrawBullet();
                //if (testcounter < 30)
                //{
                //    testcounter++;
                //    if (testcounter % 3 == 0)
                //    {
                //        Ship ship = new Ship
                //        {
                //            Type = ShipType.MilitaryShip,
                //            State = ShipState.Stunned,
                //            Type_s = UtilInfo.ShipTypeNameDict[ShipType.MilitaryShip],
                //            State_s = UtilInfo.ShipStateNameDict[ShipState.Stunned]
                //        };
                //        RedPlayer.Ships.Add(ship);
                //        BluePlayer.Ships.Add(ship);
                //    }
                //}
                //DrawHome(new MessageOfHome
                //{
                //    X = 10,
                //    Y = 10,
                //    Hp = 100,
                //    TeamId = 1
                //});

                //DrawRecycleBank(new MessageOfRecycleBank
                //{
                //    X = 11,
                //    Y = 11,
                //    Hp = 100,
                //    TeamId = 2
                //});

                //DrawWormHole(new MessageOfBridge
                //{
                //    X = 12,
                //    Y = 12,
                //    Hp = 100
                //});
                //ballX += 1;
                //ballY += 1;
            }
        }



        public readonly int numOfShips = 16;
        public readonly int numOfBullets = 40;

        public double lastMoveAngle;

        public readonly float unitWidth = UtilInfo.unitWidth;
        public readonly float unitHeight = UtilInfo.unitHeight;

        public readonly int ShipStatusAttributesFontSize = 13;

        public Logger myLogger;

        public GeneralViewModel()
        {
            //ConfigData d = new();
            //d.Commands.Launched = true;
            //myLogger.LogInfo(String.Format("========={0}============", d.Commands.LaunchID));
            //d.Commands.LaunchID = d.Commands.LaunchID + 1;
            //myLogger.LogInfo(String.Format("========={0}============", d.Commands.LaunchID));
            //d.Commands.PlayerID = Convert.ToString(d.Commands.LaunchID);
            //myLogger.LogInfo(String.Format("========={0}============", d.Commands.PlayerID));
            InitiateObjects();
            Title = "THUAI7";

            installer.Data.ConfigData d = new();
            ip = d.Commands.IP;
            port = d.Commands.Port;
            playerID = Convert.ToInt64(d.Commands.PlayerID);
            teamID = Convert.ToInt64(d.Commands.TeamID);
            shipTypeID = Convert.ToInt32(d.Commands.ShipType);
            playbackFile = d.Commands.PlaybackFile;
            playbackSpeed = d.Commands.PlaybackSpeed;
            myLogger = LoggerProvider.FromFile(Path.Combine(d.InstallPath, "Logs", "Client.log"));

            MoveUpCommand = new Command(() =>
            {
                try
                {
                    if (client == null || isSpectatorMode || isPlaybackMode)
                    {
                        myLogger.LogInfo("Client is null or is SpectatorMode or isPlaybackMode");
                        return;
                    }
                    MoveMsg movemsg = new()
                    {
                        PlayerId = playerID,
                        TeamId = teamID,
                        Angle = double.Pi
                    };
                    lastMoveAngle = movemsg.Angle;
                    movemsg.TimeInMilliseconds = 50;
                    client.Move(movemsg);
                }
                catch (Exception ex)
                {
                    myLogger.LogInfo("-------- Move Exception -------");
                    myLogger.LogInfo(ex.Message);
                    /* 
                        #TODO
                        Show the error message
                    */
                }
            });

            int moveTime = 100;

            MoveDownCommand = new Command(() =>
            {
                if (client == null || isSpectatorMode || isPlaybackMode)
                {
                    myLogger.LogInfo("Client is null or is SpectatorMode or isPlaybackMode");
                    return;
                }
                MoveMsg movemsg = new()
                {
                    PlayerId = playerID,
                    TeamId = teamID,
                    Angle = double.NegativeZero
                };
                lastMoveAngle = movemsg.Angle;
                movemsg.TimeInMilliseconds = moveTime;
                client.Move(movemsg);
            });

            MoveLeftCommand = new Command(() =>
            {
                if (client == null || isSpectatorMode || isPlaybackMode)
                {
                    myLogger.LogInfo("Client is null or is SpectatorMode or isPlaybackMode");
                    return;
                }
                MoveMsg movemsg = new()
                {
                    PlayerId = playerID,
                    TeamId = teamID,
                    Angle = double.Pi * 3 / 2
                };
                lastMoveAngle = movemsg.Angle;
                movemsg.TimeInMilliseconds = moveTime;
                client.Move(movemsg);
            });

            MoveRightCommand = new Command(() =>
            {
                if (client == null || isSpectatorMode || isPlaybackMode)
                {
                    myLogger.LogInfo("Client is null or is SpectatorMode or isPlaybackMode");
                    return;
                }
                MoveMsg movemsg = new()
                {
                    PlayerId = playerID,
                    TeamId = teamID,
                    Angle = double.Pi / 2
                };
                lastMoveAngle = movemsg.Angle;
                movemsg.TimeInMilliseconds = moveTime;
                client.Move(movemsg);
            });

            MoveLeftUpCommand = new Command(() =>
            {
                if (client == null || isSpectatorMode || isPlaybackMode)
                {
                    myLogger.LogInfo("Client is null or is SpectatorMode or isPlaybackMode");
                    return;
                }
                MoveMsg movemsg = new()
                {
                    PlayerId = playerID,
                    TeamId = teamID,
                    Angle = double.Pi * 5 / 4
                };
                lastMoveAngle = movemsg.Angle;
                movemsg.TimeInMilliseconds = moveTime;
                client.Move(movemsg);
            });

            MoveRightUpCommand = new Command(() =>
            {
                if (client == null || isSpectatorMode || isPlaybackMode)
                {
                    myLogger.LogInfo("Client is null or is SpectatorMode or isPlaybackMode");
                    return;
                }
                MoveMsg movemsg = new()
                {
                    PlayerId = playerID,
                    TeamId = teamID,
                    Angle = double.Pi * 3 / 4
                };
                lastMoveAngle = movemsg.Angle;
                movemsg.TimeInMilliseconds = moveTime;
                client.Move(movemsg);
            });

            MoveLeftDownCommand = new Command(() =>
            {
                if (client == null || isSpectatorMode || isPlaybackMode)
                {
                    myLogger.LogInfo("Client is null or is SpectatorMode or isPlaybackMode");
                    return;
                }
                MoveMsg movemsg = new()
                {
                    PlayerId = playerID,
                    TeamId = teamID,
                    Angle = double.Pi * 7 / 4
                };
                lastMoveAngle = movemsg.Angle;
                movemsg.TimeInMilliseconds = moveTime;
                client.Move(movemsg);
            });

            MoveRightDownCommand = new Command(() =>
            {
                if (client == null || isSpectatorMode || isPlaybackMode)
                {
                    myLogger.LogInfo("Client is null or is SpectatorMode or isPlaybackMode");
                    return;
                }
                MoveMsg movemsg = new()
                {
                    PlayerId = playerID,
                    TeamId = teamID,
                    Angle = double.Pi / 4
                };
                lastMoveAngle = movemsg.Angle;
                movemsg.TimeInMilliseconds = moveTime;
                client.Move(movemsg);
            });

            AttackCommand = new Command(() =>
            {
                if (client == null || isSpectatorMode || isPlaybackMode)
                {
                    myLogger.LogInfo("Client is null or is SpectatorMode or isPlaybackMode");
                    return;
                }
                AttackMsg attackMsg = new()
                {
                    PlayerId = playerID,
                    TeamId = teamID,
                    Angle = lastMoveAngle
                };
                client.Attack(attackMsg);
            });

            RecoverCommand = new Command(() =>
            {
                if (client == null || isSpectatorMode || isPlaybackMode)
                {
                    myLogger.LogInfo("Client is null or is SpectatorMode or isPlaybackMode");
                    return;
                }
                RecoverMsg recoverMsg = new()
                {
                    PlayerId = playerID,
                    TeamId = teamID
                };
                client.Recover(recoverMsg);
            });

            ProduceCommand = new Command(() =>
            {
                if (client == null || isSpectatorMode || isPlaybackMode)
                {
                    myLogger.LogInfo("Client is null or is SpectatorMode or isPlaybackMode");
                    return;
                }
                IDMsg iDMsg = new()
                {
                    PlayerId = playerID,
                    TeamId = teamID
                };
                client.Produce(iDMsg);
            });

            ConstructCommand = new Command(() =>
            {
                if (client == null || isSpectatorMode || isPlaybackMode)
                {
                    myLogger.LogInfo("Client is null or is SpectatorMode or isPlaybackMode");
                    return;
                }
                ConstructMsg constructMsg = new()
                {
                    PlayerId = playerID,
                    TeamId = teamID,
                    ConstructionType = ConstructionType.Factory
                };
                client.Construct(constructMsg);
            });

            //Links = [
            //    new Link { Name = "天梯信息", Url = "" },
            //    new Link { Name = "获取更新", Url = "" },
            //    new Link { Name = "我的AI", Url = "" },
            //    new Link { Name = "配置链接", Url = "" }
            //];
            //RedPlayer.Hp = 100;
            //RedPlayer.Money = 1000;

            WormHole1HP = 100;
            WormHole2HP = 100;
            WormHole3HP = 100;


            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    MapPatchesList.Add(new MapPatch
                    {
                        PatchColor = Colors.AliceBlue,
                    });
                }
            }

            shipCircList = [];
            for (int i = 0; i < numOfShips; i++)
            {
                shipCircList.Add(new DrawCircLabel
                {
                    X = 0,
                    Y = 0,
                    Color = Colors.Transparent,
                    Text = ""
                });
            }

            bulletCircList = [];
            for (int i = 0; i < numOfBullets; i++)
            {
                bulletCircList.Add(new DrawCircLabel
                {
                    X = 0,
                    Y = 0,
                    Color = Colors.Transparent
                });
            }


            // PureDrawMap(GameMap.GameMapArray);
            //ReactToCommandline();

            myLogger.LogInfo(String.Format("ip:{0}, port:{1}, playerid:{2}, teamid:{3}, shiptype:{4}, playbackfile:{5}, playbackspeed:{6}", ip, port, playerID, teamID, shipTypeID, playbackFile, playbackSpeed));

            //Playback("E:\\program\\Project\\THUAI7\\logic\\Client\\114514.thuai7.pb", 2.0);
            if (playbackFile.Length == 0)
            {
                try
                {
                    string[] comInfo =
                    [
                        ip,
                        port,
                        Convert.ToString(playerID),
                        Convert.ToString(teamID),
                        Convert.ToString(shipTypeID),
                    ];
                    myLogger.LogInfo(string.Format("cominfo[{0}]", comInfo[0]));
                    myLogger.LogInfo(string.Format("cominfo[{0}]", comInfo[1]));
                    myLogger.LogInfo(string.Format("cominfo[{0}]", comInfo[2]));
                    myLogger.LogInfo(string.Format("cominfo[{0}]", comInfo[3]));
                    myLogger.LogInfo(string.Format("cominfo[{0}]", comInfo[4]));
                    ConnectToServer(comInfo);
                    OnReceive();
                }
                catch
                {
                    OnReceive();
                }
            }
            else
            {
                Playback(playbackFile, playbackSpeed);
            }
            //连接Server,comInfo[] 的格式：0 - ip 1 - port 2 - playerID 3 - teamID 4 - ShipType

            //ConnectToServer(new string[]{
            //    "localhost",
            //    "8888",
            //    "0",
            //    "0",
            //    "1"
            //});
            //d.Commands.Launched = true;


            // 连接Server,comInfo[]的格式：0-ip 1- port 2-playerID (>2023则为观察者模式）
            //ConnectToServer(new string[]{
            //    "localhost",
            //    "8888",
            //    "2025",
            //    "0",
            //    "1"
            //});

            //Playback("E:\\program\\Project\\THUAI7\\logic\\Server\\bin\\Debug\\net8.0\\114514.thuai7.pb", 1);

            timerViewModel = Dispatcher.CreateTimer();
            timerViewModel.Interval = TimeSpan.FromMilliseconds(50);
            timerViewModel.Tick += new EventHandler(Refresh);
            timerViewModel.Start();

            //OnReceive();
        }
    }
}
