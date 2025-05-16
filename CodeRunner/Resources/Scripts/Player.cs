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


        private float _speed;
        public Point Location { get; set; }
        public float Rotation { get; set; }

        public int Score { get; set; }

        public string SpySpritePath { get; set; }
        public event EventHandler SpyMovementScript;


        public Player()
        {

        }

        public void MovePlayer(Point direction)
        {

        }

        public void RotatePlayer(Point direction)
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
