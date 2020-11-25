using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    public class Door :  Entity
    {
        public Door()
        {
            symbol = '+';
            color = Colors.Door;
            BackgroundColor = Colors.DoorBackground;
        }

        public bool isOpen { get; set; }
        public RLColor BackgroundColor { get; set; }

        public override void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(x, y).IsExplored)
            {
                return;
            }

            symbol = isOpen ? '-' : '+';
            name = isOpen ? "A open door" : "A closed door";

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
