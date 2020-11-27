using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    //stairs are a unique entity because again they are drawn in a
    //funny way - up stairs and down stairs need to be drawn differently

    [Serializable]
    public class Stairs : Entity 
    { 
 
        public bool IsUp { get; set; }
        public override void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(x, y).IsExplored && !Clairvoince) 
            {
                return;
            }
            symbol = IsUp ? '<' : '>';
            name = IsUp ? "Stairs leading up" : "Stairs leading down";

            if (map.IsInFov(x, y) || Clairvoince)
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
