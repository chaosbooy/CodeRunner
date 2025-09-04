namespace CodeRunner.Resources.Scripts
{
    public class Enemy : ICloneable
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Point Location { get; set; }
        public float Speed { get; set; } = 2f; // default speed

        public int Difficulty { get; set; }
        public int HitBoxRadius { get; set; }

        public string SpySpritePath{ get; set; }
        public event EventHandler SpyMovementScript;

        //if the enemy doesnt use the base the blind version will be the same
        public string BlindSpritePath { get; set; }
        public event EventHandler BlindMovementScript;


        public Enemy()
        {

        }

        public void Kaboooomm()
        {

        }

        public void Move(bool isSpy)
        {
            if(isSpy) SpyMovementScript.Invoke(this, new EventArgs());
            else BlindMovementScript.Invoke(this, new EventArgs());
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
