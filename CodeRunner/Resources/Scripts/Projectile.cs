namespace CodeRunner.Resources.Scripts
{
    public struct Projectile : ICloneable
    {
        public PointF Location { get; set; }
        public float Rotation { get; set; }

        public PointF Speed { get; set; }
        public float SpeedMultiplier { get; set; }

        public int radius;

        public string SpritePath { get; set; }

        public void Move() =>
            Location = new PointF(Location.X + (Speed.X * SpeedMultiplier), Location.Y + (Speed.Y * SpeedMultiplier));

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
