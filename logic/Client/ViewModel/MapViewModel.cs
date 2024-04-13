using Client.Model;
using Client.Util;
using Protobuf;
using System.Collections.ObjectModel;


namespace Client.ViewModel
{
    public partial class GeneralViewModel : BindableObject
    {

        private List<MessageOfAll> listOfAll;
        private List<MessageOfShip> listOfShip;
        private List<MessageOfBullet> listOfBullet;
        private List<MessageOfBombedBullet> listOfBombedBullet;
        private List<MessageOfFactory> listOfFactory;
        private List<MessageOfFort> listOfFort;
        private List<MessageOfCommunity> listOfCommunity;
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
        private (int x, int y)[] FactoryPositionIndex;
        private (int x, int y)[] CommunityPositionIndex;
        private (int x, int y)[] FortPositionIndex;
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
                        switch (obj.Rows[i].Cols[j])
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
                getMapFlag = false;
            }
            finally
            {
                defaultMap = map;
                getMapFlag = true;
            }
        }

        //public class XY
        //{
        //    volatile int x = 10;
        //    volatile int y = 10;
        //    public int X 
        //    {
        //        get => Interlocked.CompareExchange(ref x,-1,-1); 
        //        set => Interlocked.Exchange(ref x, value); 
        //    }
        //    public int Y 
        //    {
        //        get => Interlocked.CompareExchange(ref y,-1,-1); 
        //        set => Interlocked.Exchange(ref y, value);
        //    } 
        //    public void AddX(int value) => Interlocked.Add(ref x, value);
        //    public void AddY(int value) => Interlocked.Add(ref y, value);
        //}

        //private XY ballxy = new XY();

        //public void Draw(ICanvas canvas, RectF dirtyRect)
        //{
        //    lock (drawPicLock)
        //    {
        //        System.Diagnostics.Debug.WriteLine(String.Format("Draw--cou:{0}, coud{1}", cou, Countdow));

        //        System.Diagnostics.Debug.WriteLine("Draw");
        //        canvas.FillColor = Colors.Red;

        //        // 绘制小球
        //        ballx = ballx_receive;
        //        bally = bally_receive;
        //        canvas.FillEllipse(ballx, bally, 20, 20);
        //        System.Diagnostics.Debug.WriteLine(String.Format("============= Draw: ballX:{0}, ballY:{1} ================", ballx, bally));
        //        System.Diagnostics.Debug.WriteLine(String.Format("============= Draw Receive: ballX:{0}, ballY:{1} ================", ballx_receive, bally_receive));

        //        DrawBullet(new MessageOfBullet
        //        {
        //            X = 10,
        //            Y = 10,
        //            Type = BulletType.NullBulletType,
        //            BombRange = 5
        //        }, canvas);

        //        DrawShip(new MessageOfShip
        //        {
        //            X = 10,
        //            Y = 11,
        //            Hp = 100,
        //            TeamId = 0
        //        }, canvas);

        //        DrawBullet(new MessageOfBullet
        //        {
        //            X = 9,
        //            Y = 11,
        //            Type = BulletType.NullBulletType,
        //            BombRange = 5
        //        }, canvas);

        //        DrawShip(new MessageOfShip
        //        {
        //            X = 10,
        //            Y = 12,
        //            Hp = 100,
        //            TeamId = 1
        //        }, canvas);

        //        listOfBullet.Add(new MessageOfBullet
        //        {
        //            X = 20,
        //            Y = 20,
        //            Type = BulletType.NullBulletType,
        //            BombRange = 5
        //        });

        //        listOfShip.Add(new MessageOfShip
        //        {
        //            X = 10,
        //            Y = 12,
        //            Hp = 100,
        //            TeamId = 1
        //        });

        //        if (listOfBullet.Count > 0)
        //        {
        //            foreach (var data in listOfBullet)
        //            {
        //                DrawBullet(data, canvas);
        //            }
        //        }

        //        if (listOfBullet.Count > 0)
        //        {
        //            foreach (var data in listOfShip)
        //            {
        //                DrawShip(data, canvas);
        //            }
        //        }
        //    }
        //}

        private Dictionary<MapPatchType, Color> PatchColorDict = new Dictionary<MapPatchType, Color>
        {
            {MapPatchType.RedHome, Color.FromRgb(237, 49, 47)},
            {MapPatchType.BlueHome, Colors.Blue},
            {MapPatchType.Ruin, Color.FromRgb(181, 122, 88)},
            {MapPatchType.Shadow, Color.FromRgb(73, 177, 82)},
            {MapPatchType.Asteroid, Color.FromRgb(164, 217, 235)},
            {MapPatchType.Resource, Color.FromRgb(160, 75, 166)},
            {MapPatchType.Factory, Color.FromRgb(231, 144, 74)},
            {MapPatchType.Community, Color.FromRgb(231, 144, 74)},
            {MapPatchType.Fort, Color.FromRgb(231, 144, 74)},
            {MapPatchType.WormHole, Color.FromRgb(137, 17, 26)},
            {MapPatchType.Space, Color.FromRgb(255, 255, 255)},
            {MapPatchType.Null, Color.FromRgb(0,0,0)}
        };

        private void PureDrawMap(int[,] Map)
        {
            lock (drawPicLock)
            {
                for (int i = 0; i < 50; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        switch ((MapPatchType)Map[i, j])
                        {
                            case MapPatchType.RedHome:
                                MapPatchesList[UtilFunctions.getCellIndex(i, j)].PatchColor = Color.FromRgb(237, 49, 47); break;  //Red Home
                            case MapPatchType.BlueHome:
                                MapPatchesList[UtilFunctions.getCellIndex(i, j)].PatchColor = Colors.Blue; break; //Blue Home
                            case MapPatchType.Ruin:
                                MapPatchesList[UtilFunctions.getCellIndex(i, j)].PatchColor = Color.FromRgb(181, 122, 88); break; // Ruin
                            case MapPatchType.Shadow:
                                MapPatchesList[UtilFunctions.getCellIndex(i, j)].PatchColor = Color.FromRgb(73, 177, 82); break; // Grass
                            case MapPatchType.Asteroid:
                                MapPatchesList[UtilFunctions.getCellIndex(i, j)].PatchColor = Color.FromRgb(164, 217, 235); break; // River
                            case MapPatchType.Resource:
                                MapPatchesList[UtilFunctions.getCellIndex(i, j)].PatchColor = Color.FromRgb(160, 75, 166); break; //Resource
                            case MapPatchType.Factory:
                                MapPatchesList[UtilFunctions.getCellIndex(i, j)].PatchColor = Color.FromRgb(231, 144, 74); break; //RecycleBank
                            case MapPatchType.Community:
                                MapPatchesList[UtilFunctions.getCellIndex(i, j)].PatchColor = Color.FromRgb(231, 144, 74); break; //ChargeStation
                            case MapPatchType.Fort:
                                MapPatchesList[UtilFunctions.getCellIndex(i, j)].PatchColor = Color.FromRgb(231, 144, 74); break; //SignalTower
                            case MapPatchType.Space:
                                MapPatchesList[UtilFunctions.getCellIndex(i, j)].PatchColor = Color.FromRgb(255, 255, 255); break; //SignalTower
                            case MapPatchType.WormHole:
                                MapPatchesList[UtilFunctions.getCellIndex(i, j)].PatchColor = Color.FromRgb(137, 17, 26); break; //SignalTower
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void DrawShip()
        {
            for (int i = 0; i < ShipCircList.Count; i++)
            {
                ShipCircList[i].Color = Colors.Transparent;
                ShipCircList[i].Text = "";
            }
            System.Diagnostics.Debug.WriteLine(String.Format("listOfShip.Count:{0}", listOfShip.Count));
            System.Diagnostics.Debug.WriteLine(String.Format("ShipCircList.Count:{0}", ShipCircList.Count));
            for (int i = 0; i < listOfShip.Count; i++)
            {
                MessageOfShip data = listOfShip[i];
                DrawCircLabel shipinfo = ShipCircList[i];
                PointF point = UtilFunctions.Grid2CellPoint(data.X, data.Y);
                shipinfo.X = point.X;
                shipinfo.Y = point.Y;
                System.Diagnostics.Debug.WriteLine(String.Format("shipinfo.X:{0}", shipinfo.X));
                System.Diagnostics.Debug.WriteLine(String.Format("shipinfo.Y:{0}", shipinfo.Y));
                long team_id = data.TeamId;
                switch (team_id)
                {
                    case (long)PlayerTeam.Red:
                        System.Diagnostics.Debug.WriteLine("shipinfo.color = red");
                        shipinfo.Color = Colors.DarkRed;
                        break;

                    case (long)PlayerTeam.Blue:
                        System.Diagnostics.Debug.WriteLine("shipinfo.color = blue");

                        shipinfo.Color = Colors.DarkBlue;
                        break;

                    default:
                        System.Diagnostics.Debug.WriteLine("shipinfo.color = black");

                        shipinfo.Color = Colors.DarkGreen;
                        break;
                }
                //shipinfo.Radius = 4.5F;
                //shipinfo.FontSize = 5.5F;
                //shipinfo.TextColor = Colors.White;
                //ShipCircList.Add(shipinfo);
            }
            //shipCircList.Add(
            //    new DrawCircLabel
            //    {
            //        Radius = 4.5F,
            //        Color = Colors.Purple,
            //        Text = "100",
            //        FontSize = 5.5F,
            //        TextColor = Colors.White
            //    }
            //);
        }

        private void DrawBullet()
        {
            for (int i = 0; i < BulletCircList.Count; i++)
            {
                BulletCircList[i].Color = Colors.Transparent;
                BulletCircList[i].Text = "";
            }
            System.Diagnostics.Debug.WriteLine(String.Format("listOfBullet.Count:{0}", listOfBullet.Count));
            System.Diagnostics.Debug.WriteLine(String.Format("BulletCircList.Count:{0}", BulletCircList.Count));
            for (int i = 0; i < listOfBullet.Count; i++)
            {
                MessageOfBullet data = listOfBullet[i];
                DrawCircLabel bulletinfo = BulletCircList[i];
                PointF point = UtilFunctions.Grid2CellPoint(data.X, data.Y);
                bulletinfo.X = point.X;
                bulletinfo.Y = point.Y;
                long team_id = data.TeamId;
                switch (team_id)
                {
                    case (long)PlayerTeam.Red:
                        System.Diagnostics.Debug.WriteLine("bulletinfo.color = red");
                        bulletinfo.Color = Colors.DarkRed;
                        break;

                    case (long)PlayerTeam.Blue:
                        System.Diagnostics.Debug.WriteLine("bulletinfo.color = blue");
                        bulletinfo.Color = Colors.DarkBlue;
                        break;

                    default:
                        System.Diagnostics.Debug.WriteLine("bulletinfo.color = black");
                        bulletinfo.Color = Colors.DarkGreen;
                        break;
                }
                //shipinfo.Radius = 4.5F;
                //shipinfo.FontSize = 5.5F;
                //shipinfo.TextColor = Colors.White;
                //ShipCircList.Add(shipinfo);
            }
        }


        //private void DrawMap()
        //{
        //    //resourceArray = new Label[countMap[(int)MapPatchType.Resource]];
        //    resourcePositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Resource]];
        //    //FactoryArray = new Label[countMap[(int)MapPatchType.Factory]];
        //    FactoryPositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Factory]];
        //    //CommunityArray = new Label[countMap[(int)MapPatchType.Community]];
        //    CommunityPositionIndex = new (int x, int y)[countMap[(int)MapPatchType.Community]];
        //    //FortArray = new Label[countMap[(int)MapPatchType.Fort]];
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
        //                    FactoryPositionIndex[counterOfFactory] = (i, j);
        //                    mapPatches[i, j].Text = "F";
        //                    //FactoryArray[counterOfFactory] = new Label()
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
        //                    CommunityPositionIndex[counterOfCommunity] = (i, j);
        //                    mapPatches[i, j].Text = "C";
        //                    //FactoryArray[counterOfCommunity] = new Label()
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
        //                    FortPositionIndex[counterOfFort] = (i, j);
        //                    mapPatches[i, j].Text = "Fo";
        //                    //FortArray[counterOfFort] = new Label()
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

        //private int FindIndexOfFort(MessgaeOfFort obj)
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



        private void DrawHome(MessageOfHome data)
        {
            int x = data.X / 1000;
            int y = data.Y / 1000;
            int hp = data.Hp;
            long team_id = data.TeamId;
            int index = UtilFunctions.getCellIndex(x, y);
            System.Diagnostics.Debug.WriteLine(String.Format("Draw Home index: {0}", index));

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
            int index = UtilFunctions.getGridIndex(x, y);
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
            int index = UtilFunctions.getGridIndex(x, y);
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
            int index = UtilFunctions.getGridIndex(x, y);
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
            int index = UtilFunctions.getGridIndex(x, y);
            MapPatchesList[index].Text = Convert.ToString(hp);
            MapPatchesList[index].PatchColor = PatchColorDict[MapPatchType.WormHole];
            MapPatchesList[index].TextColor = Colors.White;
        }

        private void DrawResource(MessageOfResource data)
        {
            int x = data.X;
            int y = data.Y;
            int hp = data.Progress;
            int index = UtilFunctions.getGridIndex(x, y);
            MapPatchesList[index].Text = Convert.ToString(hp);
            MapPatchesList[index].PatchColor = PatchColorDict[MapPatchType.Resource];
            MapPatchesList[index].TextColor = Colors.White;
        }

        private bool isClientStocked = false;
        private bool hasDrawn = false;
        private bool getMapFlag = false;
        public readonly object drawPicLock = new();
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

        public ObservableCollection<DrawCircLabel> shipCircList;
        public ObservableCollection<DrawCircLabel> ShipCircList
        {
            get
            {
                return shipCircList ??= new ObservableCollection<DrawCircLabel>();
            }
            set
            {
                shipCircList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DrawCircLabel> bulletCircList;
        public ObservableCollection<DrawCircLabel> BulletCircList
        {
            get
            {
                return bulletCircList ??= new ObservableCollection<DrawCircLabel>();
            }
            set
            {
                bulletCircList = value;
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

        private int mapHeight = 500;
        public int MapHeight
        {
            get
            {
                return mapHeight;
            }
            set
            {
                mapHeight = value;
                OnPropertyChanged();
            }
        }

        private int mapWidth = 500;
        public int MapWidth
        {
            get
            {
                return mapWidth;
            }
            set
            {
                mapWidth = value;
                OnPropertyChanged();
            }
        }

        public Command MoveUpCommand { get; }
        public Command MoveDownCommand { get; }
        public Command MoveLeftCommand { get; }
        public Command MoveRightCommand { get; }
        public Command MoveLeftUpCommand { get; }
        public Command MoveLeftDownCommand { get; }
        public Command MoveRightUpCommand { get; }
        public Command MoveRightDownCommand { get; }
        public Command AttackCommand { get; }
        public Command RecoverCommand { get; }
        public Command ProduceCommand { get; }
        public Command ConstructCommand { get; }
        public Command RebuildCommand { get; }
    }
}
