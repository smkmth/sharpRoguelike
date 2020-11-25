using System;
using System.Collections.Generic;
using System.Text;
using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Interfaces;
namespace sharpRoguelike.Core
{
    public class Actor : IActor, IDrawable
    {
        public string name { get ; set ; }
        public int awareness { get; set ; }
        public RLColor color { get; set; }
        public char symbol { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public void Draw(RLConsole con, IMap map)
        {
            if (!map.GetCell(x, y).IsExplored)
            {
                return;
            }

            if (map.IsInFov(x, y))
            {
                con.Set(x, y, color, Colors.FloorBackgroundFov, symbol);

            }
            else
            {
                con.Set(x, y, Colors.Floor, Colors.FloorBackground, '.');
            }
        }
    }
}
