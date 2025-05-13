namespace CodeRunner.Resources.Scripts
{
    public class MapHolder
    {
        private int[,] _map;

        public int[,] Map { get { return _map; } }

        public MapHolder ()
        {
            _map = new int[0,0];
        }

        public int[,] GenerateMap(int columns, int rows)
        {
            _map = new int[columns, rows];

            Random rand = new Random();

            int entry, exit;
            entry = rand.Next(rows);
            exit = rand.Next(rows);

            _map[0,entry] = 1;
            _map[columns - 1,exit] = 2;

            return Map;
        }
    }
}
