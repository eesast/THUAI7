namespace Client.View;

public partial class CircleLabel : ContentView
{
	public static readonly BindableProperty CLMarginProperty = BindableProperty.Create(nameof(CLMargin), typeof(Thickness), typeof(CircleLabel), Thickness.Zero);
	public static readonly BindableProperty CLDiameterProperty = BindableProperty.Create(nameof(CLDiameter), typeof(double), typeof(CircleLabel), 10.0);
	public static readonly BindableProperty CLBackgroundColorProperty = BindableProperty.Create(nameof(CLBackgroundColor), typeof(Color), typeof(CircleLabel), Colors.Transparent);
	public static readonly BindableProperty CLTextProperty = BindableProperty.Create(nameof(CLText), typeof(string), typeof(CircleLabel), "");
	public static readonly BindableProperty CLTextColorProperty = BindableProperty.Create(nameof(CLTextColor), typeof(Color), typeof(CircleLabel), Colors.Transparent);
	public static readonly BindableProperty CLFontSizeProperty = BindableProperty.Create(nameof(CLFontSize), typeof(double), typeof(CircleLabel), 5.0);

	public Thickness CLMargin
	{
		get => (Thickness)GetValue(CLMarginProperty);
		set => SetValue(CLMarginProperty, value);
	}

	public double CLDiameter
	{
		get => (double)GetValue(CLDiameterProperty);
		set { SetValue(CLDiameterProperty, value); }
	}

	public Color CLBackgroundColor
	{
		get => (Color)GetValue(CLBackgroundColorProperty);
		set => SetValue(CLBackgroundColorProperty, value);
	}

	public string CLText
	{
		get => (string)GetValue(CLTextProperty);
		set => SetValue(CLTextProperty, value);
	}

	public Color CLTextColor
	{
		get => (Color)GetValue(CLTextColorProperty);
		set => SetValue(CLTextColorProperty, value);
	}

	public double CLFontSize
	{
		get => (double)GetValue(CLFontSizeProperty);
		set => SetValue(CLFontSizeProperty, value);
	}

	public CircleLabel()
	{
		InitializeComponent();
	}
}