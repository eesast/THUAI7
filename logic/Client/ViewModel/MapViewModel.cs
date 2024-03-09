using Client.Model;
using Protobuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Dispatching;
using Grpc.Core;


namespace Client.ViewModel
{
    public class GraphicsDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            //canvas.FillColor = Colors.Red;

            //// 绘制小球
            //canvas.FillEllipse(ballX, 10, 20, 20);
        }
    }
    public partial class GeneralViewModel : BindableObject, IDrawable
    {

        private List<MessageOfAll> listOfAll;
        private List<MessageOfShip> listOfShip;
        private List<MessageOfBullet> listOfBullet;
        private List<MessageOfBombedBullet> listOfBombedBullet;
        private List<MessageOfFactory> listOfFactory;
        private List<MessageOfCommunity> listOfCommunity;
        private List<MessageOfFort> listOfFort;
        private List<MessageOfWormhole> listOfWormhole;
        private List<MessageOfResource> listOfResource;
        private List<MessageOfHome> listOfHome;

        /* initiate the Lists of Objects and CountList */
        private void InitiateObjects()
        {
            listOfAll = new List<MessageOfAll>();
            listOfShip = new List<MessageOfShip>(); ;
            listOfBullet = new List<MessageOfBullet>();
            listOfBombedBullet = new List<MessageOfBombedBullet>();
            listOfFactory = new List<MessageOfFactory>();
            listOfCommunity = new List<MessageOfCommunity>();
            listOfFort = new List<MessageOfFort>();
            listOfResource = new List<MessageOfResource>();
            listOfHome = new List<MessageOfHome>();
            listOfWormhole = new List<MessageOfWormhole>();
            countMap = new Dictionary<int, int>();
        }
        private (int x, int y)[] resourcePositionIndex;
        private (int x, int y)[] factoryPositionIndex;
        private (int x, int y)[] communityPositionIndex;
        private (int x, int y)[] fortPositionIndex;
        private (int x, int y)[] wormHolePositionIndex;
        private Dictionary<int, int> countMap;


        private int[,] defaultMap;
        ///* Get the Map to default map */
        private void GetMap(MessageOfMap obj)
        {
            int[,] map = new int[50, 50];
            try
            {
                for (int i = 0; i < 50; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        map[i, j] = Convert.ToInt32(obj.Rows[i].Cols[j]) + 4; // 与proto一致
                    }
                }
            }
            catch
            {
                getMapFlag = false;
            }
            finally
            {
                defaultMap = map;
                getMapFlag = true;
            }
        }

        int ballX = 0;
        int ballY = 0;
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.Red;

            // 绘制小球
            canvas.FillEllipse(ballX, ballY, 20, 20);
        }

        private Dictionary<MapPatchType, Color> PatchColorDict = new Dictionary<MapPatchType, Color>
        {
            {MapPatchType.RedHome, Colors.Red},
            {MapPatchType.BlueHome, Colors.Blue},
            {MapPatchType.Ruin, Colors.Black},
            {MapPatchType.Shadow, Colors.Gray},
            {MapPatchType.Asteroid, Colors.Brown},
            {MapPatchType.Resource, Colors.Yellow},
            {MapPatchType.Factory, Colors.Orange},
            {MapPatchType.Community, Colors.Chocolate},
            {MapPatchType.Fort, Colors.Azure},
            {MapPatchType.WormHole, Colors.Purple},
            {MapPatchType.Null, Colors.White}
        };

        private void PureDrawMap(int[,] Map)
        {
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    switch ((MapPatchType)Map[i, j])
                    {
                        case MapPatchType.RedHome:
                            MapPatchesList[UtilFunctions.getIndex(i, j)].PatchColor = Colors.Red; break;  //Red Home
                        case MapPatchType.BlueHome:
                            MapPatchesList[UtilFunctions.getIndex(i, j)].PatchColor = Colors.Blue; break; //Blue Home
                        case MapPatchType.Ruin:
                            MapPatchesList[UtilFunctions.getIndex(i, j)].PatchColor = Colors.Black; break; // Ruin
                        case MapPatchType.Shadow:
                            MapPatchesList[UtilFunctions.getIndex(i, j)].PatchColor = Colors.Gray; break; // Shadow
                        case MapPatchType.Asteroid:
                            MapPatchesList[UtilFunctions.getIndex(i, j)].PatchColor = Colors.Brown; break; // Asteroid
                        case MapPatchType.Resource:
                            MapPatchesList[UtilFunctions.getIndex(i, j)].PatchColor = Colors.Yellow; break; //Resource
                        case MapPatchType.Factory:
                            MapPatchesList[UtilFunctions.getIndex(i, j)].PatchColor = Colors.Orange; break; //Factory
                        case MapPatchType.Community:
                            MapPatchesList[UtilFunctions.getIndex(i, j)].PatchColor = Colors.Chocolate; break; //Community
                        case MapPatchType.Fort:
                            MapPatchesList[UtilFunctions.getIndex(i, j)].PatchColor = Colors.Azure; break; //Fort
                        default:
                            break;
                    }
                }
            }
        }

        //private void DrawMap()
        //{
        //    //resourceArray = new Label[countMap[(int)MapPatchType.Resource]];
        //    resourcePositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Resource]];
        //    //factoryArray = new Label[countMap[(int)MapPatchType.Factory]];
        //    factoryPositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Factory]];
        //    //communityArray = new Label[countMap[(int)MapPatchType.Community]];
        //    communityPositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Community]];
        //    //fortArray = new Label[countMap[(int)MapPatchType.Fort]];
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
        //            MapPatchType mapPatchType = (MapPatchType)todrawMap[i, j];
        //            switch (mapPatchType)
        //            {
        //                case MapPatchType.RedHome:
        //                    mapPatches[i, j].PatchColor = Colors.Red; break;  //Red Home
        //                case MapPatchType.BlueHome:
        //                    mapPatches[i, j].PatchColor = Colors.Blue; break; //Blue Home
        //                case MapPatchType.Ruin:
        //                    mapPatches[i, j].PatchColor = Colors.Black; break; // Ruin
        //                case MapPatchType.Shadow:
        //                    mapPatches[i, j].PatchColor = Colors.Gray; break; // Shadow
        //                case MapPatchType.Asteroid:
        //                    mapPatches[i, j].PatchColor = Colors.Brown; break; // Asteroid
        //                case MapPatchType.Resource:
        //                    mapPatches[i, j].PatchColor = Colors.Yellow; //Resource
        //                    resourcePositionIndex[counterOfResource] = (i, j);
        //                    mapPatches[i, j].Text = "R";
        //                    //resourceArray[counterOfResource] = new Label()
        //                    //{
        //                    //    FontSize = unitFontSize,
        //                    //    WidthRequest = unitWidth,
        //                    //    HeightRequest = unitHeight,
        //                    //    Text = Convert.ToString(-1),
        //                    //    HorizontalOptions = LayoutOptions.Start,
        //                    //    VerticalOptions = LayoutOptions.Start,
        //                    //    HorizontalTextAlignment = TextAlignment.Center,
        //                    //    VerticalTextAlignment = TextAlignment.Center,
        //                    //    BackgroundColor = Colors.Transparent
        //                    //};
        //                    counterOfResource++;
        //                    break;

        //                case MapPatchType.Factory:
        //                    mapPatches[i, j].PatchColor = Colors.Orange; //Factory
        //                    factoryPositionIndex[counterOfFactory] = (i, j);
        //                    mapPatches[i, j].Text = "F";
        //                    //factoryArray[counterOfFactory] = new Label()
        //                    //{
        //                    //    FontSize = unitFontSize,
        //                    //    WidthRequest = unitWidth,
        //                    //    HeightRequest = unitHeight,
        //                    //    Text = Convert.ToString(100),
        //                    //    HorizontalOptions = LayoutOptions.Start,
        //                    //    VerticalOptions = LayoutOptions.Start,
        //                    //    HorizontalTextAlignment = TextAlignment.Center,
        //                    //    VerticalTextAlignment = TextAlignment.Center,
        //                    //    BackgroundColor = Colors.Transparent
        //                    //};
        //                    counterOfFactory++;
        //                    break;

        //                case MapPatchType.Community:
        //                    mapPatches[i, j].PatchColor = Colors.Chocolate; //Community
        //                    communityPositionIndex[counterOfCommunity] = (i, j);
        //                    mapPatches[i, j].Text = "C";
        //                    //factoryArray[counterOfCommunity] = new Label()
        //                    //{
        //                    //    FontSize = unitFontSize,
        //                    //    WidthRequest = unitWidth,
        //                    //    HeightRequest = unitHeight,
        //                    //    Text = Convert.ToString(100),
        //                    //    HorizontalOptions = LayoutOptions.Start,
        //                    //    VerticalOptions = LayoutOptions.Start,
        //                    //    HorizontalTextAlignment = TextAlignment.Center,
        //                    //    VerticalTextAlignment = TextAlignment.Center,
        //                    //    BackgroundColor = Colors.Transparent
        //                    //};
        //                    counterOfCommunity++;
        //                    break;

        //                case MapPatchType.Fort:
        //                    mapPatches[i, j].PatchColor = Colors.Azure; //Fort
        //                    fortPositionIndex[counterOfFort] = (i, j);
        //                    mapPatches[i, j].Text = "Fo";
        //                    //fortArray[counterOfFort] = new Label()
        //                    //{
        //                    //    FontSize = unitFontSize,
        //                    //    WidthRequest = unitWidth,
        //                    //    HeightRequest = unitHeight,
        //                    //    Text = Convert.ToString(-1),
        //                    //    HorizontalOptions = LayoutOptions.Start,
        //                    //    VerticalOptions = LayoutOptions.Start,
        //                    //    HorizontalTextAlignment = TextAlignment.Center,
        //                    //    VerticalTextAlignment = TextAlignment.Center,
        //                    //    BackgroundColor = Colors.Transparent
        //                    //};
        //                    counterOfFort++;
        //                    break;

        //                default:
        //                    break;
        //            }
        //            //MapGrid.Children.Add(mapPatches[i, j]);
        //        }
        //    }
        //}

        private async void OnReceive()
        {
            try
            {
                CancellationToken cts = new CancellationToken();
                while (responseStream != null && await responseStream.ResponseStream.MoveNext(cts))
                {
                    lock (drawPicLock)
                    {
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
                /* 
                    #TODO
                    Show the error message
                */
            }
        }

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

        //private void Refresh(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        lock (drawPicLock)
        //        {
        //            //if (UIinitiated)
        //            //{
        //            //    redPlayer.SlideLengthSet();
        //            //    bluePlayer.SlideLengthSet();
        //            //    gameStatusBar.SlideLengthSet();
        //            //}
        //            if (!isClientStocked)
        //            {
        //                /* For Debug */
        //                //if (MapGrid.Children.Count > 0)
        //                //{
        //                //    MapGrid.Children.Clear();
        //                //}
        //                //foreach (var data in listOfAll)
        //                //{
        //                //    gameStatusBar.SetGameTimeValue(data);
        //                //}
        //                /* For Debug */
        //                //if (!hasDrawed && mapFlag)
        //                //{ 
        //                //    DrawMap();
        //                //}
        //                if (!hasDrawed)
        //                {
        //                    PureDrawMap();
        //                }
        //                //foreach (var data in listOfHome)
        //                //{
        //                //    if (data.TeamId == (long)PlayerTeam.Red)
        //                //    {
        //                //        redPlayer.SetPlayerValue(data);
        //                //    }
        //                //    else
        //                //    {
        //                //        bluePlayer.SetPlayerValue(data);
        //                //    }
        //                //    DrawHome(data);
        //                //}
        //                foreach (var data in listOfBombedBullet)
        //                {
        //                    DrawBombedBullet(data);
        //                }
        //                foreach (var data in listOfBullet)
        //                {
        //                    DrawBullet(data);
        //                }
        //                foreach (var data in listOfResource)
        //                {
        //                    DrawResource(data);
        //                }
        //                //foreach (var data in listOfShip)
        //                //{
        //                //    if (data.TeamId == (long)PlayerTeam.Red)
        //                //    {
        //                //        redPlayer.SetShipValue(data);
        //                //    }
        //                //    else
        //                //    {
        //                //        bluePlayer.SetShipValue(data);
        //                //    }
        //                //    // TODO: Dynamic change the ships' label
        //                //    DrawShip(data);
        //                //}
        //            }
        //        }
        //    }
        //    finally
        //    {

        //    }
        //    //counter++;
        //}

        private void DrawHome(MessageOfHome data)
        {
            int x = data.X;
            int y = data.Y;
            int hp = data.Hp;
            long team_id = data.TeamId;
            int index = UtilFunctions.getIndex(x, y);
            MapPatchesList[index].Text = Convert.ToString(hp);
            switch (team_id)
            {
                case (long)PlayerTeam.Red:
                    MapPatchesList[index].PatchColor = PatchColorDict[MapPatchType.RedHome];
                    MapPatchesList[index].TextColor = Colors.White;
                    break;

                case (long)PlayerTeam.Blue:
                    MapPatchesList[index].PatchColor = PatchColorDict[MapPatchType.BlueHome];
                    MapPatchesList[index].TextColor = Colors.White;
                    break;

                default:
                    MapPatchesList[index].PatchColor = Colors.Black;
                    MapPatchesList[index].TextColor = Colors.White;
                    break;
            }
        }

        private void DrawFactory(MessageOfFactory data)
        {
            int x = data.X;
            int y = data.Y;
            int hp = data.Hp;
            long team_id = data.TeamId;
            int index = UtilFunctions.getIndex(x, y);
            MapPatchesList[index].Text = Convert.ToString(hp);
            switch (team_id)
            {
                case (long)PlayerTeam.Red:
                    MapPatchesList[index].PatchColor = PatchColorDict[MapPatchType.Factory];
                    MapPatchesList[index].TextColor = Colors.Red;
                    break;

                case (long)PlayerTeam.Blue:
                    MapPatchesList[index].PatchColor = PatchColorDict[MapPatchType.Factory];
                    MapPatchesList[index].TextColor = Colors.Blue;
                    break;

                default:
                    MapPatchesList[index].PatchColor = Colors.Black;
                    MapPatchesList[index].TextColor = Colors.White;
                    break;
            }
        }

        private void DrawCommunity(MessageOfCommunity data)
        {
            int x = data.X;
            int y = data.Y;
            int hp = data.Hp;
            long team_id = data.TeamId;
            int index = UtilFunctions.getIndex(x, y);
            MapPatchesList[index].Text = Convert.ToString(hp);
            switch (team_id)
            {
                case (long)PlayerTeam.Red:
                    MapPatchesList[index].PatchColor = PatchColorDict[MapPatchType.Community];
                    MapPatchesList[index].TextColor = Colors.Red;
                    break;

                case (long)PlayerTeam.Blue:
                    MapPatchesList[index].PatchColor = PatchColorDict[MapPatchType.Community];
                    MapPatchesList[index].TextColor = Colors.Blue;
                    break;

                default:
                    MapPatchesList[index].PatchColor = Colors.Black;
                    MapPatchesList[index].TextColor = Colors.White;
                    break;
            }
        }

        private void DrawFort(MessageOfFort data)
        {
            int x = data.X;
            int y = data.Y;
            int hp = data.Hp;
            long team_id = data.TeamId;
            int index = UtilFunctions.getIndex(x, y);
            MapPatchesList[index].Text = Convert.ToString(hp);
            switch (team_id)
            {
                case (long)PlayerTeam.Red:
                    MapPatchesList[index].PatchColor = PatchColorDict[MapPatchType.Fort];
                    MapPatchesList[index].TextColor = Colors.Red;
                    break;

                case (long)PlayerTeam.Blue:
                    MapPatchesList[index].PatchColor = PatchColorDict[MapPatchType.Fort];
                    MapPatchesList[index].TextColor = Colors.Blue;
                    break;

                default:
                    MapPatchesList[index].PatchColor = Colors.Black;
                    MapPatchesList[index].TextColor = Colors.White;
                    break;
            }
        }

        private void DrawWormHole(MessageOfWormhole data)
        {
            int x = data.X;
            int y = data.Y;
            int hp = data.Hp;
            int index = UtilFunctions.getIndex(x, y);
            MapPatchesList[index].Text = Convert.ToString(hp);
            MapPatchesList[index].PatchColor = PatchColorDict[MapPatchType.WormHole];
            MapPatchesList[index].TextColor = Colors.White;
        }

        private void DrawResource(MessageOfResource data)
        {
            int x = data.X;
            int y = data.Y;
            int hp = data.Progress;
            int index = UtilFunctions.getIndex(x, y);
            MapPatchesList[index].Text = Convert.ToString(hp);
            MapPatchesList[index].PatchColor = PatchColorDict[MapPatchType.Resource];
            MapPatchesList[index].TextColor = Colors.White;
        }

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
        //    //resourceArray[idx].FontSize = unitFontSize;
        //    //resourceArray[idx].WidthRequest = unitWidth;
        //    //resourceArray[idx].HeightRequest = unitHeight;
        //    //resourceArray[idx].Text = Convert.ToString(data.Progress);
        //    //resourceArray[idx].Margin = new Thickness(unitHeight * data.Y / 1000.0 - unitWidth * characterRadiusTimes, unitWidth * data.X / 1000.0 - unitWidth * characterRadiusTimes, 0, 0);
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


        private bool isClientStocked = false;
        private bool hasDrawn = false;
        private bool getMapFlag = false;
        private object drawPicLock;
        //private bool isPlaybackMode;
        //private double unit;
        //private double unitFontSize = 10;
        //private double unitHeight = 10.6;
        //private double unitWidth = 10.6;



        public ObservableCollection<MapPatch> mapPatchesList;
        public ObservableCollection<MapPatch> MapPatchesList
        {
            get
            {
                return mapPatchesList ??= new ObservableCollection<MapPatch>();
            }
            set
            {
                mapPatchesList = value;
                OnPropertyChanged();
            }
        }



        //private MapPatch[] mapPatches_ = new MapPatch[2500];
        //public MapPatch[] MapPatches_
        //{
        //    get
        //    {
        //        return mapPatches_ ??= new MapPatch[2500];
        //    }
        //    set
        //    {
        //        mapPatches_ = value;
        //        OnPropertyChanged();
        //    }
        //}

        private MapPatch[,] mapPatches_ = new MapPatch[50, 50];
        public MapPatch[,] MapPatches_
        {
            get
            {
                return mapPatches_;
            }
            set
            {
                mapPatches_ = value;
                OnPropertyChanged();
            }
        }

        //private readonly BoxView[,] mapPatches = new BoxView[50, 50];
        private readonly double characterRadiusTimes = 400;
        private readonly double bulletRadiusTimes = 200;

    }
}
