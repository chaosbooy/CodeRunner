using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner.Resources.Scripts
{

    public class Player
    {
        public string Name { get; set; }

        public float Speed {get; set; }
        public PointF Location { get; set; }
        public float Rotation { get; set; }

        public int Score { get; set; }

        public int NumberOfShotsShot { get; set; }

        public Projectile ProjectileStyle { get; set; }

        public string SpritePath { get; set; }
        public string AttackSpritePath { get; set; }


        public Player()
        {
            Name = "Just a virus";
            Speed = 0;
            Location = new PointF(0, 0);
            Rotation = 0;
            Score = 0;
            Score = 0;
            NumberOfShotsShot = 0;
            ProjectileStyle = new Projectile();
            SpritePath = "";
            AttackSpritePath = "";
        }

        public void Move(PointF direction)
        {
            Location = new PointF(Location.X + direction.X * Speed, Location.Y + direction.Y * Speed);
        }

        //changes sprite from 
        public void Rotate(PointF direction)
        {

        }

        public void Kaboooomm()
        {

        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            return $"Name: {Name} \n Location: {Location} \n Rotation: {Rotation} \n NumberOfShotsShot: {NumberOfShotsShot} \n";
        }
    }
}
