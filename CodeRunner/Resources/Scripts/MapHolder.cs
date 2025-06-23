namespace CodeRunner.Resources.Scripts
{
    public sealed class MapHolder
    {
        #region Sealing this class

        private static readonly MapHolder instance = new MapHolder();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static MapHolder()
        {
        }

        private MapHolder()
        {

        }

        public static MapHolder Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion


        private int[,] _map = new int[0, 0];
        private int _entrance = 0;
        private int _exit = 0;

        List<(int,int)> _emptyTiles = new List<(int x, int y)>();

        public static readonly Dictionary<int, Color> MapColors = new Dictionary<int, Color>
        {
            {0, Colors.Gray },
            {1, Colors.DarkGreen },
            {2, Colors.DarkRed },
            {3, Colors.Black },
        };


        public int Entrance { get { return _entrance; } }
        public int Exit { get { return _exit; } }
        public int[,] Map { get { return _map; } }
        public List<(int,int)> EmptyTiles { get { return _emptyTiles; } set { _emptyTiles = value; } }

        public List<int[,]> Blocks = new List<int[,]>
        {
            new int[,]
            {
                { 3 }
            },
            new int [,]
            {
                { 3, 3 },
            },
            new int[,]
            {
               { 3, 3 },
               { 3, 3 },
            },
            new int [,]
            {
                {3, 3},
                { 3, 0 },
                { 3, 3 },
            },
        };

        public int[,] GenerateMap(int columns, int rows, int seed = 0)
        {
            _map = new int[columns, rows];
            Random rand = new Random(seed);

            _entrance = rand.Next(rows);
            _exit = rand.Next(rows);
            _map[0, _entrance] = 1;
            _map[columns - 1, _exit] = 2;

            int blocksToPlace = 5 + rand.Next(8); // At least 5 blocks, up to 12

            int maxAttempts = 100;

            for (int b = 0; b < blocksToPlace; b++)
            {
                int[,] block = Blocks[rand.Next(Blocks.Count)];
                int rotations = rand.Next(4);

                for (int r = 0; r < rotations; r++)
                    block = RotateBlock(block);

                bool placed = false;
                for (int attempt = 0; attempt < maxAttempts && !placed; attempt++)
                {
                    int maxX = columns - block.GetLength(0);
                    int maxY = rows - block.GetLength(1);

                    int x = rand.Next(1, maxX - 1); // avoid corners
                    int y = rand.Next(1, maxY - 1); // avoid corners

                    if (CanPlaceBlock(x, y, block, columns, rows))
                    {
                        PlaceBlock(x, y, block);
                        placed = true;
                    }
                }
            }

            CountEmpty();

            return _map;
        }

        private int[,] RotateBlock(int[,] block)
        {
            int width = block.GetLength(0);
            int height = block.GetLength(1);
            int[,] result = new int[height, width];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    result[y, width - 1 - x] = block[x, y];

            return result;
        }

        private bool CanPlaceBlock(int startX, int startY, int[,] block, int mapWidth, int mapHeight)
        {
            int blockWidth = block.GetLength(0);
            int blockHeight = block.GetLength(1);

            for (int x = 0; x < blockWidth; x++)
            {
                for (int y = 0; y < blockHeight; y++)
                {
                    if (block[x, y] == 0)
                        continue;

                    int mapX = startX + x;
                    int mapY = startY + y;

                    if (mapX < 0 || mapY < 0 || mapX >= mapWidth || mapY >= mapHeight)
                        return false;

                    // Check surrounding cells to avoid touching
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            int nx = mapX + dx;
                            int ny = mapY + dy;
                            if (nx >= 0 && ny >= 0 && nx < mapWidth && ny < mapHeight && _map[nx, ny] != 0)
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        private void PlaceBlock(int startX, int startY, int[,] block)
        {
            int width = block.GetLength(0);
            int height = block.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (block[x, y] != 0)
                        _map[startX + x, startY + y] = block[x, y];
                }
            }
        }

        private void CountEmpty()
        {
            _emptyTiles.Clear();
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    if (Map[x, y] == 0) // Walkable tile
                    {
                        _emptyTiles.Add((x, y));
                    }
                }
            }
        }
    }
}
