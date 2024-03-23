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
using System.Linq.Expressions;
using Client.ViewModel;
using Client.Interact;
using Client.Model;
using System.Reflection;

namespace Client
{
    public partial class MainPage : ContentPage
    {
        private bool UIinitiated = false;
        GeneralViewModel viewModel;
        public MainPage()
        {
            viewModel = new GeneralViewModel();
            Console.WriteLine("Hello World");
            BindingContext = viewModel;
            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += new EventHandler(TestRefresh);
            //timer.Tick += new EventHandler(Refresh);
            timer.Start();
            Application.Current.UserAppTheme = AppTheme.Light;  //Light Theme Mode
            InitializeComponent();

            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    mapPatches_[i, j] = new()
                    {
                        WidthRequest = unitWidth,
                        HeightRequest = unitHeight,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Start,
                        Margin = new Thickness(unitWidth * j, unitHeight * i, 0, 0),
                        FontSize = 5
                    };
                    int index = i * 50 + j;
                    mapPatches_[i, j].SetBinding(Label.BackgroundColorProperty, new Binding($"MapPatchesList[{index}].PatchColor"));
                    mapPatches_[i, j].SetBinding(Label.TextProperty, new Binding($"MapPatchesList[{index}].Text"));
                    mapPatches_[i, j].SetBinding(Label.TextColorProperty, new Binding($"MapPatchesList[{index}].TextColor"));
                    MapGrid.Children.Add(mapPatches_[i, j]);
                    MapGrid.SetColumn(mapPatches_[i, j], i);
                    MapGrid.SetRow(mapPatches_[i, j], j);
                }
            }

            //CommandLineProcess.StartProcess();
            //string[] args = Environment.GetCommandLineArgs();
            //foreach(string arg in args)
            //{
            //    System.Diagnostics.Debug.WriteLine(arg);
            //}
            //PureDrawMap(viewModel);
            //InitiateObjects();
            UIinitiated = true;
        }
        private Label[,] mapPatches_ = new Label[50, 50];
        private readonly IDispatcherTimer timer;
        private long counter;
        private double unitWidth = 10;
        private double unitHeight = 10;
        private void TestRefresh(object sender, EventArgs e)
        {
            counter++;
            DrawGraphicsView.Invalidate();
        }



        //private void PureDrawMap(GeneralViewModel viewModel)
        //{
        //    for (int i = 0; i < 50; i++)
        //    {
        //        for (int j = 0; j < 50; j++)
        //        {
        //            switch ((MapPatchType)GameMap.GameMapArray[i, j])
        //            {
        //                case MapPatchType.RedHome:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Red; break;  //Red Home
        //                case MapPatchType.BlueHome:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Blue; break; //Blue Home
        //                case MapPatchType.Ruin:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Black; break; // Ruin
        //                case MapPatchType.Grass:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Gray; break; // Grass
        //                case MapPatchType.River:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Brown; break; // River
        //                case MapPatchType.Resource:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Yellow; break; //Resource
        //                case MapPatchType.RecycleBank:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Orange; break; //Factiry
        //                case MapPatchType.ChargeStation:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Chocolate; break; //ChargeStation
        //                case MapPatchType.SignalTower:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Azure; break; //SignalTower
        //                default:
        //                    break;
        //            }

        //        }
        //    }
        //}

        //private void DrawMap()
        //{
        //    resourceArray = new Label[countMap[(int)MapPatchType.Resource]];
        //    resourcePositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Resource]];
        //    RecycleBankArray = new Label[countMap[(int)MapPatchType.RecycleBank]];
        //    RecycleBankPositionIndex = new (int x, int y)[countMap[(int)MapPatchType.RecycleBank]];
        //    ChargeStationArray = new Label[countMap[(int)MapPatchType.ChargeStation]];
        //    ChargeStationPositionIndex = new (int x, int y)[countMap[(int)MapPatchType.ChargeStation]];
        //    signaltowerArray = new Label[countMap[(int)MapPatchType.SignalTower]];
        //    signaltowerPositionIndex = new (int x, int y)[countMap[(int)MapPatchType.SignalTower]];


        //    int counterOfResource = 0;
        //    int counterOfRecycleBank = 0;
        //    int counterOfChargeStation = 0;
        //    int counterOfSignalTower = 0;


        //    int[,] todrawMap;
        //    todrawMap = defaultMap;
        //    for (int i = 0; i < todrawMap.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < todrawMap.GetLength(1); j++)
        //        {
        //            mapPatches[i, j] = new()
        //            {
        //                WidthRequest = unitWidth,
        //                HeightRequest = unitHeight,
        //                HorizontalOptions = LayoutOptions.Start,
        //                VerticalOptions = LayoutOptions.Start,
        //                Margin = new Thickness(unitWidth * j, unitHeight * i, 0, 0)
        //            };
        //            MapPatchType mapPatchType = (MapPatchType)todrawMap[i, j];
        //            switch (mapPatchType)
        //            {
        //                case MapPatchType.RedHome:
        //                    mapPatches[i, j].Color = Colors.Red; break;  //Red Home
        //                case MapPatchType.BlueHome:
        //                    mapPatches[i, j].Color = Colors.Blue; break; //Blue Home
        //                case MapPatchType.Ruin:
        //                    mapPatches[i, j].Color = Colors.Black; break; // Ruin
        //                case MapPatchType.Grass:
        //                    mapPatches[i, j].Color = Colors.Gray; break; // Grass
        //                case MapPatchType.River:
        //                    mapPatches[i, j].Color = Colors.Brown; break; // River
        //                case MapPatchType.Resource:
        //                    mapPatches[i, j].Color = Colors.Yellow; //Resource
        //                    resourcePositionIndex[counterOfResource] = (i, j);
        //                    resourceArray[counterOfResource] = new Label()
        //                    {
        //                        FontSize = unitFontSize,
        //                        WidthRequest = unitWidth,
        //                        HeightRequest = unitHeight,
        //                        Text = Convert.ToString(-1),
        //                        HorizontalOptions = LayoutOptions.Start,
        //                        VerticalOptions = LayoutOptions.Start,
        //                        HorizontalTextAlignment = TextAlignment.Center,
        //                        VerticalTextAlignment = TextAlignment.Center,
        //                        BackgroundColor = Colors.Transparent
        //                    };
        //                    counterOfResource++;
        //                    break;

        //                case MapPatchType.RecycleBank:
        //                    mapPatches[i, j].Color = Colors.Orange; //RecycleBank
        //                    RecycleBankPositionIndex[counterOfRecycleBank] = (i, j);
        //                    RecycleBankArray[counterOfRecycleBank] = new Label()
        //                    {
        //                        FontSize = unitFontSize,
        //                        WidthRequest = unitWidth,
        //                        HeightRequest = unitHeight,
        //                        Text = Convert.ToString(100),
        //                        HorizontalOptions = LayoutOptions.Start,
        //                        VerticalOptions = LayoutOptions.Start,
        //                        HorizontalTextAlignment = TextAlignment.Center,
        //                        VerticalTextAlignment = TextAlignment.Center,
        //                        BackgroundColor = Colors.Transparent
        //                    };
        //                    counterOfRecycleBank++;
        //                    break;

        //                case MapPatchType.ChargeStation:
        //                    mapPatches[i, j].Color = Colors.Chocolate; //ChargeStation
        //                    RecycleBankPositionIndex[counterOfChargeStation] = (i, j);
        //                    RecycleBankArray[counterOfChargeStation] = new Label()
        //                    {
        //                        FontSize = unitFontSize,
        //                        WidthRequest = unitWidth,
        //                        HeightRequest = unitHeight,
        //                        Text = Convert.ToString(100),
        //                        HorizontalOptions = LayoutOptions.Start,
        //                        VerticalOptions = LayoutOptions.Start,
        //                        HorizontalTextAlignment = TextAlignment.Center,
        //                        VerticalTextAlignment = TextAlignment.Center,
        //                        BackgroundColor = Colors.Transparent
        //                    };
        //                    counterOfChargeStation++;
        //                    break;

        //                case MapPatchType.SignalTower:
        //                    mapPatches[i, j].Color = Colors.Azure; //SignalTower
        //                    RecycleBankPositionIndex[counterOfSignalTower] = (i, j);
        //                    RecycleBankArray[counterOfSignalTower] = new Label()
        //                    {
        //                        FontSize = unitFontSize,
        //                        WidthRequest = unitWidth,
        //                        HeightRequest = unitHeight,
        //                        Text = Convert.ToString(-1),
        //                        HorizontalOptions = LayoutOptions.Start,
        //                        VerticalOptions = LayoutOptions.Start,
        //                        HorizontalTextAlignment = TextAlignment.Center,
        //                        VerticalTextAlignment = TextAlignment.Center,
        //                        BackgroundColor = Colors.Transparent
        //                    };
        //                    counterOfSignalTower++;
        //                    break;

        //                default:
        //                    break;
        //            }
        //            //MapGrid.Children.Add(mapPatches[i, j]);
        //        }
        //    }
        //}

        //private async void OnReceive()
        //{
        //    try
        //    {
        //        CancellationToken cts = new CancellationToken();
        //        while (responseStream != null && await responseStream.ResponseStream.MoveNext(cts))
        //        {
        //            lock (drawPicLock)
        //            {
        //                listOfSweeper.Clear();
        //                //listOfBuilding.Clear();
        //                listOfBullet.Clear();
        //                listOfResource.Clear();
        //                listOfHome.Clear();
        //                listOfAll.Clear();
        //                MessageToClient content = responseStream.ResponseStream.Current;
        //                MessageOfMap mapMassage = new();
        //                bool mapMessageExist = false;
        //                switch (content.GameState)
        //                {
        //                    case GameState.GameStart:
        //                        foreach (var obj in content.ObjMessage)
        //                        {
        //                            switch (obj.MessageOfObjCase)
        //                            {
        //                                case MessageOfObj.MessageOfObjOneofCase.SweeperMessage:
        //                                    listOfSweeper.Add(obj.SweeperMessage);
        //                                    break;

        //                                //case MessageOfObj.MessageOfObjOneofCase.BuildingMessage:
        //                                //    listOfBuilding.Add(obj.BuildingMessage);
        //                                //    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
        //                                    listOfBullet.Add(obj.BulletMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.BombedBulletMessage:
        //                                    listOfBombedBullet.Add(obj.BombedBulletMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.RecyclebankMessage:
        //                                    listOfRecycleBank.Add(obj.RecyclebankMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.ChargestationMessage:
        //                                    listOfChargeStation.Add(obj.ChargestationMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.SignaltowerMessage:
        //                                    listOfSignalTower.Add(obj.SignaltowerMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.GarbageMessage:
        //                                    listOfResource.Add(obj.GarbageMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.HomeMessage:
        //                                    listOfHome.Add(obj.HomeMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.MapMessage:
        //                                    mapMassage = obj.MapMessage;
        //                                    break;
        //                            }
        //                        }
        //                        listOfAll.Add(content.AllMessage);
        //                        countMap.Clear();
        //                        countMap.Add((int)MapPatchType.Resource, listOfResource.Count);
        //                        countMap.Add((int)MapPatchType.RecycleBank, listOfRecycleBank.Count);
        //                        countMap.Add((int)MapPatchType.ChargeStation, listOfChargeStation.Count);
        //                        countMap.Add((int)MapPatchType.SignalTower, listOfSignalTower.Count);
        //                        //countMap.Add((int)MapPatchType.Building, listOfBuilding.Count);
        //                        GetMap(mapMassage);
        //                        break;
        //                    case GameState.GameRunning:
        //                        foreach (var obj in content.ObjMessage)
        //                        {
        //                            switch (obj.MessageOfObjCase)
        //                            {
        //                                case MessageOfObj.MessageOfObjOneofCase.SweeperMessage:
        //                                    listOfSweeper.Add(obj.SweeperMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.RecyclebankMessage:
        //                                    listOfRecycleBank.Add(obj.RecyclebankMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.ChargestationMessage:
        //                                    listOfChargeStation.Add(obj.ChargestationMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.SignaltowerMessage:
        //                                    listOfSignalTower.Add(obj.SignaltowerMessage);
        //                                    break;

        //                                //case MessageOfObj.MessageOfObjOneofCase.BuildingMessage:
        //                                //    listOfBuilding.Add(obj.BuildingMessage);
        //                                //    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
        //                                    listOfBullet.Add(obj.BulletMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.BombedBulletMessage:
        //                                    listOfBombedBullet.Add(obj.BombedBulletMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.GarbageMessage:
        //                                    listOfResource.Add(obj.GarbageMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.HomeMessage:
        //                                    listOfHome.Add(obj.HomeMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.MapMessage:
        //                                    mapMassage = obj.MapMessage;
        //                                    mapMessageExist = true;
        //                                    break;
        //                            }
        //                        }
        //                        listOfAll.Add(content.AllMessage);
        //                        if (mapMessageExist)
        //                        {
        //                            countMap.Clear();
        //                            countMap.Add((int)MapPatchType.Resource, listOfResource.Count);
        //                            countMap.Add((int)MapPatchType.RecycleBank, listOfRecycleBank.Count);
        //                            countMap.Add((int)MapPatchType.ChargeStation, listOfChargeStation.Count);
        //                            countMap.Add((int)MapPatchType.SignalTower, listOfSignalTower.Count);
        //                            GetMap(mapMassage);
        //                            mapMessageExist = false;
        //                        }
        //                        break;

        //                    case GameState.GameEnd:
        //                        //DisplayAlert("Info", "Game End", "OK");
        //                        foreach (var obj in content.ObjMessage)
        //                        {
        //                            switch (obj.MessageOfObjCase)
        //                            {
        //                                case MessageOfObj.MessageOfObjOneofCase.SweeperMessage:
        //                                    listOfSweeper.Add(obj.SweeperMessage);
        //                                    break;

        //                                //case MessageOfObj.MessageOfObjOneofCase.BuildingMessage:
        //                                //    listOfBuilding.Add(obj.BuildingMessage);
        //                                //    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.RecyclebankMessage:
        //                                    listOfRecycleBank.Add(obj.RecyclebankMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.ChargestationMessage:
        //                                    listOfChargeStation.Add(obj.ChargestationMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.SignaltowerMessage:
        //                                    listOfSignalTower.Add(obj.SignaltowerMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
        //                                    listOfBullet.Add(obj.BulletMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.BombedBulletMessage:
        //                                    listOfBombedBullet.Add(obj.BombedBulletMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.GarbageMessage:
        //                                    listOfResource.Add(obj.GarbageMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.HomeMessage:
        //                                    listOfHome.Add(obj.HomeMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.MapMessage:
        //                                    mapMassage = obj.MapMessage;
        //                                    break;
        //                            }
        //                        }
        //                        listOfAll.Add(content.AllMessage);
        //                        break;
        //                }
        //            }
        //            if (responseStream == null)
        //            {
        //                throw new Exception("Unconnected");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        /* 
        //            #TODO
        //            Show the error message
        //        */
        //    }
        //}

        //private int FindIndexOfResource(MessageOfGarbage obj)
        //{
        //    for (int i = 0; i < listOfResource.Count; i++)
        //    {
        //        if (resourcePositionIndex[i].x == obj.X && resourcePositionIndex[i].y == obj.Y)
        //        {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}

        //private int FindIndexOfRecycleBank(MessageOfRecycleBank obj)
        //{
        //    for (int i = 0; i < listOfRecycleBank.Count; i++)
        //    {
        //        if (RecycleBankPositionIndex[i].x == obj.X && RecycleBankPositionIndex[i].y == obj.Y)
        //        {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}

        //private int FindIndexOfChargeStation(MessageOfChargeStation obj)
        //{
        //    for (int i = 0; i < listOfChargeStation.Count; i++)
        //    {
        //        if (ChargeStationPositionIndex[i].x == obj.X && ChargeStationPositionIndex[i].y == obj.Y)
        //        {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}

        //private int FindIndexOfSignalTower(MessageOfSignalTower obj)
        //{
        //    for (int i = 0; i < listOfSignalTower.Count; i++)
        //    {
        //        if (signaltowerPositionIndex[i].x == obj.X && signaltowerPositionIndex[i].y == obj.Y)
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
                    //if (UIinitiated)
                    //{
                    //    redPlayer.SlideLengthSet();
                    //    bluePlayer.SlideLengthSet();
                    //    gameStatusBar.SlideLengthSet();
                    //}
                    if (!isClientStocked)
                    {
                        /* For Debug */
                        //if (MapGrid.Children.Count > 0)
                        //{
                        //    MapGrid.Children.Clear();
                        //}
                        //foreach (var data in listOfAll)
                        //{
                        //    gameStatusBar.SetGameTimeValue(data);
                        //}
                        /* For Debug */
                        //if (!hasDrawed && mapFlag)
                        //{ 
                        //    DrawMap();
                        //}
                        //if (!hasDrawed)
                        //{
                        //    PureDrawMap(viewModel);
                        //}
                        //foreach (var data in listOfHome)
                        //{
                        //    if (data.TeamId == (long)PlayerTeam.Red)
                        //    {
                        //        redPlayer.SetPlayerValue(data);
                        //    }
                        //    else
                        //    {
                        //        bluePlayer.SetPlayerValue(data);
                        //    }
                        //    DrawHome(data);
                        //}
                        //foreach (var data in listOfBombedBullet)
                        //{
                        //    DrawBombedBullet(data);
                        //}
                        //foreach (var data in listOfBullet)
                        //{
                        //    DrawBullet(data);
                        //}
                        //foreach (var data in listOfResource)
                        //{
                        //    DrawResource(data);
                        //}
                        ////foreach (var data in listOfSweeper)
                        //{
                        //    if (data.TeamId == (long)PlayerTeam.Red)
                        //    {
                        //        redPlayer.SetSweeperValue(data);
                        //    }
                        //    else
                        //    {
                        //        bluePlayer.SetSweeperValue(data);
                        //    }
                        //    // TODO: Dynamic change the ships' label
                        //    DrawSweeper(data);
                        //}
                    }
                }
            }
            finally
            {

            }
            //counter++;
        }
        private readonly object drawPicLock = new();

        private readonly object locklock = new();

        private bool mapFlag = false;
        private bool hasDrawed = false;
        private bool isClientStocked = false;
        //private void DrawHome(MessageOfHome data)
        //{
        //    //Ellipse iconOfHome = new()
        //    //{
        //    //    WidthRequest = 2 * characterRadiusTimes * unitWidth,
        //    //    HeightRequest = 2 * characterRadiusTimes * unitHeight,
        //    //    HorizontalOptions = LayoutOptions.Start,
        //    //    VerticalOptions = LayoutOptions.Start,
        //    //    Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0),
        //    //    Fill = (data.TeamId == (long)PlayerTeam.Red) ? Colors.Red : Colors.Blue
        //    //};
        //    //MapGrid.Children.Add(iconOfHome);
        //}

        //private void DrawRecycleBank(MessageOfRecycleBank data)
        //{
        //    int hp = data.Hp;
        //    //TODO: calculate the percentage of Hp
        //    int idx = FindIndexOfRecycleBank(data);
        //    RecycleBankArray[idx].FontSize = unitFontSize;
        //    RecycleBankArray[idx].WidthRequest = unitWidth;
        //    RecycleBankArray[idx].HeightRequest = unitHeight;
        //    RecycleBankArray[idx].Text = Convert.ToString(hp);
        //    RecycleBankArray[idx].Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0);
        //    RecycleBankArray[idx].BackgroundColor = Colors.Chocolate;
        //    //MapGrid.Children.Add(RecycleBankArray[idx]);
        //}

        //private void DrawChargeStation(MessageOfChargeStation data)
        //{
        //    int hp = data.Hp;
        //    //TODO: calculate the percentage of Hp
        //    int idx = FindIndexOfChargeStation(data);
        //    ChargeStationArray[idx].FontSize = unitFontSize;
        //    ChargeStationArray[idx].WidthRequest = unitWidth;
        //    ChargeStationArray[idx].HeightRequest = unitHeight;
        //    ChargeStationArray[idx].Text = Convert.ToString(hp);
        //    ChargeStationArray[idx].Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0);
        //    ChargeStationArray[idx].BackgroundColor = Colors.Green;
        //    //MapGrid.Children.Add(ChargeStationArray[idx]);
        //}

        //private void DrawSignalTower(MessageOfSignalTower data)
        //{
        //    int hp = data.Hp;
        //    //TODO: calculate the percentage of Hp
        //    int idx = FindIndexOfSignalTower(data);
        //    signaltowerArray[idx].FontSize = unitFontSize;
        //    signaltowerArray[idx].WidthRequest = unitWidth;
        //    signaltowerArray[idx].HeightRequest = unitHeight;
        //    signaltowerArray[idx].Text = Convert.ToString(hp);
        //    signaltowerArray[idx].Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0);
        //    signaltowerArray[idx].BackgroundColor = Colors.Azure;
        //    //MapGrid.Children.Add(signaltowerArray[idx]);
        //}
        //private void DrawBullet(MessageOfBullet data)
        //{
        //    //Ellipse iconOfBullet = new()
        //    //{
        //    //    WidthRequest = 2 * bulletRadiusTimes * unitWidth,
        //    //    HeightRequest = 2 * bulletRadiusTimes * unitHeight,
        //    //    HorizontalOptions = LayoutOptions.Start,
        //    //    VerticalOptions = LayoutOptions.Start,
        //    //    Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * bulletRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * bulletRadiusTimes, 0, 0),
        //    //    Fill = (data.TeamId == (long)PlayerTeam.Red) ? Colors.Red : Colors.Blue
        //    //};
        //    //switch (data.Type)
        //    //{
        //    //    case BulletType.Plasma:
        //    //        iconOfBullet.Fill = Colors.Yellow;
        //    //        break;
        //    //    case BulletType.Laser:
        //    //        iconOfBullet.Fill = Colors.Orange;
        //    //        break;
        //    //    case BulletType.Missile:
        //    //        iconOfBullet.Fill = Colors.Purple;
        //    //        break;
        //    //    case BulletType.Arc:
        //    //        iconOfBullet.Fill = Colors.Green;
        //    //        break;
        //    //}
        //    //MapGrid.Children.Add(iconOfBullet);
        //}

        //private void DrawBombedBullet(MessageOfBombedBullet data)
        //{
        //    //Ellipse iconOfBombedBullet = new()
        //    //{
        //    //    WidthRequest = 2 * bulletRadiusTimes * unitWidth,
        //    //    HeightRequest = 2 * bulletRadiusTimes * unitHeight,
        //    //    HorizontalOptions = LayoutOptions.Start,
        //    //    VerticalOptions = LayoutOptions.Start,
        //    //    Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * bulletRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * bulletRadiusTimes, 0, 0),
        //    //    Fill = (data.MappingId == (long)PlayerTeam.Red) ? Colors.Red : Colors.Blue
        //    //};
        //    //switch (data.Type)
        //    //{
        //    //    case BulletType.Plasma:
        //    //        iconOfBombedBullet.Fill = Colors.Yellow;
        //    //        break;
        //    //    case BulletType.Laser:
        //    //        iconOfBombedBullet.Fill = Colors.Orange;
        //    //        break;
        //    //    case BulletType.Missile:
        //    //        iconOfBombedBullet.Fill = Colors.Purple;
        //    //        break;
        //    //    case BulletType.Arc:
        //    //        iconOfBombedBullet.Fill = Colors.Green;
        //    //        break;
        //    //}
        //    //MapGrid.Children.Add(iconOfBombedBullet);
        //}

        //private void DrawResource(MessageOfGarbage data)
        //{
        //    int idx = FindIndexOfResource(data);
        //    resourceArray[idx].FontSize = unitFontSize;
        //    resourceArray[idx].WidthRequest = unitWidth;
        //    resourceArray[idx].HeightRequest = unitHeight;
        //    resourceArray[idx].Text = Convert.ToString(data.Progress);
        //    resourceArray[idx].Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0);
        //    //MapGrid.Children.Add(resourceArray[idx]);
        //}

        //private void DrawSweeper(MessgaeOfSweeper data)
        //{
        //    //Ellipse iconOfSweeper = new()
        //    //{
        //    //    WidthRequest = 2 * characterRadiusTimes * unitWidth,
        //    //    HeightRequest = 2 * characterRadiusTimes * unitHeight,
        //    //    HorizontalOptions = LayoutOptions.Start,
        //    //    VerticalOptions = LayoutOptions.Start,
        //    //    Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0),
        //    //    Fill = (data.TeamId == (long)PlayerTeam.Red) ? Colors.Red : Colors.Blue
        //    //};
        //    Label nameOfSweeper = new()
        //    {
        //        FontSize = unitFontSize,
        //        WidthRequest = unitWidth,
        //        HeightRequest = unitHeight,
        //        Text = data.SweeperType.ToString()[0] + data.PlayerId.ToString(),
        //        HorizontalOptions = LayoutOptions.Start,
        //        VerticalOptions = LayoutOptions.Start,
        //        HorizontalTextAlignment = TextAlignment.Center,
        //        VerticalTextAlignment = TextAlignment.Center,
        //        BackgroundColor = Colors.Transparent,
        //        Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0)
        //    };
        //    //MapGrid.Children.Add(iconOfSweeper);
        //    //MapGrid.Children.Add(nameOfSweeper);
        //}


        //private bool isClientStocked = false;

        //private bool isPlaybackMode;
        //private double unit;
        //private double unitFontSize = 10;
        //private double unitHeight = 10.6;
        //private double unitWidth = 10.6;
        //private readonly BoxView[,] mapPatches = new BoxView[50, 50];
        //private readonly double characterRadiusTimes = 400;
        //private readonly double bulletRadiusTimes = 200;

        //private readonly object drawPicLock = new();

        //private readonly object locklock = new();

        //private bool mapFlag = false;
        //private bool hasDrawed = false;

        //public int[,] defaultMap = new int[,] {
        //    { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 },//6墙,1-5出生点
        //    { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },//7草
        //    { 6, 0, 0, 0, 0, 0, 0, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 6 },//8机
        //    { 6, 0, 0, 0, 0, 6, 0, 6, 7, 0, 0, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0, 0, 0, 6 },//9大门
        //    { 6, 0, 0, 0, 0, 6, 6, 6, 6, 7, 0, 0, 0, 0, 0, 15, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 15, 0, 0, 0, 6 },//10紧急出口
        //    { 6, 6, 0, 0, 0, 0, 9, 6, 6, 7, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 6, 6, 7, 7, 6, 6, 6, 6, 6, 6, 11, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },//11窗
        //    { 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 7, 7, 6, 6, 7, 7, 6, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 13, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6 },//12-14门
        //    { 6, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 0, 0, 0, 7, 7, 7, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 6 },//15箱
        //    { 6, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 6 },
        //    { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0, 6 },
        //    { 6, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 7, 7, 6, 0, 6 },
        //    { 6, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 7, 6, 0, 6 },
        //    { 6, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 6, 12, 6, 6, 6, 6, 6, 6, 11, 6, 6, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 6, 0, 6 },
        //    { 6, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 15, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0, 6 },
        //    { 6, 0, 0, 0, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0, 0, 0, 7, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 6 },
        //    { 6, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 7, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 0, 0, 0, 0, 0, 6 },
        //    { 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 0, 0, 0, 6 },
        //    { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 11, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6, 6, 7, 0, 0, 6 },
        //    { 6, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 11, 6, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 7, 0, 0, 6 },
        //    { 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 6, 7, 0, 0, 6 },
        //    { 6, 7, 7, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 6, 6, 0, 0, 0, 0, 0, 0, 7, 7, 0, 0, 6, 6, 6, 6, 6, 6, 0, 0, 0, 6 },
        //    { 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 0, 0, 5, 0, 7, 7, 6, 0, 0, 0, 0, 0, 0, 7, 6, 6, 6, 6, 15, 0, 0, 0, 0, 0, 0, 0, 6 },
        //    { 6, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 6, 7, 7, 0, 0, 0, 0, 7, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 7, 6, 0, 0, 0, 6 },
        //    { 6, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 7, 7, 0, 0, 0, 0, 0, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 7, 7, 6, 6, 0, 10, 0, 6 },
        //    { 6, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 6, 6, 6, 6, 7, 0, 0, 0, 6 },
        //    { 6, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 6, 7, 0, 2, 0, 0, 6 },
        //    { 6, 0, 6, 0, 0, 0, 0, 0, 0, 6, 11, 6, 6, 6, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 11, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },
        //    { 6, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },
        //    { 6, 0, 11, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 6, 12, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 0, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },
        //    { 6, 0, 6, 0, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 15, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 7, 7, 0, 0, 0, 0, 6 },
        //    { 6, 0, 6, 7, 0, 0, 0, 8, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 0, 0, 0, 0, 6 },
        //    { 6, 0, 6, 6, 6, 6, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6,6, 7, 0, 0, 0, 6 },
        //    { 6, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 11, 0, 0, 0, 0, 0, 0, 7, 7, 7, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 7, 0, 0, 0, 6 },
        //    { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 6, 7, 0, 0, 0, 6 },
        //    { 6, 0, 0, 0, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 6 },
        //    { 6, 0, 0, 0, 6, 6, 6, 6, 6, 7, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 6 },
        //    { 6, 6, 0, 0, 7, 7, 6, 7, 7, 0, 0, 0, 0, 0, 0, 0, 14, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0, 0, 6 },
        //    { 6, 6, 15, 0, 0, 0, 7, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 11, 6, 0, 0, 0, 0, 0, 6 },
        //    { 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6,6, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },
        //    { 6, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 15, 0, 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },
        //    { 6, 0, 0, 0, 0, 0, 0, 0, 6, 7, 0, 0,8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 6 },
        //    { 6, 0, 0, 0, 0, 0, 0, 0, 6, 7, 7, 0, 0, 0, 6, 6, 6, 11, 6, 0, 0, 6, 6, 6, 7, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 6 },
        //    { 6, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 6, 0, 6, 7, 7, 6, 7, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 14, 6, 6, 6, 0, 0, 0, 0, 0, 7, 0, 0, 6, 0, 6 },
        //    { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 0, 7, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 7, 6, 0, 6, 6, 0, 6 },
        //    { 6, 0, 0, 0, 0, 0, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 7, 0, 0, 7, 6, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 7, 6, 6, 6, 0, 0, 6 },
        //    { 6, 0, 0, 0, 0, 0, 6, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 0, 0, 0, 7, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 },
        //    { 6, 0, 0, 0, 0, 6, 6, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 11, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 6 },
        //    { 6, 0, 0, 0, 6, 6, 6, 6, 6, 7, 0, 0, 0, 10, 0, 0, 0, 0, 6, 6, 7, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 6, 0, 0, 0, 0, 7, 6, 6, 0, 0, 0, 6 },
        //    { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 7, 7, 6, 6, 0, 0, 0, 6 },
        //    { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 }
        //    };
    }
}