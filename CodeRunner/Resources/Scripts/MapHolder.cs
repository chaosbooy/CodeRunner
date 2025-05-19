namespace CodeRunner.Resources.Scripts
{
    public class MapHolder
    {
        private int[,] _map;
        private int _entrance;
        private int _exit;

        public readonly Dictionary<int, Color> MapColors = new Dictionary<int, Color>
        {
            {0, Colors.Azure },
            {1, Colors.Green },
            {2, Colors.Red },
            {3, Colors.Black },
        };


        public int Entrance { get { return _entrance; } }
        public int Exit { get { return _exit; } }
        public int[,] Map { get { return _map; } }

        public static List<int[,]> Blocks = new List<int[,]>
        {
            new int[,]
            {
                { 1 }
            },
            new int [,]
            {
                { 1, 1 },
            },
            new int[,]
            {
               { 1, 1 },
               { 1, 1 },
            },
            new int [,]
            {
                {1, 1 },
                { 1, 0 },
                { 1, 1 },
            },
        };

        public MapHolder ()
        {
            _map = new int[0,0];
        }

        public int[,] GenerateMap(int columns, int rows, int seed = 0)
        {
            _map = new int[columns, rows];

            Random rand = new Random(seed);

            int entry, exit;
            entry = rand.Next(rows);
            exit = rand.Next(rows);

            for(int i = 0; i < 5; i++)
            {
                int[,] randomBlock = Blocks[rand.Next(Blocks.Count)];

                int BlockRotation = rand.Next(4);


            }

            _map[0,entry] = 1;
            _map[columns - 1,exit] = 2;

            return Map;
        }
    }
}
