using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    //a door is a unique entity because its drawn wierd - it has two states
    //open or not open - which changes how its drawn. 
    [Serializable]
    public class Door :  Entity
    {
        public Door()
        {
            symbol = '+';
            color = Colors.Door;
            BackgroundColor = Colors.DoorBackground;
        }

        public bool isOpen { get; set; }
        [NonSerialized()] private RLColor bgcolor;
        public RLColor BackgroundColor { get { return bgcolor; } set { bgcolor = value; } }

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
