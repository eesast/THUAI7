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
using Client.Util;


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
            DrawBullet(new MessageOfBullet
            {
                X = 10,
                Y = 10,
                Type = BulletType.NullBulletType,
                BombRange = 5
            }, canvas);

            DrawShip(new MessageOfShip
            {
                X = 10,
                Y = 11,
                Hp = 100,
                TeamId = 0
            }, canvas);

            DrawBullet(new MessageOfBullet
            {
                X = 9,
                Y = 11,
                Type = BulletType.NullBulletType,
                BombRange = 5
            }, canvas);

            DrawShip(new MessageOfShip
            {
                X = 10,
                Y = 12,
                Hp = 100,
                TeamId = 1
            }, canvas);

            listOfBullet.Add(new MessageOfBullet
            {
                X = 20,
                Y = 20,
                Type = BulletType.NullBulletType,
                BombRange = 5
            });

            listOfShip.Add(new MessageOfShip
            {
                X = 10,
                Y = 12,
                Hp = 100,
                TeamId = 1
            });

            if (listOfBullet.Count > 0)
            {
                foreach (var data in listOfBullet)
                {
                    DrawBullet(data, canvas);
                }
            }

            if (listOfBullet.Count > 0)
            {
                foreach (var data in listOfShip)
                {
                    DrawShip(data, canvas);
                }
            }
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

        private void DrawBullet(MessageOfBullet data, ICanvas canvas)
        {
            PointF point = UtilFunctions.getMapCenter(data.X, data.Y);
            float x = point.X;
            float y = point.Y;
            switch (data.Type)
            {
                case BulletType.Plasma:
                    canvas.FillColor = Colors.Red;
                    break;
                case BulletType.Laser:
                    canvas.FillColor = Colors.Orange;
                    break;
                case BulletType.Missile:
                    canvas.FillColor = Colors.Yellow;
                    break;
                case BulletType.Arc:
                    canvas.FillColor = Colors.Green;
                    break;
                case BulletType.Shell:
                    canvas.FillColor = Colors.Green;
                    break;
                default:
                    canvas.FillColor = Colors.Black;
                    break;
            }
            canvas.FillCircle(x, y, 2);
        }

        private void DrawShip(MessageOfShip data, ICanvas canvas)
        {
            PointF point = UtilFunctions.getMapCenter(data.X, data.Y);
            float x = point.X;
            float y = point.Y;
            int hp = data.Hp;
            long team_id = data.TeamId;
            switch (team_id)
            {
                case (long)PlayerTeam.Red:
                    canvas.FillColor = Colors.Red;
                    break;

                case (long)PlayerTeam.Blue:
                    canvas.FillColor = Colors.Blue;
                    break;

                default:
                    canvas.FillColor = Colors.Black;
                    break;
            }
            canvas.FillCircle(x, y, (float)4.5);
            canvas.FontSize = 5.5F;
            canvas.FontColor = Colors.White;
            canvas.DrawString(Convert.ToString(hp), x - 5, y - 5, 10, 10, HorizontalAlignment.Left, VerticalAlignment.Top);
        }

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

    }
}
