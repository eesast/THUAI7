using System;
using Client.Util;
using Protobuf;

namespace Client
{
    public partial class PlayerStatusBar : ContentView
    {
        private bool haveSetSlideLength = false;
        enum PlayerRole
        {
            Red,    //the down player
            Blue    //the up player
        };
        PlayerRole myRole;
        private double lengthOfHpSlide = 240;
        List<SweeperLabel> shipLabels = new List<SweeperLabel>();
        public PlayerStatusBar(Grid parent, int Row, int Column, int role)
        {
            InitializeComponent();
            MyHpSlide.WidthRequest = lengthOfHpSlide;
            if (role == 0)
            {
                myRole = PlayerRole.Red;
            }
            else
            {
                myRole = PlayerRole.Blue;
            }
            parent.Children.Add(this);
            parent.SetColumn(this, Column);
            parent.SetRow(this, Row);
            shipLabels.Add(new SweeperLabel());
            shipLabels.Add(new SweeperLabel());
            shipLabels.Add(new SweeperLabel());
            DrawSelfInfo();
            DrawSweeperTable();
        }

        private void DrawSelfInfo()
        {
            MyHpSlide.WidthRequest = lengthOfHpSlide;
            if (myRole == PlayerRole.Red)
            {
                MyName.Text = "Red Player";
                MyColor.Color = Colors.Red;
                MyHpSlide.Color = Colors.Red;
            }
            else
            {
                MyName.Text = "Blue Player";
                MyColor.Color = Colors.Blue;
                MyHpSlide.Color = Colors.Blue;
            }
        }

        private void DrawSweeperTable()
        {
            for (int shipCounter = 0; shipCounter < shipLabels.Count; shipCounter++)
            {
                if (myRole == PlayerRole.Red)
                {
                    shipLabels[shipCounter].hpSlide.Color = Colors.Red;
                }
                else
                {
                    shipLabels[shipCounter].hpSlide.Color = Colors.Blue;
                }
                shipLabels[shipCounter].shipStatusGrid.RowDefinitions.Add(new RowDefinition());
                shipLabels[shipCounter].shipStatusGrid.RowDefinitions.Add(new RowDefinition(10));
                shipLabels[shipCounter].shipStatusGrid.Add(shipLabels[shipCounter].status);
                shipLabels[shipCounter].shipStatusGrid.Add(shipLabels[shipCounter].hpSlide);
                shipLabels[shipCounter].shipStatusGrid.SetRow(shipLabels[shipCounter].status, 0);
                shipLabels[shipCounter].shipStatusGrid.SetRow(shipLabels[shipCounter].hpSlide, 1);

                SweeperAllAttributesGrid.Children.Add(shipLabels[shipCounter].name);
                SweeperAllAttributesGrid.Children.Add(shipLabels[shipCounter].producer);
                SweeperAllAttributesGrid.Children.Add(shipLabels[shipCounter].constructor);
                SweeperAllAttributesGrid.Children.Add(shipLabels[shipCounter].armor);
                SweeperAllAttributesGrid.Children.Add(shipLabels[shipCounter].shield);
                SweeperAllAttributesGrid.Children.Add(shipLabels[shipCounter].weapon);
                SweeperAllAttributesGrid.Children.Add(shipLabels[shipCounter].shipStatusGrid);
                //SweeperAllAttributesGrid.Children.Add(shipLabels[shipCounter].status);

                SweeperAllAttributesGrid.SetRow(shipLabels[shipCounter].name, shipCounter);
                SweeperAllAttributesGrid.SetRow(shipLabels[shipCounter].producer, shipCounter);
                SweeperAllAttributesGrid.SetRow(shipLabels[shipCounter].constructor, shipCounter);
                SweeperAllAttributesGrid.SetRow(shipLabels[shipCounter].armor, shipCounter);
                SweeperAllAttributesGrid.SetRow(shipLabels[shipCounter].shield, shipCounter);
                SweeperAllAttributesGrid.SetRow(shipLabels[shipCounter].weapon, shipCounter);
                SweeperAllAttributesGrid.SetRow(shipLabels[shipCounter].shipStatusGrid, shipCounter);
                //SweeperAllAttributesGrid.SetRow(shipLabels[shipCounter].status, shipCounter);

                SweeperAllAttributesGrid.SetColumn(shipLabels[shipCounter].name, 0);
                SweeperAllAttributesGrid.SetColumn(shipLabels[shipCounter].producer, 1);
                SweeperAllAttributesGrid.SetColumn(shipLabels[shipCounter].constructor, 2);
                SweeperAllAttributesGrid.SetColumn(shipLabels[shipCounter].armor, 3);
                SweeperAllAttributesGrid.SetColumn(shipLabels[shipCounter].shield, 4);
                SweeperAllAttributesGrid.SetColumn(shipLabels[shipCounter].weapon, 5);
                SweeperAllAttributesGrid.SetColumn(shipLabels[shipCounter].shipStatusGrid, 6);
                //SweeperAllAttributesGrid.SetColumn(shipLabels[shipCounter].status, 6);
            }
        }

        public void SetPlayerValue(MessageOfHome player)
        {
            if (player.TeamId == (long)PlayerTeam.Red && myRole == PlayerRole.Red || player.TeamId == (long)PlayerTeam.Blue && myRole == PlayerRole.Blue)
            {
                MyHpData.Text = player.Hp.ToString();
                MyHpSlide.WidthRequest = player.Hp / 100.0 * lengthOfHpSlide;
                //MyMoney.Text = player.Score.ToString();
            }
        }

        public void SetSweeperValue(MessageOfSweeper ship)
        {
            if (ship.TeamId == (long)PlayerTeam.Red && myRole == PlayerRole.Red || ship.TeamId == (long)PlayerTeam.Blue && myRole == PlayerRole.Blue)
            {
                SweeperLabel shipLabel = new SweeperLabel();
                shipLabel.name.Text = ship.SweeperType.ToString() + ship.PlayerId.ToString();
                shipLabel.producer.Text = ship.ProducerType.ToString();
                shipLabel.armor.Text = ship.ArmorType.ToString();
                shipLabel.shield.Text = ship.ShieldType.ToString();
                shipLabel.weapon.Text = ship.WeaponType.ToString();
                shipLabel.constructor.Text = ship.ConstructorType.ToString();
                shipLabel.status.Text = ship.SweeperState.ToString();
            }
            //TODO: Dynamic change the ships
        }

        public void SlideLengthSet()
        {
            UtilFunctions.SlideLengthSet(MyHpSlide, ref haveSetSlideLength, ref lengthOfHpSlide, PlayerRoleInfoGrid.Width);
            foreach (SweeperLabel shiplabel in shipLabels)
            {
                UtilFunctions.SlideLengthSet(shiplabel.hpSlide, ref haveSetSlideLength, ref shiplabel.lengthOfSweeperHpSlide, shiplabel.shipStatusGrid.Width);
            }
            haveSetSlideLength = true;
        }
    }
    public class SweeperLabel
    {
        public Label name = new Label() { Text = "name" };
        public Label producer = new Label() { Text = "producer" };
        public Label constructor = new Label() { Text = "constructor" };
        public Label armor = new Label() { Text = "armor" };
        public Label shield = new Label() { Text = "shield" };
        public Label weapon = new Label() { Text = "weapon" };
        public Label status = new Label() { Text = "IDLE" };
        public double lengthOfSweeperHpSlide = 80;
        public BoxView hpSlide = new BoxView() { Color = Colors.Red, WidthRequest = 80, HeightRequest = 3, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.End };
        public Grid shipStatusGrid = new Grid();
    };
}