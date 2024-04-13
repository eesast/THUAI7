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
using Client.View;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using Microsoft.Maui.Converters;

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

            Application.Current.UserAppTheme = AppTheme.Light;  //Light Theme Mode
            InitializeComponent();

            unitWidth = viewModel.unitWidth;
            unitHeight = viewModel.unitHeight;
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
                        //Margin = new Thickness(unitWidth * (49 - j), unitHeight * (49 - i), 0, 0),
                        Margin = new Thickness(unitWidth * (j), unitHeight * (i), 0, 0),
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        Padding = 0,
                        FontSize = 4
                    };
                    int index = i * 50 + j;
                    mapPatches_[i, j].SetBinding(Label.BackgroundColorProperty, new Binding($"MapPatchesList[{index}].PatchColor"));
                    mapPatches_[i, j].SetBinding(Label.TextProperty, new Binding($"MapPatchesList[{index}].Text"));
                    mapPatches_[i, j].SetBinding(Label.TextColorProperty, new Binding($"MapPatchesList[{index}].TextColor"));
                    MapGrid.Children.Add(mapPatches_[i, j]);
                    //MapGrid.SetColumn(mapPatches_[i, j], i);
                    //MapGrid.SetRow(mapPatches_[i, j], j);
                }
            }

            for (int i = 0; i < viewModel.numOfShips; i++)
            {
                CircleLabel shipinfo = new()
                {
                    CLDiameter = unitWidth * 0.8,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start,
                    CLBackgroundColor = Colors.Red,
                    CLFontSize = 5,
                    CLTextColor = Colors.White,
                };
                shipCirc.Add(shipinfo);
                shipCirc[i].SetBinding(CircleLabel.CLTextProperty, new Binding($"ShipCircList[{i}].Text"));
                shipCirc[i].SetBinding(CircleLabel.CLBackgroundColorProperty, new Binding($"ShipCircList[{i}].Color"));
                shipCirc[i].SetBinding(CircleLabel.CLMarginProperty, new Binding($"ShipCircList[{i}].Thick"));
                MapGrid.Children.Add(shipCirc[i]);
            }

            for (int i = 0; i < viewModel.numOfBullets; i++)
            {
                CircleLabel bulletinfo = new()
                {
                    CLDiameter = unitWidth * 0.4,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start,
                    CLBackgroundColor = Colors.Black,
                    CLText = "",
                };
                bulletCirc.Add(bulletinfo);
                bulletCirc[i].SetBinding(CircleLabel.CLBackgroundColorProperty, new Binding($"BulletCircList[{i}].Color"));
                bulletCirc[i].SetBinding(CircleLabel.CLMarginProperty, new Binding($"BulletCircList[{i}].Thick"));
                MapGrid.Children.Add(bulletCirc[i]);
            }

            //timer = Dispatcher.CreateTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(500);
            //timer.Tick += new EventHandler(TestRefresh);
            //timer.Start();

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
        private List<CircleLabel> shipCirc = new List<CircleLabel>();
        private List<CircleLabel> bulletCirc = new List<CircleLabel>();
        private readonly IDispatcherTimer timer;
        private long counter;
        public float unitWidth = 10;
        public float unitHeight = 10;

        private void TestRefresh(object sender, EventArgs e)
        {
            lock (viewModel.drawPicLock)
            {
                for (int i = 0; i < viewModel.ShipCircList.Count; i++)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("ship{0}.xy=({1}, {2})", i, viewModel.ShipCircList[i].X, viewModel.ShipCircList[i].Y));
                    System.Diagnostics.Debug.WriteLine(String.Format("numOfShipCirc{0}", shipCirc.Count));
                    //shipCirc[i].Margin = XY2Margin(viewModel.ShipCircList[i].X, viewModel.ShipCircList[i].Y);
                }
            }
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
        //                case MapPatchType.Shadow:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Gray; break; // Shadow
        //                case MapPatchType.Asteroid:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Brown; break; // Asteroid
        //                case MapPatchType.Resource:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Yellow; break; //Resource
        //                case MapPatchType.Factory:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Orange; break; //Factiry
        //                case MapPatchType.Community:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Chocolate; break; //Community
        //                case MapPatchType.Fort:
        //                    mapPatches_[i, j].BackgroundColor = Colors.Azure; break; //Fort
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
        //    FactoryArray = new Label[countMap[(int)MapPatchType.Factory]];
        //    FactoryPositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Factory]];
        //    CommunityArray = new Label[countMap[(int)MapPatchType.Community]];
        //    CommunityPositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Community]];
        //    FortArray = new Label[countMap[(int)MapPatchType.Fort]];
        //    FortPositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Fort]];


        //    int counterOfResource = 0;
        //    int counterOfFactory = 0;
        //    int counterOfCommunity = 0;
        //    int counterOfFort = 0;


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
        //                case MapPatchType.Shadow:
        //                    mapPatches[i, j].Color = Colors.Gray; break; // Shadow
        //                case MapPatchType.Asteroid:
        //                    mapPatches[i, j].Color = Colors.Brown; break; // Asteroid
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

        //                case MapPatchType.Factory:
        //                    mapPatches[i, j].Color = Colors.Orange; //Factory
        //                    FactoryPositionIndex[counterOfFactory] = (i, j);
        //                    FactoryArray[counterOfFactory] = new Label()
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
        //                    counterOfFactory++;
        //                    break;

        //                case MapPatchType.Community:
        //                    mapPatches[i, j].Color = Colors.Chocolate; //Community
        //                    FactoryPositionIndex[counterOfCommunity] = (i, j);
        //                    FactoryArray[counterOfCommunity] = new Label()
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
        //                    counterOfCommunity++;
        //                    break;

        //                case MapPatchType.Fort:
        //                    mapPatches[i, j].Color = Colors.Azure; //Fort
        //                    FactoryPositionIndex[counterOfFort] = (i, j);
        //                    FactoryArray[counterOfFort] = new Label()
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
        //                    counterOfFort++;
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
        //                listOfShip.Clear();
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
        //                                case MessageOfObj.MessageOfObjOneofCase.ShipMessage:
        //                                    listOfShip.Add(obj.ShipMessage);
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

        //                                case MessageOfObj.MessageOfObjOneofCase.FactoryMessage:
        //                                    listOfFactory.Add(obj.FactoryMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.CommunityMessage:
        //                                    listOfCommunity.Add(obj.CommunityMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.FortMessage:
        //                                    listOfFort.Add(obj.FortMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.ResourceMessage:
        //                                    listOfResource.Add(obj.ResourceMessage);
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
        //                        countMap.Add((int)MapPatchType.Factory, listOfFactory.Count);
        //                        countMap.Add((int)MapPatchType.Community, listOfCommunity.Count);
        //                        countMap.Add((int)MapPatchType.Fort, listOfFort.Count);
        //                        //countMap.Add((int)MapPatchType.Building, listOfBuilding.Count);
        //                        GetMap(mapMassage);
        //                        break;
        //                    case GameState.GameRunning:
        //                        foreach (var obj in content.ObjMessage)
        //                        {
        //                            switch (obj.MessageOfObjCase)
        //                            {
        //                                case MessageOfObj.MessageOfObjOneofCase.ShipMessage:
        //                                    listOfShip.Add(obj.ShipMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.FactoryMessage:
        //                                    listOfFactory.Add(obj.FactoryMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.CommunityMessage:
        //                                    listOfCommunity.Add(obj.CommunityMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.FortMessage:
        //                                    listOfFort.Add(obj.FortMessage);
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

        //                                case MessageOfObj.MessageOfObjOneofCase.ResourceMessage:
        //                                    listOfResource.Add(obj.ResourceMessage);
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
        //                            countMap.Add((int)MapPatchType.Factory, listOfFactory.Count);
        //                            countMap.Add((int)MapPatchType.Community, listOfCommunity.Count);
        //                            countMap.Add((int)MapPatchType.Fort, listOfFort.Count);
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
        //                                case MessageOfObj.MessageOfObjOneofCase.ShipMessage:
        //                                    listOfShip.Add(obj.ShipMessage);
        //                                    break;

        //                                //case MessageOfObj.MessageOfObjOneofCase.BuildingMessage:
        //                                //    listOfBuilding.Add(obj.BuildingMessage);
        //                                //    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.FactoryMessage:
        //                                    listOfFactory.Add(obj.FactoryMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.CommunityMessage:
        //                                    listOfCommunity.Add(obj.CommunityMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.FortMessage:
        //                                    listOfFort.Add(obj.FortMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.BulletMessage:
        //                                    listOfBullet.Add(obj.BulletMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.BombedBulletMessage:
        //                                    listOfBombedBullet.Add(obj.BombedBulletMessage);
        //                                    break;

        //                                case MessageOfObj.MessageOfObjOneofCase.ResourceMessage:
        //                                    listOfResource.Add(obj.ResourceMessage);
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

        //private int FindIndexOfResource(MessageOfResource obj)
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

        //private int FindIndexOfFactory(MessageOfFactory obj)
        //{
        //    for (int i = 0; i < listOfFactory.Count; i++)
        //    {
        //        if (FactoryPositionIndex[i].x == obj.X && FactoryPositionIndex[i].y == obj.Y)
        //        {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}

        //private int FindIndexOfCommunity(MessageOfCommunity obj)
        //{
        //    for (int i = 0; i < listOfCommunity.Count; i++)
        //    {
        //        if (CommunityPositionIndex[i].x == obj.X && CommunityPositionIndex[i].y == obj.Y)
        //        {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}

        //private int FindIndexOfFort(MessageOfFort obj)
        //{
        //    for (int i = 0; i < listOfFort.Count; i++)
        //    {
        //        if (FortPositionIndex[i].x == obj.X && FortPositionIndex[i].y == obj.Y)
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
                        ////foreach (var data in listOfShip)
                        //{
                        //    if (data.TeamId == (long)PlayerTeam.Red)
                        //    {
                        //        redPlayer.SetShipValue(data);
                        //    }
                        //    else
                        //    {
                        //        bluePlayer.SetShipValue(data);
                        //    }
                        //    // TODO: Dynamic change the ships' label
                        //    DrawShip(data);
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

        //private void DrawFactory(MessageOfFactory data)
        //{
        //    int hp = data.Hp;
        //    //TODO: calculate the percentage of Hp
        //    int idx = FindIndexOfFactory(data);
        //    FactoryArray[idx].FontSize = unitFontSize;
        //    FactoryArray[idx].WidthRequest = unitWidth;
        //    FactoryArray[idx].HeightRequest = unitHeight;
        //    FactoryArray[idx].Text = Convert.ToString(hp);
        //    FactoryArray[idx].Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0);
        //    FactoryArray[idx].BackgroundColor = Colors.Chocolate;
        //    //MapGrid.Children.Add(FactoryArray[idx]);
        //}

        //private void DrawCommunity(MessageOfCommunity data)
        //{
        //    int hp = data.Hp;
        //    //TODO: calculate the percentage of Hp
        //    int idx = FindIndexOfCommunity(data);
        //    CommunityArray[idx].FontSize = unitFontSize;
        //    CommunityArray[idx].WidthRequest = unitWidth;
        //    CommunityArray[idx].HeightRequest = unitHeight;
        //    CommunityArray[idx].Text = Convert.ToString(hp);
        //    CommunityArray[idx].Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0);
        //    CommunityArray[idx].BackgroundColor = Colors.Green;
        //    //MapGrid.Children.Add(CommunityArray[idx]);
        //}

        //private void DrawFort(MessageOfFort data)
        //{
        //    int hp = data.Hp;
        //    //TODO: calculate the percentage of Hp
        //    int idx = FindIndexOfFort(data);
        //    FortArray[idx].FontSize = unitFontSize;
        //    FortArray[idx].WidthRequest = unitWidth;
        //    FortArray[idx].HeightRequest = unitHeight;
        //    FortArray[idx].Text = Convert.ToString(hp);
        //    FortArray[idx].Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0);
        //    FortArray[idx].BackgroundColor = Colors.Azure;
        //    //MapGrid.Children.Add(FortArray[idx]);
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

        //private void DrawResource(MessageOfResource data)
        //{
        //    int idx = FindIndexOfResource(data);
        //    resourceArray[idx].FontSize = unitFontSize;
        //    resourceArray[idx].WidthRequest = unitWidth;
        //    resourceArray[idx].HeightRequest = unitHeight;
        //    resourceArray[idx].Text = Convert.ToString(data.Progress);
        //    resourceArray[idx].Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0);
        //    //MapGrid.Children.Add(resourceArray[idx]);
        //}

        //private void DrawShip(MessgaeOfShip data)
        //{
        //    //Ellipse iconOfShip = new()
        //    //{
        //    //    WidthRequest = 2 * characterRadiusTimes * unitWidth,
        //    //    HeightRequest = 2 * characterRadiusTimes * unitHeight,
        //    //    HorizontalOptions = LayoutOptions.Start,
        //    //    VerticalOptions = LayoutOptions.Start,
        //    //    Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0),
        //    //    Fill = (data.TeamId == (long)PlayerTeam.Red) ? Colors.Red : Colors.Blue
        //    //};
        //    Label nameOfShip = new()
        //    {
        //        FontSize = unitFontSize,
        //        WidthRequest = unitWidth,
        //        HeightRequest = unitHeight,
        //        Text = data.ShipType.ToString()[0] + data.PlayerId.ToString(),
        //        HorizontalOptions = LayoutOptions.Start,
        //        VerticalOptions = LayoutOptions.Start,
        //        HorizontalTextAlignment = TextAlignment.Center,
        //        VerticalTextAlignment = TextAlignment.Center,
        //        BackgroundColor = Colors.Transparent,
        //        Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0)
        //    };
        //    //MapGrid.Children.Add(iconOfShip);
        //    //MapGrid.Children.Add(nameOfShip);
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