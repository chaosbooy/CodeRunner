using CodeRunner.Resources.Scripts;
using Microsoft.Maui.Dispatching;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CodeRunner;

public partial class GamePage : ContentPage, INotifyPropertyChanged
{
	private bool _paused;
	private int _level;
	private MapHolder _map;
	private PointF _attackPosition;
    private PointF _movePosition;
	private Player _player;

    private List<Enemy> _allEnemies;
    private bool _baseExists;


    public GamePage()
    {
        _paused = false;
        _level = 0;
        _map = new MapHolder();
        _player = new Player
        {
            SpritePath = "player.gif",
            Speed = 3,
            Location = new Point(0, 0),
            Name = "Yo mama",
            Rotation = 0,
            Score = 0,
        };

        _allEnemies = new List<Enemy>();

        InitializeComponent();
        BindingContext = this;

        StartLoopTimer();
    }

    private void ClickedGeneration(object sender, EventArgs e)
    {
        GenerateMap();
    }

    #region GameLoop

    CancellationTokenSource _cts = new();

    async Task StartLoopTimer()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            GameLoop();
            await Task.Delay(20); // wait 0,02 second
        }
    }

    // To stop it:
    void StopLoopTimer()
    {
        _cts.Cancel();
    }

    private void GameLoop()
    {
        if (_paused) return;

        foreach (var enemy in _allEnemies)
        {
            enemy.Move(_baseExists);
        }

        _player.Move(_movePosition);
        OnPropertyChanged(nameof(PlayerTranslationX));
        OnPropertyChanged(nameof(PlayerTranslationY));

    }

    #endregion

    #region Level Generation

    private void GenerateEnemies()
	{
        // y =log(x+1) * 4 <- enemy number spawning
		// maybe change that with enemy points to give out and then based on difficulty it will spend the points UP to the number of max enemies spawned

    }

	private void GenerateMap()
	{
		if (_paused) return;

		Random rand = new Random();
		_map.GenerateMap(20, 10, rand.Next());

        boardGrid.Children.Clear();
        boardGrid.RowDefinitions.Clear();
        boardGrid.ColumnDefinitions.Clear();

        for (int i = 0; i < _map.Map.GetLength(1); i++)
        {
            boardGrid.RowDefinitions.Add(new RowDefinition());
            boardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            boardGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        for (int i = 0; i < _map.Map.GetLength(0); i++) 
		{
			for(int j = 0; j < _map.Map.GetLength(1); j++)
			{
				var temp = new Border
				{
					BackgroundColor = _map.MapColors[_map.Map[i,j]]
				};

                Grid.SetRow(temp, j);
                Grid.SetColumn(temp, i);

                boardGrid.Children.Add(temp);
            }
		}
		
	}

    #endregion

    #region Player Controls

    private void MovementDirectionChanged(object sender, PointF e) => _movePosition = e;

    private void Attack(object sender, PanUpdatedEventArgs e)
    {

    }

    private void AttackDirectionChanged(object sender, PointF e) => _attackPosition = e;

    #endregion

    #region Page Navigation

    private async void OpenSettings(object sender, EventArgs e)
	{
		SettingsPage.PreviousPage = nameof(GamePage);
		try
		{
            await Shell.Current.GoToAsync($"//{nameof(SettingsPage)}");
        }
		catch
		{
			throw new Exception($"There is no page named \"{SettingsPage.PreviousPage}\".");
		}
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

    #region Bindings
    public string PlayerSprite => _player.SpritePath;
    public float PlayerTranslationX => _player.Location.X;
    public float PlayerTranslationY => _player.Location.Y;


    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    #endregion
}