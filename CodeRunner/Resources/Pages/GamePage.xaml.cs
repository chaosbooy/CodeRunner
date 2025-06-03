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
    private Random _rng;

    private List<Enemy> _allEnemies;
    private List<Projectile> _allProjectiles;
    private List<Item> _allItems;
    private bool _baseExists;

    private static double tileWidth;
    private static double tileHeight;

    private readonly float playerRadius = 20;


    public GamePage()
    {
        _paused = false;
        _level = 0;
        _map = MapHolder.Instance;
        _rng = new Random(5);
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
                SpritePath = "bullet.gif"
            }
        };


        _allEnemies = new List<Enemy>();
        _allProjectiles = new List<Projectile>();
        _allItems = new List<Item>();

        InitializeComponent();
        BindingContext = this;


        StartLoopTimer();
    }

    private void ClickedGeneration(object sender, EventArgs e)
    {
        GenerateLevel();
    }

    #region GameLoop

    private void GameLoop()
    {
        if (_map.Map.Length == 0) return;

        foreach (var enemy in _allEnemies.Select((value, index) => new { value, index }))
        {
            enemy.value.Move(_baseExists);

        }
        
        for (int i = 0; i < _allProjectiles.Count; i++)
        {
            _allProjectiles[i].Move();
            var image = (Image)projectilesGrid[i];
            image.TranslationX = _allProjectiles[i].Location.X;
            image.TranslationY = _allProjectiles[i].Location.Y;
        }

        var nextX = _player.Location.X + _movePosition.X * _player.Speed;
        var nextY = _player.Location.Y + _movePosition.Y * _player.Speed;
        
        // Separate X and Y movement to allow sliding
        var currentPos = _player.Location;

        var tryX = new PointF(nextX, currentPos.Y);
        if (IsWalkable(tryX.X, tryX.Y, playerRadius))
        {
            _player.Location = tryX;
            OnPropertyChanged(nameof(PlayerTranslationX));
        }

        var tryY = new PointF(_player.Location.X, nextY);
        if (IsWalkable(tryY.X, tryY.Y, playerRadius))
        {
            _player.Location = tryY;
            OnPropertyChanged(nameof(PlayerTranslationY));
        }
        // --- End player movement ---

        for (int i = _allItems.Count - 1; i >= 0; i--)
        {
            var item = _allItems[i];
            var dx = item.Location.X - _player.Location.X;
            var dy = item.Location.Y - _player.Location.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance < item.Radius + 25) // Adjust pickup radius
            {
                _player.Score += item.Score;
                _allItems.RemoveAt(i);
                itemGrid.Children.RemoveAt(i); // Remove the corresponding image
                OnPropertyChanged(nameof(PlayerScore));
            }
        }

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
        if (_paused) return;


        _player.NumberOfShotsShot = 0;
        _allProjectiles.Clear();
        projectilesGrid.Children.Clear();

        _level++;

        GenerateMap();
        GenerateEnemies();
        GenerateItems();


        //Setting player loaction to level start
        _player.Location = new PointF((float)(tileWidth + playerRadius) / 2, (float)((_map.Entrance + 1) * tileHeight - (tileHeight / 2)) + (playerRadius / 2));
        OnPropertyChanged(nameof(PlayerTranslationY));
        OnPropertyChanged(nameof(PlayerTranslationX));

    }

    private void GenerateEnemies()
    {
        enemiesGrid.Children.Clear();
        _allEnemies.Clear();
        // y =log(x+1) * 4 <- enemy number spawning
        // maybe change that with enemy points to give out and then based on difficulty it will spend the points UP to the number of max enemies spawned

    }

	private void GenerateMap()
    {
		_map.GenerateMap(20, 10, _rng.Next());

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

        tileWidth = boardGrid.Width / _map.Map.GetLength(0);
        tileHeight = boardGrid.Height / _map.Map.GetLength(1);

    }

    private void GenerateItems()
    {
        _allItems = new List<Item>();
        itemGrid.Children.Clear();
        _player.Score = 0;

        int itemsToSpawn = Math.Min(_map.EmptyTiles.Count, _level * 3); // Increase with level but capped

        // Shuffle tile positions
        var emptyTiles = _map.EmptyTiles.OrderBy(t => _rng.Next()).ToList();

        for (int i = 0; i < Math.Min(itemsToSpawn, emptyTiles.Count); i++)
        {
            var (tileX, tileY) = emptyTiles[i];
            var item = (_rng.Next(20) == 1) ? Item.RareItem() : Item.NormalItem();

            // Center item in tile
            item.Location = new PointF(
                (float)(tileX * tileWidth + tileWidth / 2 ),
                (float)(tileY * tileHeight + tileHeight / 2)
            );

            _allItems.Add(item);

            var image = new Image
            {
                Source = item.SpritePath,
                WidthRequest = item.Radius * 2,
                HeightRequest = item.Radius * 2,
                TranslationX = item.Location.X - item.Radius,
                TranslationY = item.Location.Y - item.Radius,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                Background = Colors.Purple
            };

            itemGrid.Children.Add(image);
        }
    }

    #endregion

    #region Player Controls

    public bool IsWalkable(float x, float y, float radius)
    {
        if (x - radius < 0 || x + radius >= boardGrid.Width) return false;
        if (y - radius < 0 || y + radius >= boardGrid.Height) return false;

        int leftTile = (int)((x - radius) / tileWidth);
        int rightTile = (int)((x + radius) / tileWidth);
        int topTile = (int)((y - radius) / tileHeight);
        int bottomTile = (int)((y + radius) / tileHeight);

        for (int i = leftTile; i <= rightTile; i++)
        {
            for (int j = topTile; j <= bottomTile; j++)
            {
                if (_map.Map[i, j] == 3) return false; // Wall tile
            }
        }

        return true;
    } 

    private void MovementDirectionChanged(object sender, PointF e) => _movePosition = e;

    private void Attack(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Completed:
            case GestureStatus.Canceled:

                if (_player.NumberOfShotsShot > 2) return;

                var playerProjectile = (Projectile)_player.ProjectileStyle.Clone();

                playerProjectile.Speed = _attackPosition;
                playerProjectile.Rotation = _player.Rotation;
                playerProjectile.Location = _player.Location;

                var projectileImage = new Image
                {
                    Rotation = playerProjectile.Rotation,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start,
                    TranslationX = playerProjectile.Location.X,
                    TranslationY = playerProjectile.Location.Y,
                    WidthRequest = playerProjectile.Radius,
                    HeightRequest = playerProjectile.Radius,
                    Source = playerProjectile.SpritePath,
                    Aspect = Aspect.Fill,
                    AnchorX = 0.5,
                    AnchorY = 0.5,
                };

                projectilesGrid.Add(projectileImage);
                _allProjectiles.Add(playerProjectile);
                _player.NumberOfShotsShot++;

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
    public float PlayerTranslationX => (float)(_player.Location.X - (tileWidth / 2));
    public float PlayerTranslationY => (float)((float)_player.Location.Y - (tileHeight/ 2));
    public int PlayerScore => _player.Score;



    public new event PropertyChangedEventHandler? PropertyChanged;
    protected override void OnPropertyChanged([CallerMemberName] string ?name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    #endregion
}