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
    private List<Projectile> _projectiles;
    private bool _baseExists;


    public GamePage()
    {
        _paused = false;
        _level = 0;
        _map = MapHolder.Instance;
        _player = new Player
        {
            SpritePath = "player.gif",
            Speed = 5,
            Location = new Point(0, 0),
            Name = "Yo mama",
            Rotation = 0,
            Score = 0,
            ProjectileStyle = new Projectile
            {
                Location = new PointF(0, 0),
                Radius = 15,
                Rotation = 0,
                Speed = new PointF(0,0),
                SpeedMultiplier = 15,
                SpritePath = "bullet.png"
            }
        };



        _allEnemies = new List<Enemy>();
        _projectiles = new List<Projectile>();

        InitializeComponent();
        BindingContext = this;



        StartLoopTimer();
    }

    private void ClickedGeneration(object sender, EventArgs e)
    {
        GenerateMap();
    }

    #region GameLoop

    private void GameLoop()
    {

        foreach (var enemy in _allEnemies.Select((value, index) => new { value, index }))
        {
            enemy.value.Move(_baseExists);

        }
        
        for (int i = 0; i < _projectiles.Count; i++)
        {
            _projectiles[i].Move();
            var image = (Image)projectilesGrid[i];
            image.TranslationX = _projectiles[i].Location.X;
            image.TranslationY = _projectiles[i].Location.Y;
        }

        _player.Move(_movePosition);
        OnPropertyChanged(nameof(PlayerTranslationX));
        OnPropertyChanged(nameof(PlayerTranslationY));

    }

    //idk its like a bool for the asynchronous loop to stop
    CancellationTokenSource _cts = new();

    //Invoke this to start the loop
    async Task StartLoopTimer()
    {
        _cts = new();
        while (!_cts.Token.IsCancellationRequested)
        {
            GameLoop();
            await Task.Delay(20); // wait 0,02 second
        }
    }

    //Invoke this to stop the loop
    void StopLoopTimer()
    {
        _cts.Cancel();
    }

    

    #endregion

    #region Level Generation

    private void GenerateLevel()
    {
        GenerateMap();
        GenerateEnemies();

        GeneratePickUps();
    }

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
					BackgroundColor = MapHolder.MapColors[_map.Map[i,j]]
				};

                Grid.SetRow(temp, j);
                Grid.SetColumn(temp, i);

                boardGrid.Children.Add(temp);
            }
		}
		
	}

    private void GeneratePickUps()
    {

    }

    #endregion

    #region Player Controls

    private void MovementDirectionChanged(object sender, PointF e) => _movePosition = e;

    private void Attack(object sender, PanUpdatedEventArgs e)
    {
        switch(e.StatusType)
        {
            case GestureStatus.Completed:
            case GestureStatus.Canceled:

                var playerProjectile = (Projectile)_player.ProjectileStyle.Clone();

                playerProjectile.Speed = _attackPosition;
                playerProjectile.Rotation = _player.Rotation;
                playerProjectile.Location = _player.Location;

                Debug.WriteLine(_attackPosition);

                var projectileImage = new Image
                {
                    Margin = 50,
                    Rotation = playerProjectile.Rotation,
                    HorizontalOptions = LayoutOptions.Start,
                    TranslationX = playerProjectile.Location.X + (playerProjectile.Speed.X * playerProjectile.SpeedMultiplier),
                    TranslationY = playerProjectile.Location.Y + (playerProjectile.Speed.Y * playerProjectile.SpeedMultiplier),
                    WidthRequest = playerProjectile.Radius,
                    HeightRequest = playerProjectile.Radius,
                    Source = playerProjectile.SpritePath
                };

                projectilesGrid.Add(projectileImage);
                _projectiles.Add(playerProjectile);

                break;
        }
    }

    private void AttackDirectionChanged(object sender, PointF e)
    {
        if (e.IsEmpty) return;
        _attackPosition = e;
    }

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
            StopLoopTimer();
			return;
		}

		pauseButton.IsVisible = true;
		pauseMenu.IsVisible = false;
        StartLoopTimer();
    }

    #endregion

    #region Bindings
    public string PlayerSprite => _player.SpritePath;
    public float PlayerTranslationX => _player.Location.X;
    public float PlayerTranslationY => _player.Location.Y;


    public new event PropertyChangedEventHandler? PropertyChanged;
    protected override void OnPropertyChanged([CallerMemberName] string ?name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    #endregion
}