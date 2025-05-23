using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner.Resources.Scripts
{
    public struct Item
    {

        public PointF Location { get; set; }
        public float Padding { get; set; }
        public int Score { get; set; }
        public float Radius { get; set; }
        public string SpritePath { get; set; }

        public static Item NormalItem ()
        {
            return new Item
            {
                Padding = 2,
                Score = 1,
                Radius = 5,
                SpritePath = "NormalItem.gif",
            };
        }

        public static Item RareItem()
        {
            return new Item
            {
                Padding = 4,
                Score = 3,
                Radius = 8,
                SpritePath = "RareItem.gif"
            };
        }
    }
}
