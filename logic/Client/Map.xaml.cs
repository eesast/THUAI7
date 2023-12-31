namespace Client;

public partial class Map : ContentView
{
    public Map(Grid parent, int Row, int Column, int RowSpan = 2)
    {
        InitializeComponent();
        parent.Children.Add(this);
        parent.SetColumn(this, Column);
        parent.SetRow(this, Row);
        parent.SetRowSpan(this, RowSpan);
        DrawMap();
    }

    public void DrawMap()
    {
        for (int i = 0; i < 50; i++)
        {
            InMapGrid.AddRowDefinition(new RowDefinition(13));
            for (int j = 0; j < 50; j++)
            {
                InMapGrid.AddColumnDefinition(new ColumnDefinition(13));
                BoxView boxView = new BoxView
                {
                    Color = Colors.Gray,
                    HeightRequest = 13,
                    WidthRequest = 13
                };
                Border border = new Border
                {
                    Content = boxView,
                    HeightRequest = 13,
                    WidthRequest = 13,
                    Padding = 0,
                    StrokeThickness = 0.005
                };
                InMapGrid.Children.Add(border);
                InMapGrid.SetColumn(border, i);
                InMapGrid.SetRow(border, j);
            }
        }
    }
}