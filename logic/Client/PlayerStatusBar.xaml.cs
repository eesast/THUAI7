using System;
using Protobuf;

namespace Client
{
    public partial class PlayerStatusBar : ContentView
    {
        enum PlayerRole
        {
            Red,    //the down player
            Blue    //the up player
        };
        PlayerRole myRole;
        private readonly int lengthOfHpSlide = 240;

        List<ShipLabel> shipLabels = new List<ShipLabel>();
        public PlayerStatusBar(Grid parent, int Row, int Column, int role)
        {
            InitializeComponent();
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
                Grid shipStatusGrid = new Grid();
                shipStatusGrid.RowDefinitions.Add(new RowDefinition());
                shipStatusGrid.RowDefinitions.Add(new RowDefinition(10));
                shipStatusGrid.Add(shipLabels[shipCounter].status);
                shipStatusGrid.Add(shipLabels[shipCounter].hpSlide);
                shipStatusGrid.SetRow(shipLabels[shipCounter].status, 0);
                shipStatusGrid.SetRow(shipLabels[shipCounter].hpSlide, 1);

                ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].name);
                ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].producer);
                ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].constructor);
                ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].armor);
                ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].shield);
                ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].weapon);
                ShipAllAttributesGrid.Children.Add(shipStatusGrid);
                //ShipAllAttributesGrid.Children.Add(shipLabels[shipCounter].status);

                ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].name, shipCounter);
                ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].producer, shipCounter);
                ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].constructor, shipCounter);
                ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].armor, shipCounter);
                ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].shield, shipCounter);
                ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].weapon, shipCounter);
                ShipAllAttributesGrid.SetRow(shipStatusGrid, shipCounter);
                //ShipAllAttributesGrid.SetRow(shipLabels[shipCounter].status, shipCounter);

                ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].name, 0);
                ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].producer, 1);
                ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].constructor, 2);
                ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].armor, 3);
                ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].shield, 4);
                ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].weapon, 5);
                ShipAllAttributesGrid.SetColumn(shipStatusGrid, 6);
                //ShipAllAttributesGrid.SetColumn(shipLabels[shipCounter].status, 6);
            }
        }

        public void SetPlayerValue(MessageOfHome player)
        {
            if (player.Team == PlayerTeam.Down && myRole == PlayerRole.Red || player.Team == PlayerTeam.Up && myRole == PlayerRole.Blue)
            {
                MyHpData.Text = player.Hp.ToString();
                MyHpSlide.WidthRequest = player.Hp / 100.0 * lengthOfHpSlide;
                MyMoney.Text = player.Economy.ToString();
            }
        }

        public void SetShipValue(MessageOfShip ship)
        {
            if (ship.Team == PlayerTeam.Down && myRole == PlayerRole.Red || ship.Team == PlayerTeam.Up && myRole == PlayerRole.Blue)
            {
                ShipLabel shipLabel = new ShipLabel();
                shipLabel.name.Text = ship.ShipType.ToString() + ship.ShipId.ToString();
                shipLabel.producer.Text = ship.CollectorType.ToString();
                shipLabel.armor.Text = ship.ArmorType.ToString();
                shipLabel.shield.Text = ship.ShieldType.ToString();
                shipLabel.weapon.Text = ship.BulletType.ToString();
                shipLabel.constructor.Text = ship.BuilderType.ToString();
                shipLabel.status.Text = ship.ShipState.ToString();
            }
            //TODO: Dynamic change the ships
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
        public BoxView hpSlide = new BoxView() { Color = Colors.Red, WidthRequest = 80, HeightRequest = 3, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.End };
    };
}