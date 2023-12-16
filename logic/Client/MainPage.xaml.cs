using System;
using System.Windows;
using System.Threading.Tasks;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Controls;
using System.Drawing;
using Grpc.Core;
using Google.Protobuf;
using System.Runtime.CompilerServices;
using Protobuf;
using Microsoft.Maui.Controls.Shapes;

namespace Client
{
    public partial class MainPage : ContentPage
    {
        private bool UIinitiated = false;
        public MainPage()
        {
            Console.WriteLine("Hello World");
            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += new EventHandler(Refresh);
            timer.Start();
            Application.Current.UserAppTheme = AppTheme.Light;  //Light Theme Mode
            InitializeComponent();
            SetStatusBars();
            InitiateObjects();
            PureDrawMap();
            UIinitiated = true;
        }

        /* Set the StatusBars */
        private void SetStatusBars()
        {
            redPlayer = new PlayerStatusBar(MainGrid, 0, 1, 0);
            bluePlayer = new PlayerStatusBar(MainGrid, 1, 1, 1);
            gameStatusBar = new GameStatusBar(MainGrid, 0, 4);
        }
        /* initiate the Lists of Objects and CountList */
        private void InitiateObjects()
        {
            listOfAll = new List<MessageOfAll>();
            listOfShip = new List<MessageOfShip>();
            //listOfBuilding = new List<MessageOfBuilding>();
            listOfBullet = new List<MessageOfBullet>();
            listOfResource = new List<MessageOfResource>();
            listOfHome = new List<MessageOfHome>();
            countMap = new Dictionary<int, int>();
        }

        /* Get the Map to default map */
        private void GetMap(MessageOfMap obj)
        {
            int[,] map = new int[50, 50];
            try
            {
                for (int i = 0; i < 50; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        map[i, j] = Convert.ToInt32(obj.Row[i].Col[j]) + 4;//与proto一致
                    }
                }
            }
            catch
            {
                mapFlag = false;
            }
            finally
            {
                defaultMap = map;
                mapFlag = true;
            }
        }

        private void PureDrawMap()
        {
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    mapPatches[i, j] = new()
                    {
                        WidthRequest = unitWidth,
                        HeightRequest = unitHeight,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Start,
                        Margin = new Thickness(unitWidth * j, unitHeight * i, 0, 0)
                    };
                    switch ((MapPatchType)GameMap.GameMapArray[i, j])
                    {
                        case MapPatchType.RedHome:
                            mapPatches[i, j].Color = Colors.Red; break;  //Red Home
                        case MapPatchType.BlueHome:
                            mapPatches[i, j].Color = Colors.Blue; break; //Blue Home
                        case MapPatchType.Ruin:
                            mapPatches[i, j].Color = Colors.Black; break; // Ruin
                        case MapPatchType.Shadow:
                            mapPatches[i, j].Color = Colors.Gray; break; // Shadow
                        case MapPatchType.Asteroid:
                            mapPatches[i, j].Color = Colors.Brown; break; // Asteroid
                        case MapPatchType.Resource:
                            mapPatches[i, j].Color = Colors.Yellow; break; //Resource
                        case MapPatchType.Building:
                            mapPatches[i, j].Color = Colors.Orange; break; //Building
                        default:
                            break;
                    }
                    MapGrid.Children.Add(mapPatches[i, j]);
                }
            }
        }

        private void DrawMap()
        {
            resourceArray = new Label[countMap[(int)MapPatchType.Resource]];
            resourcePositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Resource]];
            buildingArray = new Label[countMap[(int)MapPatchType.Building]];
            buildingPositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Building]];

            int counterOfResource = 0;
            int counterOfBuilding = 0;

            int[,] todrawMap;
            todrawMap = defaultMap;
            for (int i = 0; i < todrawMap.GetLength(0); i++)
            {
                for (int j = 0; j < todrawMap.GetLength(1); j++)
                {
                    mapPatches[i, j] = new()
                    {
                        WidthRequest = unitWidth,
                        HeightRequest = unitHeight,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Start,
                        Margin = new Thickness(unitWidth * j, unitHeight * i, 0, 0)
                    };
                    MapPatchType mapPatchType = new MapPatchType();
                    mapPatchType = (MapPatchType)todrawMap[i, j];
                    switch (mapPatchType)
                    {
                        case MapPatchType.RedHome:
                            mapPatches[i, j].Color = Colors.Red; break;  //Red Home
                        case MapPatchType.BlueHome:
                            mapPatches[i, j].Color = Colors.Blue; break; //Blue Home
                        case MapPatchType.Ruin:
                            mapPatches[i, j].Color = Colors.Black; break; // Ruin
                        case MapPatchType.Shadow:
                            mapPatches[i, j].Color = Colors.Gray; break; // Shadow
                        case MapPatchType.Asteroid:
                            mapPatches[i, j].Color = Colors.Brown; break; // Asteroid
                        case MapPatchType.Resource:
                            mapPatches[i, j].Color = Colors.Yellow; //Resource
                            resourcePositionIndex[counterOfResource] = (i, j);
                            resourceArray[counterOfResource] = new Label()
                            {
                                FontSize = unitFontSize,
                                WidthRequest = unitWidth,
                                HeightRequest = unitHeight,
                                Text = Convert.ToString(-1),
                                HorizontalOptions = LayoutOptions.Start,
                                VerticalOptions = LayoutOptions.Start,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center,
                                BackgroundColor = Colors.Transparent
                            };
                            counterOfResource++;
                            break;

                        case MapPatchType.Building:
                            mapPatches[i, j].Color = Colors.Orange; //Building
                            buildingPositionIndex[counterOfBuilding] = (i, j);
                            buildingArray[counterOfBuilding] = new Label()
                            {
                                FontSize = unitFontSize,
                                WidthRequest = unitWidth,
                                HeightRequest = unitHeight,
                                Text = Convert.ToString(-1),
                                HorizontalOptions = LayoutOptions.Start,
                                VerticalOptions = LayoutOptions.Start,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center,
                                BackgroundColor = Colors.Transparent
                            };
                            counterOfBuilding++;
                            break;


                        default:
                            break;
                    }
                    MapGrid.Children.Add(mapPatches[i, j]);
                }
            }
            hasDrawed = true;
        }

        private async void OnReceive()
        {
            try
            {
                while (responseStream != null && await responseStream.ResponseStream.MoveNext())
                {
                    lock (drawPicLock)
                    {
                        listOfShip.Clear();
                        //listOfBuilding.Clear();
                        listOfBullet.Clear();
                        listOfResource.Clear();
                        listOfHome.Clear();
                        listOfAll.Clear();
                        MessageToClient content = responseStream.ResponseStream.Current;
                        MessageOfMap mapMassage = new MessageOfMap();
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

                                        //case MessageOfObj.MessageOfObjOneofCase.BuildingMessage:
                                        //    listOfBuilding.Add(obj.BuildingMessage);
                                        //    break;

                                        case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
                                            listOfBullet.Add(obj.BulletMessage);
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
                                //countMap.Add((int)MapPatchType.Building, listOfBuilding.Count);
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

                                        //case MessageOfObj.MessageOfObjOneofCase.BuildingMessage:
                                        //    listOfBuilding.Add(obj.BuildingMessage);
                                        //    break;

                                        case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
                                            listOfBullet.Add(obj.BulletMessage);
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
                                    //countMap.Add((int)MapPatchType.Building, listOfBuilding.Count);
                                    GetMap(mapMassage);
                                    mapMessageExist = false;
                                }
                                break;

                            case GameState.GameEnd:
                                DisplayAlert("Info", "Game End", "OK");
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

                                        case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
                                            listOfBullet.Add(obj.BulletMessage);
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
                /* 
                    #TODO
                    Show the error message
                */
            }
        }

        private int FindIndexOfResource(MessageOfResource obj)
        {
            for (int i = 0; i < listOfResource.Count; i++)
            {
                if (resourcePositionIndex[i].x == obj.X && resourcePositionIndex[i].y == obj.Y)
                {
                    return i;
                }
            }
            return -1;
        }

        //private int FindIndexOfBuilding(MessageOfBuilding obj)
        //{
        //    for (int i = 0; i < listOfBuilding.Count; i++)
        //    {
        //        if (buildingPositionIndex[i].x == obj.X && buildingPositionIndex[i].y == obj.Y)
        //        {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}

        private void Refresh(object sender, EventArgs e)
        {
            try
            {
                lock (drawPicLock)
                {
                    if (UIinitiated)
                    {
                        redPlayer.SlideLengthSet();
                        bluePlayer.SlideLengthSet();
                        gameStatusBar.SlideLengthSet();
                    }
                    if (!isClientStocked)
                    {
                        if (MapGrid.Children.Count() > 0)
                        {
                            MapGrid.Children.Clear();
                        }
                        foreach (var data in listOfAll)
                        {
                            gameStatusBar.SetGameTimeValue(data);
                        }
                        if (!hasDrawed && mapFlag)
                        {
                            DrawMap();
                        }
                        foreach (var data in listOfHome)
                        {
                            if (data.Team == PlayerTeam.Down)
                            {
                                redPlayer.SetPlayerValue(data);
                            }
                            else
                            {
                                bluePlayer.SetPlayerValue(data);
                            }
                            DrawHome(data);
                        }
                        //foreach (var data in listOfBuilding)
                        //{
                        //    if (data.BuildingType == BuildingType.Wormhole)
                        //    {
                        //        gameStatusBar.SetWormHoleValue(data);
                        //    }
                        //    DrawBuilding(data);
                        //}
                        foreach (var data in listOfBullet)
                        {
                            DrawBullet(data);
                        }
                        foreach (var data in listOfResource)
                        {
                            DrawResource(data);
                        }
                        foreach (var data in listOfShip)
                        {
                            if (data.Team == PlayerTeam.Down)
                            {
                                redPlayer.SetShipValue(data);
                            }
                            else
                            {
                                bluePlayer.SetShipValue(data);
                            }
                            // TODO: Dynamic change the ships' label
                            DrawShip(data);
                        }
                    }
                }
            }
            finally
            {

            }
            counter++;
        }

        private void DrawHome(MessageOfHome data)
        {
            Ellipse iconOfHome = new()
            {
                WidthRequest = 2 * characterRadiusTimes * unitWidth,
                HeightRequest = 2 * characterRadiusTimes * unitHeight,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0),
                Fill = data.Team == PlayerTeam.Down ? Colors.Red : Colors.Blue
            };
            MapGrid.Children.Add(iconOfHome);
        }

        // private void DrawBuilding(MessageOfBuilding data)
        // {
        //     int hp = data.Hp;
        //     //TODO: calculate the percentage of Hp
        //     int idx = FindIndexOfBuilding(data);
        //     buildingArray[idx].FontSize = unitFontSize;
        //     buildingArray[idx].WidthRequest = unitWidth;
        //     buildingArray[idx].HeightRequest = unitHeight;
        //     buildingArray[idx].Text = Convert.ToString(hp);
        //     buildingArray[idx].Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0);
        //     switch (data.BuildingType)
        //     {
        //         case BuildingType.Factory:
        //             buildingArray[idx].BackgroundColor = Colors.Chocolate;
        //             break;
        //         case BuildingType.Community:
        //             buildingArray[idx].BackgroundColor = Colors.Green;
        //             break;
        //         case BuildingType.Fortress:
        //             buildingArray[idx].BackgroundColor = Colors.Azure;
        //             break;
        //         case BuildingType.Wormhole:
        //             buildingArray[idx].BackgroundColor = Colors.Purple;
        //             break;
        //     }
        //     MapGrid.Children.Add(buildingArray[idx]);
        // }

        private void DrawBullet(MessageOfBullet data)
        {
            Ellipse iconOfBullet = new()
            {
                WidthRequest = 2 * bulletRadiusTimes * unitWidth,
                HeightRequest = 2 * bulletRadiusTimes * unitHeight,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * bulletRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * bulletRadiusTimes, 0, 0),
                Fill = data.Team == PlayerTeam.Down ? Colors.Red : Colors.Blue
            };
            switch (data.Type)
            {
                case BulletType.Plasma:
                    iconOfBullet.Fill = Colors.Yellow;
                    break;
                case BulletType.Laser:
                    iconOfBullet.Fill = Colors.Orange;
                    break;
                case BulletType.Missile:
                    iconOfBullet.Fill = Colors.Purple;
                    break;
                case BulletType.ElectricArc:
                    iconOfBullet.Fill = Colors.Green;
                    break;
            }
            MapGrid.Children.Add(iconOfBullet);
        }

        private void DrawResource(MessageOfResource data)
        {
            int idx = FindIndexOfResource(data);
            resourceArray[idx].FontSize = unitFontSize;
            resourceArray[idx].WidthRequest = unitWidth;
            resourceArray[idx].HeightRequest = unitHeight;
            resourceArray[idx].Text = Convert.ToString(data.Progress);
            resourceArray[idx].Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0);
            MapGrid.Children.Add(resourceArray[idx]);
        }

        private void DrawShip(MessageOfShip data)
        {
            Ellipse iconOfShip = new()
            {
                WidthRequest = 2 * characterRadiusTimes * unitWidth,
                HeightRequest = 2 * characterRadiusTimes * unitHeight,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0),
                Fill = data.Team == PlayerTeam.Down ? Colors.Red : Colors.Blue
            };
            Label nameOfShip = new()
            {
                FontSize = unitFontSize,
                WidthRequest = unitWidth,
                HeightRequest = unitHeight,
                Text = data.ShipType.ToString()[0] + data.ShipId.ToString(),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                BackgroundColor = Colors.Transparent,
                Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0)
            };
            MapGrid.Children.Add(iconOfShip);
            MapGrid.Children.Add(nameOfShip);
        }

        private readonly IDispatcherTimer timer;
        private long counter;

        AsyncServerStreamingCall<MessageToClient>? responseStream;
        private bool isClientStocked;

        private PlayerStatusBar redPlayer;
        private PlayerStatusBar bluePlayer;
        private GameStatusBar gameStatusBar;

        private bool isPlaybackMode;
        private long playerID;
        private double unit;
        private double unitFontSize = 10;
        private double unitHeight = 10.6;
        private double unitWidth = 10.6;
        private readonly BoxView[,] mapPatches = new BoxView[50, 50];
        private readonly double characterRadiusTimes = 400;
        private readonly double bulletRadiusTimes = 200;




        private List<MessageOfAll> listOfAll;
        private List<MessageOfShip> listOfShip;
        //private List<MessageOfBuilding> listOfBuilding;
        private List<MessageOfBullet> listOfBullet;
        private List<MessageOfResource> listOfResource;
        private List<MessageOfHome> listOfHome;
        private int gameTime;

        private Label[] resourceArray;
        private (int x, int y)[] resourcePositionIndex;
        private Label[] buildingArray;
        private (int x, int y)[] buildingPositionIndex;
        // private Label[] factoryArray;
        // private int[] factoryPositionIndex;
        // private Label[] communityArray;
        // private int[] communityPositionIndex;
        // private Label[] fortressArray;
        // private int[] fortressPositionIndex;
        // private Label[] wormHoleArray;
        // private int[] wormHolePositionIndex;
        private Dictionary<int, int> countMap;

        private object drawPicLock = new object();

        private bool mapFlag = false;
        private bool hasDrawed = false;
        public int[,] defaultMap = new int[,] {
            { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 },//6墙,1-5出生点
            { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },//7草
            { 6, 0, 0, 0, 0, 0, 0, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 6 },//8机
            { 6, 0, 0, 0, 0, 6, 0, 6, 7, 0, 0, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0, 0, 0, 6 },//9大门
            { 6, 0, 0, 0, 0, 6, 6, 6, 6, 7, 0, 0, 0, 0, 0, 15, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 15, 0, 0, 0, 6 },//10紧急出口
            { 6, 6, 0, 0, 0, 0, 9, 6, 6, 7, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 6, 6, 7, 7, 6, 6, 6, 6, 6, 6, 11, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },//11窗
            { 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 7, 7, 6, 6, 7, 7, 6, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 13, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6 },//12-14门
            { 6, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 0, 0, 0, 7, 7, 7, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 6 },//15箱
            { 6, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 6 },
            { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0, 6 },
            { 6, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 7, 7, 6, 0, 6 },
            { 6, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 7, 6, 0, 6 },
            { 6, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 6, 12, 6, 6, 6, 6, 6, 6, 11, 6, 6, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 6, 0, 6 },
            { 6, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 15, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0, 6 },
            { 6, 0, 0, 0, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0, 0, 0, 7, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 6 },
            { 6, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 7, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 0, 0, 0, 0, 0, 6 },
            { 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 0, 0, 0, 6 },
            { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 11, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6, 6, 7, 0, 0, 6 },
            { 6, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 11, 6, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 7, 0, 0, 6 },
            { 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 6, 7, 0, 0, 6 },
            { 6, 7, 7, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 6, 6, 0, 0, 0, 0, 0, 0, 7, 7, 0, 0, 6, 6, 6, 6, 6, 6, 0, 0, 0, 6 },
            { 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 0, 0, 5, 0, 7, 7, 6, 0, 0, 0, 0, 0, 0, 7, 6, 6, 6, 6, 15, 0, 0, 0, 0, 0, 0, 0, 6 },
            { 6, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 6, 7, 7, 0, 0, 0, 0, 7, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 7, 6, 0, 0, 0, 6 },
            { 6, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 7, 7, 0, 0, 0, 0, 0, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 7, 7, 6, 6, 0, 10, 0, 6 },
            { 6, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 6, 6, 6, 6, 7, 0, 0, 0, 6 },
            { 6, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 6, 7, 0, 2, 0, 0, 6 },
            { 6, 0, 6, 0, 0, 0, 0, 0, 0, 6, 11, 6, 6, 6, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 11, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },
            { 6, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },
            { 6, 0, 11, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 6, 12, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 0, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },
            { 6, 0, 6, 0, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 15, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 7, 7, 0, 0, 0, 0, 6 },
            { 6, 0, 6, 7, 0, 0, 0, 8, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 0, 0, 0, 0, 6 },
            { 6, 0, 6, 6, 6, 6, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6,6, 7, 0, 0, 0, 6 },
            { 6, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 11, 0, 0, 0, 0, 0, 0, 7, 7, 7, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 7, 0, 0, 0, 6 },
            { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 6, 7, 0, 0, 0, 6 },
            { 6, 0, 0, 0, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 6 },
            { 6, 0, 0, 0, 6, 6, 6, 6, 6, 7, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 6 },
            { 6, 6, 0, 0, 7, 7, 6, 7, 7, 0, 0, 0, 0, 0, 0, 0, 14, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0, 0, 6 },
            { 6, 6, 15, 0, 0, 0, 7, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 11, 6, 0, 0, 0, 0, 0, 6 },
            { 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6,6, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },
            { 6, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 15, 0, 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },
            { 6, 0, 0, 0, 0, 0, 0, 0, 6, 7, 0, 0,8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 6 },
            { 6, 0, 0, 0, 0, 0, 0, 0, 6, 7, 7, 0, 0, 0, 6, 6, 6, 11, 6, 0, 0, 6, 6, 6, 7, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 6 },
            { 6, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 6, 0, 6, 7, 7, 6, 7, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 14, 6, 6, 6, 0, 0, 0, 0, 0, 7, 0, 0, 6, 0, 6 },
            { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 0, 7, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 7, 6, 0, 6, 6, 0, 6 },
            { 6, 0, 0, 0, 0, 0, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 7, 0, 0, 7, 6, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 7, 6, 6, 6, 0, 0, 6 },
            { 6, 0, 0, 0, 0, 0, 6, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 0, 0, 0, 7, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },
            { 6, 0, 0, 0, 0, 6, 6, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 11, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 6 },
            { 6, 0, 0, 0, 6, 6, 6, 6, 6, 7, 0, 0, 0, 10, 0, 0, 0, 0, 6, 6, 7, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 6, 0, 0, 0, 0, 7, 6, 6, 0, 0, 0, 6 },
            { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 7, 7, 6, 6, 0, 0, 0, 6 },
            { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 }
            };

        enum MapPatchType
        {
            Space = 0,
            RedHome = 1,
            BlueHome = 2,
            Ruin = 3,
            Shadow = 4,
            Asteroid = 5,
            Resource = 6,
            Building = 7
        };

    }
}