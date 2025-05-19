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

        public string SpritePath { get; set; }
        public string AttackSpritePath { get; set; }


        public Player()
        {

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
    }
}
