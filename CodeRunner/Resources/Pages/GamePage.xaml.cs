using CodeRunner.Resources.Scripts;

namespace CodeRunner;

public partial class GamePage : ContentPage
{
	private bool _paused;
	private int _level;
	private MapHolder _mapHolder;

	public readonly Dictionary<int, Color> MapColors = new Dictionary<int, Color>
	{
		{0, Colors.Azure },
		{1, Colors.Green },
		{2, Colors.Red },
		{3, Colors.Black },
	};

	public GamePage()
	{
		InitializeComponent();

		_paused = false;
		_level = 0;
		_mapHolder = new MapHolder();
	}

	private void ClickedGeneration(object sender, EventArgs e)
	{
		GenerateMap();
	}

	#region TerrainGeneration

	private void GenerateMap()
	{
		if (_paused) return;

		_mapHolder.GenerateMap(20, 10);

        boardGrid.Children.Clear();
        boardGrid.RowDefinitions.Clear();
        boardGrid.ColumnDefinitions.Clear();

        for (int i = 0; i < _mapHolder.Map.GetLength(1); i++)
        {
            boardGrid.RowDefinitions.Add(new RowDefinition());
            boardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            boardGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        for (int i = 0; i < _mapHolder.Map.GetLength(0); i++) 
		{
			for(int j = 0; j < _mapHolder.Map.GetLength(1); j++)
			{
				var temp = new Border
				{
					BackgroundColor = MapColors[_mapHolder.Map[i,j]]
				};

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