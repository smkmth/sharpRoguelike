using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    public class Door : IDrawable
    {
        public Door()
        {
            symbol = '+';
            color = Colors.Door;
            BackgroundColor = Colors.DoorBackground;
        }

        public bool isOpen { get; set; }
        public RLColor color { get; set; }
        public RLColor BackgroundColor { get; set; }
        public char symbol { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(x, y).IsExplored)
            {
                return;
            }

            symbol = isOpen ? '-' : '+';

            if (map.IsInFov(x, y))
            {
                color = Colors.DoorFov;
                BackgroundColor = Colors.DoorBackgroundFov;
            }
            else
            {
               
                color = Colors.Door;
                BackgroundColor = Colors.DoorBackground;
            }

            console.Set(x, y, color, BackgroundColor, symbol);

        }
    }
}
