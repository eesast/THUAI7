using Protobuf;

namespace Client;

public partial class GameStatusBar : ContentView
{
    private readonly int lengthOfHp1Slide;
    private readonly int lengthOfHp2Slide;
    private readonly int lengthOfHp3Slide;
    private readonly int WormHoleFullHp = 18000;
    public GameStatusBar(Grid parent, int Row, int Column)
    {
        InitializeComponent();
        parent.Children.Add(this);
        parent.SetColumn(this, Column);
        parent.SetRow(this, Row);
        parent.SetRowSpan(this, 2);
    }
    public void SetWormHoleValue(MessageOfBuilding wormholeMsg)
    {
        if (wormholeMsg.BuildingType != BuildingType.Wormhole)
        {
            return;
        }
        switch (wormholeMsg.BuildingId)
        {
            case 0:
                WormHole1HpSlide.WidthRequest = wormholeMsg.Hp / WormHoleFullHp * lengthOfHp1Slide;
                break;
            case 1:
                WormHole2HpSlide.WidthRequest = wormholeMsg.Hp / WormHoleFullHp * lengthOfHp2Slide;
                break;
            case 2:
                WormHole3HpSlide.WidthRequest = wormholeMsg.Hp / WormHoleFullHp * lengthOfHp3Slide;
                break;
        }
    }

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

}