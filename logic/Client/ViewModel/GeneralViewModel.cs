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
                return links ?? (links = new List<Link>());
            }
            set
            {
                links = value;
                OnPropertyChanged();
            }
        }

        private long playerID;
        private ShipType ShipType;
        private long teamID;
        AvailableService.AvailableServiceClient? client;
        AsyncServerStreamingCall<MessageToClient>? responseStream;
        bool isSpectatorMode = false;
        // 连接Server,comInfo[]的格式：0-ip 1- port 2-playerID 3-teamID 4-ShipType
        public void ConnectToServer(string[] comInfo)
        {
            if (!isSpectatorMode && comInfo.Length != 5 || isSpectatorMode && comInfo.Length != 4)
            {
                throw new Exception("Error Registration Information！");
            }
            playerID = Convert.ToInt64(comInfo[2]);
            teamID = Convert.ToInt64(comInfo[3]);
            string connect = new string(comInfo[0]);
            connect += ':';
            connect += comInfo[1];
            Channel channel = new Channel(connect, ChannelCredentials.Insecure);
            client = new AvailableService.AvailableServiceClient(channel);
            PlayerMsg playerMsg = new PlayerMsg();
            playerMsg.PlayerId = playerID;
            playerMsg.TeamId = teamID;
            //playerMsg.X = 0;
            //playerMsg.Y = 0;
            if (!isSpectatorMode)
            {
                ShipType = Convert.ToInt64(comInfo[4]) switch
                {
                    0 => ShipType.NullShipType,
                    1 => ShipType.CivilianShip,
                    2 => ShipType.MilitaryShip,
                    3 => ShipType.FlagShip,
                    _ => ShipType.NullShipType
                };
                playerMsg.ShipType = ShipType;
            }
            responseStream = client.AddPlayer(playerMsg);
            isClientStocked = false;
        }

        private void Playback(string fileName, double pbSpeed = 2.0)
        {
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
                System.Diagnostics.Debug.WriteLine("============= OnReceiving Server Stream ================");
                if (responseStream != null)
                    System.Diagnostics.Debug.WriteLine("============= responseStream != null ================");

                if (await responseStream.ResponseStream.MoveNext())
                    System.Diagnostics.Debug.WriteLine("============= responseStream.ResponseStream.MoveNext() ================");

                while (responseStream != null && await responseStream.ResponseStream.MoveNext())
                {
                    System.Diagnostics.Debug.WriteLine("============= Receiving Server Stream ================");
                    //lock (drawPicLock)
                    {
                        ballX += 20;
                        ballY += 20;

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
                                foreach (var obj in content.ObjMessage)
                                {
                                    switch (obj.MessageOfObjCase)
                                    {
                                        case MessageOfObj.MessageOfObjOneofCase.ShipMessage:
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

                                        case MessageOfObj.MessageOfObjOneofCase.MapMessage:
                                            mapMassage = obj.MapMessage;
                                            break;
                                    }
                                }
                                listOfAll.Add(content.AllMessage);
                                break;
                        }
                    }
                    if (responseStream == null)
                    {
                        throw new Exception("Unconnected");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("-----------------------------");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                /* 
                    #TODO
                    Show the error message
                */
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
                            //PureDrawMap(defaultMap);
                            hasDrawn = true;
                        }

                        foreach (var data in listOfAll)
                        {
                            RedPlayer.Money = data.RedTeamEnergy;
                            RedPlayer.Hp = data.RedHomeHp;
                            RedPlayer.Score = data.RedTeamScore;
                            BluePlayer.Money = data.BlueTeamEnergy;
                            BluePlayer.Hp = data.BlueHomeHp;
                            BluePlayer.Score = data.BlueTeamScore;
                        }

                        foreach (var data in listOfHome)
                        {
                            DrawHome(data);
                            if (data.TeamId == (long)PlayerTeam.Red)
                            {
                                RedPlayer.Team = data.TeamId;
                            }
                            else if (data.TeamId == (long)PlayerTeam.Blue)
                            {
                                BluePlayer.Team = data.TeamId;
                            }
                        }

                        foreach (var data in listOfShip)
                        {
                            if (data.TeamId == (long)PlayerTeam.Red)
                            {
                                Ship ship = new Ship
                                {
                                    Type = data.ShipType,
                                    State = data.ShipState,
                                    ArmorModule = data.ArmorType,
                                    ShieldModule = data.ShieldType,
                                    WeaponModule = data.WeaponType,
                                    ProducerModule = data.ProducerType,
                                    ConstuctorModule = data.ConstructorType,
                                    Type_s = UtilInfo.ShipTypeNameDict[data.ShipType],
                                    State_s = UtilInfo.ShipStateNameDict[data.ShipState],
                                    ArmorModule_s = UtilInfo.ShipArmorTypeNameDict[data.ArmorType],
                                    ShieldModule_s = UtilInfo.ShipShieldTypeNameDict[data.ShieldType],
                                    WeaponModule_s = UtilInfo.ShipWeaponTypeNameDict[data.WeaponType],
                                    ConstuctorModule_s = UtilInfo.ShipConstructorNameDict[data.ConstructorType],
                                    ProducerModule_s = UtilInfo.ShipProducerTypeNameDict[data.ProducerType]
                                };
                                RedPlayer.Ships.Add(ship);
                            }
                            else if (data.TeamId == (long)PlayerTeam.Blue)
                            {
                                Ship ship = new Ship
                                {
                                    Type = data.ShipType,
                                    State = data.ShipState,
                                    ArmorModule = data.ArmorType,
                                    ShieldModule = data.ShieldType,
                                    WeaponModule = data.WeaponType,
                                    ProducerModule = data.ProducerType,
                                    ConstuctorModule = data.ConstructorType,
                                    Type_s = UtilInfo.ShipTypeNameDict[data.ShipType],
                                    State_s = UtilInfo.ShipStateNameDict[data.ShipState],
                                    ArmorModule_s = UtilInfo.ShipArmorTypeNameDict[data.ArmorType],
                                    ShieldModule_s = UtilInfo.ShipShieldTypeNameDict[data.ShieldType],
                                    WeaponModule_s = UtilInfo.ShipWeaponTypeNameDict[data.WeaponType],
                                    ConstuctorModule_s = UtilInfo.ShipConstructorNameDict[data.ConstructorType],
                                    ProducerModule_s = UtilInfo.ShipProducerTypeNameDict[data.ProducerType]
                                };
                                BluePlayer.Ships.Add(ship);
                            }
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
            counterViewModelTest++;
            if (!hasDrawn && getMapFlag)
            {
                PureDrawMap(defaultMap);
                hasDrawn = true;
            }
            if (testcounter < 30)
            {
                testcounter++;
                if (testcounter % 3 == 0)
                {
                    Ship ship = new Ship
                    {
                        Type = ShipType.MilitaryShip,
                        State = ShipState.Stunned,
                        //Type_s = UtilInfo.ShipTypeNameDict[ShipType.MilitaryShip],
                        //State_s = UtilInfo.ShipStateNameDict[ShipState.Stunned]
                    };
                    RedPlayer.Ships.Add(ship);
                    BluePlayer.Ships.Add(ship);
                }
            }
            DrawHome(new MessageOfHome
            {
                X = 10,
                Y = 10,
                Hp = 100,
                TeamId = 1
            });

            //DrawFactory(new MessageOfFactory
            //{
            //    X = 11,
            //    Y = 11,
            //    Hp = 100,
            //    TeamId = 2
            //});

            DrawWormHole(new MessageOfWormhole
            {
                X = 12,
                Y = 12,
                Hp = 100
            });

            //ballX += 1;
            //ballY += 1;
        }

        public GeneralViewModel()
        {
            InitiateObjects();
            Title = "THUAI7";
            //Links = [
            //    new Link { Name = "天梯信息", Url = "" },
            //    new Link { Name = "获取更新", Url = "" },
            //    new Link { Name = "我的AI", Url = "" },
            //    new Link { Name = "配置链接", Url = "" }
            //];
            RedPlayer.Hp = 100;
            RedPlayer.Money = 1000;

            Ship ship = new Ship
            {
                Type_s = "CivilShip",
                State_s = "Idle",
                ArmorModule_s = "LightArmor"
            };
            RedPlayer.Ships.Add(ship);

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
            PureDrawMap(GameMap.GameMapArray);
            //ReactToCommandline();



            ConnectToServer(new string[]{
                "127.0.0.1",
                "8888",
                "0",
                "1",
                "1"
            });

            timerViewModel = Dispatcher.CreateTimer();
            timerViewModel.Interval = TimeSpan.FromMilliseconds(5);
            timerViewModel.Tick += new EventHandler(UpdateTest);
            timerViewModel.Start();

            OnReceive();
        }
    }
}
