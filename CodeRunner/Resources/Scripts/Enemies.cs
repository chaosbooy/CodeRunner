using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner.Resources.Scripts
{
    internal class Enemies
    {
        public Enemies() { }

        public readonly static Enemy Dummy = new Enemy
        {
            Name = "dummy",
            Description = "most basic enemy moves randomly",
            Difficulty = 1,

        };

        public readonly static Enemy Base = new Enemy
        {
            Name = "base",
            Description = "turns other enemies into their spy version",
            Difficulty = 2,
        };

        public readonly static Enemy Eye = new Enemy
        {
            Name = "eye",
            Description = "sees everything when base is around and tries to find the player when it's destroyed",
            Difficulty = 3,
        };

        public readonly static Enemy Star = new Enemy
        {
            Name = "star",
            Description = "tries to find the player using shortest route seeing everything, when base is destroyed does something else idk",
            Difficulty = 6,
        };

        public readonly static Enemy Camper = new Enemy
        {
            Name = "camper",
            Description = "camps around the base but closes in when sees the player, when the base is destroyed kills itself",
            Difficulty = 4,
        };

        public readonly static Enemy Ghost = new Enemy
        {
            Name = "ghost",
            Description = "goes through walls seing the player everywhere, when the base is destroyed doesn't know where to go to",
            Difficulty = 5,
        };
    }
}
