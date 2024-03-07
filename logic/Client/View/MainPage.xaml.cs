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
                        FontSize = 7
                    };
                    int index = i * 50 + j;
                    mapPatches_[i, j].SetBinding(Label.BackgroundColorProperty, new Binding($"MapPatchesList[{index}].PatchColor"));
                    mapPatches_[i, j].SetBinding(Label.TextProperty, new Binding($"MapPatchesList[{index}].Text"));
                    MapGrid.Children.Add(mapPatches_[i, j]);
                    MapGrid.SetColumn(mapPatches_[i, j], i);
                    MapGrid.SetRow(mapPatches_[i, j], j);
                }
            }
            //PureDrawMap(viewModel);
            //InitiateObjects();
            UIinitiated = true;
        }
        private Label[,] mapPatches_ = new Label[50, 50];
        private readonly IDispatcherTimer timer;
        private long counter;
        private int unitWidth = 10;
        private int unitHeight = 10;
        private void TestRefresh(object sender, EventArgs e)
        {
            counter++;
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
        //    factoryArray = new Label[countMap[(int)MapPatchType.Factory]];
        //    factoryPositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Factory]];
        //    communityArray = new Label[countMap[(int)MapPatchType.Community]];
        //    communityPositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Community]];
        //    fortArray = new Label[countMap[(int)MapPatchType.Fort]];
        //    fortPositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Fort]];


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
        //                    factoryPositionIndex[counterOfFactory] = (i, j);
        //                    factoryArray[counterOfFactory] = new Label()
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
        //                    factoryPositionIndex[counterOfCommunity] = (i, j);
        //                    factoryArray[counterOfCommunity] = new Label()
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
        //                    factoryPositionIndex[counterOfFort] = (i, j);
        //                    factoryArray[counterOfFort] = new Label()
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
        //        if (factoryPositionIndex[i].x == obj.X && factoryPositionIndex[i].y == obj.Y)
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
        //        if (communityPositionIndex[i].x == obj.X && communityPositionIndex[i].y == obj.Y)
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
        //        if (fortPositionIndex[i].x == obj.X && fortPositionIndex[i].y == obj.Y)
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
        //    factoryArray[idx].FontSize = unitFontSize;
        //    factoryArray[idx].WidthRequest = unitWidth;
        //    factoryArray[idx].HeightRequest = unitHeight;
        //    factoryArray[idx].Text = Convert.ToString(hp);
        //    factoryArray[idx].Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0);
        //    factoryArray[idx].BackgroundColor = Colors.Chocolate;
        //    //MapGrid.Children.Add(factoryArray[idx]);
        //}

        //private void DrawCommunity(MessageOfCommunity data)
        //{
        //    int hp = data.Hp;
        //    //TODO: calculate the percentage of Hp
        //    int idx = FindIndexOfCommunity(data);
        //    communityArray[idx].FontSize = unitFontSize;
        //    communityArray[idx].WidthRequest = unitWidth;
        //    communityArray[idx].HeightRequest = unitHeight;
        //    communityArray[idx].Text = Convert.ToString(hp);
        //    communityArray[idx].Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0);
        //    communityArray[idx].BackgroundColor = Colors.Green;
        //    //MapGrid.Children.Add(communityArray[idx]);
        //}

        //private void DrawFort(MessageOfFort data)
        //{
        //    int hp = data.Hp;
        //    //TODO: calculate the percentage of Hp
        //    int idx = FindIndexOfFort(data);
        //    fortArray[idx].FontSize = unitFontSize;
        //    fortArray[idx].WidthRequest = unitWidth;
        //    fortArray[idx].HeightRequest = unitHeight;
        //    fortArray[idx].Text = Convert.ToString(hp);
        //    fortArray[idx].Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0);
        //    fortArray[idx].BackgroundColor = Colors.Azure;
        //    //MapGrid.Children.Add(fortArray[idx]);
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

        //private void DrawShip(MessageOfShip data)
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