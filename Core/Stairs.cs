using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    public class Stairs : IDrawable 
    { 

        public RLColor color { get; set; }
        public char symbol { get; set ; }
        public int x { get; set ; }
        public int y { get; set; }        
        public bool IsUp { get; set; }

        public void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(x, y).IsExplored) 
            {
                return;
            }

            symbol = IsUp ? '<' : '>';

            if (map.IsInFov(x, y))
            {
                color = Colors.Player;
            }
            else
            {
                color = Colors.Floor;
            }

            console.Set(x, y, color, null, symbol);
        }


    }
}
