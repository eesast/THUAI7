namespace installer
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnSliderChanged(object sender, ValueChangedEventArgs e)
        {
            //Txt_Slider1.Text = e.NewValue.ToString();
        }
    }
}