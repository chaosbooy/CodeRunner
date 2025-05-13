namespace CodeRunner.Resources.Scripts
{
    public class Enemy : ICloneable
    {
        public string Name { get; set; }
        public string Description { get; set; }

        private Point _location;
        public int Difficulty { get; set; }

        public string SpySpritePath{ get; set; }
        public event EventHandler SpyMovementScript;

        //if the enemy doesnt use the base the blind version will be the same
        public string BlindSpritePath { get; set; }
        public event EventHandler BlindBaseMovementScript;


        public Enemy()
        {

        }

        public void Kaboooomm()
        {

        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
