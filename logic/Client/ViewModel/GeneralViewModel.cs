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
using CommunityToolkit.Mvvm.ComponentModel;

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

        private TestClass testClass;
        public TestClass TestClass
        {
            get
            {
                return testClass;
            }
            set
            {
                testClass = value;
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
        private ShipType shipType;
        private long teamID;
        AvailableService.AvailableServiceClient? client;
        AsyncServerStreamingCall<MessageToClient>? responseStream;
        bool isSpectatorMode = false;
        // 连接Server,comInfo[]的格式：0-ip 1- port 2-playerID 3-teamID 4-shipType
        public void ConnectToServer(string[] comInfo)
        {
            if (!isSpectatorMode && comInfo.Length != 5 || isSpectatorMode && comInfo.Length != 4)
            {
                throw new Exception("注册信息有误！");
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
            playerMsg.X = 0;
            playerMsg.Y = 0;
            if (!isSpectatorMode)
            {
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

        int testcounter = 0;
        private void UpdateTest(object sender, EventArgs e)
        {
            counterViewModelTest++;
            MapPatchesList[5].Text = "SS";
            MapPatchesList[4].Text = "SS";
            MapPatchesList[2].Text = counterViewModelTest.ToString();
            if (!hasDrawn)
            {
                PureDrawMap(GameMap.GameMapArray);
                hasDrawn = true;
            }
            if (testcounter < 30)
            {
                testcounter++;
                if (testcounter % 3 == 0)
                {
                    Ship ship = new Ship
                    {
                        Type_s = "CivilShip",
                        State_s = "Idle",
                        ArmorModule_s = "LightArmor"
                    };
                    RedPlayer.Ships.Add(ship);
                    BluePlayer.Ships.Add(ship);
                }
            }
        }

        public GeneralViewModel()
        {
            Title = "THUAI7";
            Links = [
                new Link { Name = "天梯信息", Url = "" },
                new Link { Name = "获取更新", Url = "" },
                new Link { Name = "我的AI", Url = "" },
                new Link { Name = "配置链接", Url = "" }
            ];
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
                        Text = Convert.ToString(i) + Convert.ToString(j)
                    });
                }
            }

            testPatch = new MapPatch();
            TestPatch.PatchColor = Colors.Black;
            TestPatch.Text = "SS";
            testClass = new TestClass();
            TestClass.Name = "Test";
            timerViewModel = Dispatcher.CreateTimer();
            timerViewModel.Interval = TimeSpan.FromMilliseconds(5);
            timerViewModel.Tick += new EventHandler(UpdateTest);
            timerViewModel.Start();
        }
    }
}
