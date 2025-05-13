namespace CodeRunner;

public partial class GamePage : ContentPage
{
	private bool _paused = false;
	private int _level = 0;
	private int _boardHeight = 10;
	private int _boardWidth = 20;

	public GamePage()
	{
		InitializeComponent();
	}

	private void ClickedGeneration(object sender, EventArgs e)
	{
		GenerateMap();
	}

	#region TerrainGeneration

	private void GenerateMap()
	{
		boardGrid.Children.Clear();
		boardGrid.RowDefinitions.Clear();
		boardGrid.ColumnDefinitions.Clear();

		//Generating board size
		for (int i = 0; i < _boardHeight; i++)
		{
			boardGrid.RowDefinitions.Add(new RowDefinition());
			boardGrid.ColumnDefinitions.Add(new ColumnDefinition());
			boardGrid.ColumnDefinitions.Add(new ColumnDefinition());
		}

		Random rand = new Random();

		int entry, exit;
		entry = rand.Next(_boardHeight);
		exit = rand.Next(_boardHeight);

		for (int i = 0; i < _boardWidth; i++)
		{
			for(int j = 0; j < _boardHeight; j++)
			{

				var temp = new Border
				{
					BackgroundColor = Colors.AliceBlue,
				};

				if (i == 0 && j == entry) temp.BackgroundColor = Colors.Green;
				else if(i == _boardWidth - 1 && j == exit) temp.BackgroundColor = Colors.Red;

				Grid.SetRow(temp, j);
				Grid.SetColumn(temp, i);

				boardGrid.Children.Add(temp);
			}
		}

	}

    #endregion

    #region Navigation

    private async void OpenSettings(object sender, EventArgs e)
	{
        await Shell.Current.GoToAsync($"//{nameof(SettingsPage)}");
    }

	private async void OpenLobby(object sender, EventArgs e)
	{
        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }

	private async void PauseResume(object sender, EventArgs e)
	{
		_paused = !_paused;
		if(_paused)
		{
			pauseButton.IsVisible = false;
			pauseMenu.IsVisible = true;
			return;
		}

		pauseButton.IsVisible = true;
		pauseMenu.IsVisible = false;
    }

    #endregion
}