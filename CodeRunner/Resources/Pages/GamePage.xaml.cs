using CodeRunner.Resources.Scripts;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CodeRunner;

public partial class GamePage : ContentPage, INotifyPropertyChanged
{
    #region variables

    public static GamePage Instance { get; private set; }
    private static double boardWidth = 0;
    private static double boardHeight = 0;
    
    public Player PlayerRef => _player;

    private bool _paused;
	private int _level = 0;
	private static MapHolder _map;
	private PointF _attackPosition;
    private PointF _movePosition;
	private Player _player;
    private Random _rng;

    private List<Enemy> _allEnemies;
    private List<Projectile> _allProjectiles;
    private List<Item> _allItems;
    private bool _baseExists = false;

    private static double _tileWidth;
    private static double _tileHeight;

    public static double TileWidth { get  { return _tileWidth; } }
    public static double TileHeight { get { return _tileHeight; } }

    private double hitboxRadius = 20;
    private double padding = 10;

    #endregion


    public GamePage()
    {
        Instance = this;
        _paused = false;
        _level = 0;
        _map = MapHolder.Instance;
        _rng = new Random(4);
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

    }

    private void SizeChanged(object sender, EventArgs e)
    {
        boardWidth = boardGrid.Width;
        boardHeight = boardGrid.Height;

        if (_map != null && _map.Map.Length > 0)
        {
            _tileWidth = boardWidth / _map.Map.GetLength(0);
            _tileHeight = boardHeight / _map.Map.GetLength(1);
        }
    }

    private void PageLoaded(object sender, EventArgs e)
    {
        // If the level has already been set up, do nothing.
        if (_level != 0)
        {
            return;
        }


        // Now that the page is loaded, we can safely generate the level
        // because boardGrid.Width and boardGrid.Height will have their real values.
        GenerateLevel();

        // NOW it's safe to start the game loop.
        StartLoopTimer();
        _player.Score = 0;


        boardWidth = boardGrid.Width;
        boardHeight = boardGrid.Height;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopLoopTimer();
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
            var image = (Image)enemiesGrid.Children[enemy.index];
            image.TranslationX = enemy.value.Location.X;
            image.TranslationY = enemy.value.Location.Y;
        }
        
        for (int i = 0; i < _allProjectiles.Count; i++) 
        {
            _allProjectiles[i].Move();
            var image = (Image)projectilesGrid[i];
            image.TranslationX = _allProjectiles[i].Location.X;
            image.TranslationY = _allProjectiles[i].Location.Y;

            var mapTile = ((int)(_allProjectiles[i].Location.X / _tileWidth), (int)(_allProjectiles[i].Location.Y / _tileHeight));
            if (mapTile.Item1 >= _map.Map.GetLength(0) || mapTile.Item2 >= _map.Map.GetLength(1) ||
                mapTile.Item1 < 0 || mapTile.Item2 < 0 ||_map.Map[mapTile.Item1, mapTile.Item2] == 3)
            {
                _allProjectiles.RemoveAt(i);
                projectilesGrid.Children.RemoveAt(i);
            }
        }

        var nextX = _player.Location.X + _movePosition.X * _player.Speed;
        var nextY = _player.Location.Y + _movePosition.Y * _player.Speed;
        
        // Separate X and Y movement to allow sliding
        var currentPos = _player.Location;

        var tryX = new PointF(nextX, currentPos.Y);
        if (!ContainSurroundings(tryX.X, tryX.Y, hitboxRadius - padding, 3))
        {
            _player.Location = tryX;
            OnPropertyChanged(nameof(PlayerTranslationX));
        }

        var tryY = new PointF(_player.Location.X, nextY);
        if (!ContainSurroundings(tryY.X, tryY.Y, hitboxRadius - padding, 3))
        {
            _player.Location = tryY;
            OnPropertyChanged(nameof(PlayerTranslationY));
        }

        if(ContainSurroundings(_player.Location.X, _player.Location.Y, padding, 4))
        {
            GenerateLevel();
            Debug.WriteLine("NEW LEVEL");
        }
        // --- End player movement ---

        for (int i = _allItems.Count - 1; i >= 0; i--)
        {
            var item = _allItems[i];
            var dx = item.Location.X - _player.Location.X;
            var dy = item.Location.Y - _player.Location.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance < item.Radius + hitboxRadius) // Adjust pickup radius
            {
                _player.Score += item.Score;
                _allItems.RemoveAt(i);
                itemGrid.Children.RemoveAt(i); // Remove the corresponding image
                OnPropertyChanged(nameof(PlayerScore));
                
                if (_allItems.Count == 0)
                {
                    var temp = (Border)boardGrid.Children[_map.Map.GetLength(1) * (_map.Map.GetLength(0) - 1) + _map.Exit];
                    temp.BackgroundColor = Colors.Blue;
                    _map.ActivateExit();
                }
                break;
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
        GenerateItems();

        hitboxRadius = Math.Min(_tileWidth, _tileHeight) / 2; // half-size radius
        padding = hitboxRadius / 3;
        Player.WidthRequest = hitboxRadius * 2;
        Player.HeightRequest = hitboxRadius * 2;


        GenerateEnemies();

        //Setting player loaction to level start
        var startX = _tileWidth / 2;
        var startY = (_map.Entrance * _tileHeight) + _tileHeight / 2;

        _player.Location = new PointF((float)startX, (float)startY);

        OnPropertyChanged(nameof(PlayerTranslationY));
        OnPropertyChanged(nameof(PlayerTranslationX));

    }

    private void GenerateEnemies()
    {
        enemiesGrid.Children.Clear();
        _allEnemies.Clear();
        int enemyNumber = 0;

        if (_level < 2)
            _allEnemies.Add(Enemies.Dummy);

        else if (_level < 5)
            enemyNumber = 2;
        else if (_level < 10)
            enemyNumber = 3;
        else if (_level < 20)
            enemyNumber = 4;
        else if (_level < 30)
            enemyNumber = 5;

            for (int i = 0; i < enemyNumber; i++)
                _allEnemies.Add((Enemy)Enemies.ReadyEnemies[_rng.Next(Enemies.ReadyEnemies.Count)].Clone());

        for(int i = 0; i < _allEnemies.Count; i++)
        {
            var enemyImage = new Image
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                TranslationX = _allEnemies[i].Location.X,
                TranslationY = _allEnemies[i].Location.Y,
                WidthRequest = hitboxRadius,
                HeightRequest = hitboxRadius,
                Source = _allEnemies[i].SpySpritePath,
                Aspect = Aspect.Fill,
                IsAnimationPlaying = true,
            };

            _allEnemies[i].HitBoxRadius = _allEnemies[i].HitBoxRadius * (int)hitboxRadius;
            enemiesGrid.Children.Add(enemyImage);
        }


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

        _tileWidth = boardGrid.Width / _map.Map.GetLength(0);
        _tileHeight = boardGrid.Height / _map.Map.GetLength(1);

    }

    private void GenerateItems()
    {
        _allItems = new List<Item>();
        itemGrid.Children.Clear();

        int itemsToSpawn = Math.Min(_map.EmptyTiles.Count, _level * 3); // Increase with level but capped

        // Shuffle tile positions
        var emptyTiles = _map.EmptyTiles.OrderBy(t => _rng.Next()).ToList();

        for (int i = 0; i < Math.Min(itemsToSpawn, emptyTiles.Count); i++)
        {
            var (tileX, tileY) = emptyTiles[i];
            var item = (_rng.Next(20) == 1) ? Item.RareItem() : Item.NormalItem();

            // Center item in tile
            item.Location = new PointF(
                (float)(tileX * _tileWidth + _tileWidth / 2 ),
                (float)(tileY * _tileWidth + _tileWidth / 2)
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
            };

            itemGrid.Children.Add(image);
        }
    }

    #endregion

    #region Player Controls

    //Changing from isWalkable to ContainSurroundings and instead of checking the wall it checks if nearby tiles contain the element of the chosen number
    public static bool ContainSurroundings(float x, float y, double radius, int tileID)
    {
        if (boardHeight <= 0 || boardWidth <= 0)
            return false; // avoid early invalid checks

        if (x - radius < 0 || x + radius >= boardWidth) return true;
        if (y - radius < 0 || y + radius >= boardHeight) return true;

        int leftTile = (int)((x - radius) / _tileWidth);
        int rightTile = (int)((x + radius) / _tileWidth);
        int topTile = (int)((y - radius) / _tileHeight);
        int bottomTile = (int)((y + radius) / _tileHeight);

        for (int i = leftTile; i <= rightTile; i++)
        {
            for (int j = topTile; j <= bottomTile; j++)
            {
                if (_map.Map[i, j] == tileID) return true; // Found tile
            }
        }

        return false;
    }

    private void MovementDirectionChanged(object sender, PointF e) => _movePosition = e;

    private void Attack(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Completed:
            case GestureStatus.Canceled:

                if (_player.NumberOfShotsShot > 1) return;

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
    public float PlayerTranslationX => (float)(_player.Location.X - hitboxRadius);
    public float PlayerTranslationY => (float)(_player.Location.Y - hitboxRadius);
    public string PlayerScore => _player.Score.ToString().PadLeft(4, '0');



    public new event PropertyChangedEventHandler? PropertyChanged;
    protected override void OnPropertyChanged([CallerMemberName] string ?name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    #endregion
}