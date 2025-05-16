namespace CodeRunner.Resources.Controls;

public partial class Joystick : ContentView
{
	public BindableProperty BigRadiusProperty;
	public BindableProperty SmallRadiusProperty;
	public BindableProperty BackgroundColorProperty;
	
	public BindableProperty DefaultVisibilityProperty;

	private PointF startingPosition;
	private PointF currentPosition;
	private PointF navigation;

	public Joystick()
	{
		InitializeComponent();
	}

	public int BigRadius
	{
		get;
		set;
	}

	public int SmallRadius
	{
		get; set;
	}

	public Color BackgroundColor
	{
		get; set;
	}

	public Visibility DefaultVisibility
	{
		get; set;
	}
}