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


        private int[,] _map = new int[0,0];
        private int _entrance = 0;
        private int _exit = 0;

        public static readonly Dictionary<int, Color> MapColors = new Dictionary<int, Color>
        {
            {0, Colors.Azure },
            {1, Colors.Green },
            {2, Colors.Red },
            {3, Colors.Black },
        };


        public int Entrance { get { return _entrance; } }
        public int Exit { get { return _exit; } }
        public int[,] Map { get { return _map; } }

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

            int entry, exit;
            entry = rand.Next(rows);
            exit = rand.Next(rows);

            ////Loop for adding obstacles
            //for(int i = 0; i < 5; i++)
            //{
            //    int[,] randomBlock = Blocks[rand.Next(Blocks.Count)];

            //    int BlockRotation = rand.Next(4);

            //}

            var SelectedBlock = rand.Next(4);

            for (int i = 0; i < Blocks[SelectedBlock].GetLength(0); i++)
            {
                for(int j = 0; j < Blocks[SelectedBlock].GetLength(1); j++)
                {
                    _map[i + 3, j + 4] = Blocks[SelectedBlock][i,j];
                }
            }

            _map[0,entry] = 1;
            _map[columns - 1,exit] = 2;

            return Map;
        }
    }
}
