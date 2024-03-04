using System;
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
        List<ShipLabel> shipLabels = new List<ShipLabel>();
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
            shipLabels.Add(new ShipLabel());
            shipLabels.Add(new ShipLabel());
            shipLabels.Add(new ShipLabel());
            DrawSelfInfo();
            DrawShipTable();
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

        private void DrawShipTable()
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

                ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].name);
                ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].producer);
                ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].constructor);
                ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].armor);
                ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].shield);
                ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].weapon);
                ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].shipStatusGrid);
                //ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].status);

                ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].name, shipCounter);
                ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].producer, shipCounter);
                ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].constructor, shipCounter);
                ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].armor, shipCounter);
                ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].shield, shipCounter);
                ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].weapon, shipCounter);
                ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].shipStatusGrid, shipCounter);
                //ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].status, shipCounter);

                ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].name, 0);
                ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].producer, 1);
                ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].constructor, 2);
                ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].armor, 3);
                ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].shield, 4);
                ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].weapon, 5);
                ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].shipStatusGrid, 6);
                //ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].status, 6);
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

        public void SetShipValue(MessageOfShip ship)
        {
            if (ship.TeamId == (long)PlayerTeam.Red && myRole == PlayerRole.Red || ship.TeamId == (long)PlayerTeam.Blue && myRole == PlayerRole.Blue)
            {
                ShipLabel shipLabel = new ShipLabel();
                shipLabel.name.Text = ship.ShipType.ToString() + ship.PlayerId.ToString();
                shipLabel.producer.Text = ship.ProducerType.ToString();
                shipLabel.armor.Text = ship.ArmorType.ToString();
                shipLabel.shield.Text = ship.ShieldType.ToString();
                shipLabel.weapon.Text = ship.WeaponType.ToString();
                shipLabel.constructor.Text = ship.ConstructorType.ToString();
                shipLabel.status.Text = ship.ShipState.ToString();
            }
            //TODO: Dynamic change the ships
        }

        public void SlideLengthSet()
        {
            UtilFunctions.SlideLengthSet(MyHpSlide, ref haveSetSlideLength, ref lengthOfHpSlide, PlayerRoleInfoGrid.Width);
            foreach (ShipLabel shiplabel in shipLabels)
            {
                UtilFunctions.SlideLengthSet(shiplabel.hpSlide, ref haveSetSlideLength, ref shiplabel.lengthOfShipHpSlide, shiplabel.shipStatusGrid.Width);
            }
            haveSetSlideLength = true;
        }
    }
    public class ShipLabel
    {
        public Label name = new Label() { Text = "name" };
        public Label producer = new Label() { Text = "producer" };
        public Label constructor = new Label() { Text = "constructor" };
        public Label armor = new Label() { Text = "armor" };
        public Label shield = new Label() { Text = "shield" };
        public Label weapon = new Label() { Text = "weapon" };
        public Label status = new Label() { Text = "IDLE" };
        public double lengthOfShipHpSlide = 80;
        public BoxView hpSlide = new BoxView() { Color = Colors.Red, WidthRequest = 80, HeightRequest = 3, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.End };
        public Grid shipStatusGrid = new Grid();
    };
}