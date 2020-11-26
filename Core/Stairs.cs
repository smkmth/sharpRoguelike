﻿using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    [Serializable]
    public class Stairs : Entity 
    { 
 
        public bool IsUp { get; set; }

        public override void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(x, y).IsExplored) 
            {
                return;
            }
            symbol = IsUp ? '<' : '>';
            name = IsUp ? "Stairs leading up" : "Stairs leading down";

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
