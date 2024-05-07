using Client.Util;
using Protobuf;

namespace Client;

public partial class GameStatusBar : ContentView
{
    private bool haveSetSlideLength = false;
    double lengthOfWormHole1HpSlide = 80;
    double lengthOfWormHole2HpSlide = 80;
    double lengthOfWormHole3HpSlide = 80;

    private readonly int WormHoleFullHp = 18000;
    public GameStatusBar(Grid parent, int Row, int Column)
    {
        InitializeComponent();
        parent.Children.Add(this);
        parent.SetColumn(this, Column);
        parent.SetRow(this, Row);
        parent.SetRowSpan(this, 2);
    }
    //public void SetWormHoleValue(MessageOfBuilding wormholeMsg)
    //{
    //    if (wormholeMsg.BuildingType != BuildingType.Wormhole)
    //    {
    //        return;
    //    }
    //    switch (wormholeMsg.BuildingId)
    //    {
    //        case 0:
    //            WormHole1HpSlide.WidthRequest = wormholeMsg.Hp / WormHoleFullHp * lengthOfWormHole1HpSlide;
    //            break;
    //        case 1:
    //            WormHole2HpSlide.WidthRequest = wormholeMsg.Hp / WormHoleFullHp * lengthOfWormHole2HpSlide;
    //            break;
    //        case 2:
    //            WormHole3HpSlide.WidthRequest = wormholeMsg.Hp / WormHoleFullHp * lengthOfWormHole3HpSlide;
    //            break;
    //    }
    //}

    public void SetGameTimeValue(MessageOfAll obj)
    {
        int min, sec;
        sec = obj.GameTime / 1000;
        min = sec / 60;
        sec = sec % 60;
        GameTime.Text = "时间：";
        if (min / 10 == 0)
        {
            GameTime.Text += "0";
        }
        GameTime.Text += min.ToString() + ":";
        if (sec / 10 == 0)
        {
            GameTime.Text += "0";
        }
        GameTime.Text += sec.ToString();
    }

    public void SlideLengthSet()
    {
        UtilFunctions.SlideLengthSet(WormHole1HpSlide, ref haveSetSlideLength, ref lengthOfWormHole1HpSlide, GameStatusGrid.Width);
        UtilFunctions.SlideLengthSet(WormHole2HpSlide, ref haveSetSlideLength, ref lengthOfWormHole2HpSlide, GameStatusGrid.Width);
        UtilFunctions.SlideLengthSet(WormHole3HpSlide, ref haveSetSlideLength, ref lengthOfWormHole3HpSlide, GameStatusGrid.Width);
        haveSetSlideLength = true;
    }
}