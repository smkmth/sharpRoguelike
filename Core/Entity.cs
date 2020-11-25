using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    public class Entity : IDrawable
    {

        public RLColor color { get; set; }
        public char symbol { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string name { get; set; }

        public Inventory inventory = null;    

        public virtual void Draw(RLConsole con, IMap map)
        {
            ICell cell = map.GetCell(x, y);

            if (!cell.IsExplored)
            {
                return;
            }

            if (map.IsInFov(x, y))
            {
                con.Set(x, y, color, Colors.FloorBackgroundFov, symbol);

            }
            else
            {
                con.Set(x, y, Colors.Floor, Colors.FloorBackground, ' ');
            }
        }

    }
}
