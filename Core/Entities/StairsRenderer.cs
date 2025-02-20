﻿using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    //stairs are a unique entity because again they are drawn in a
    //funny way - up stairs and down stairs need to be drawn differently

    [Serializable]
    public class StairRenderer : Renderer 
    { 

        public StairRenderer(Entity _owner) : base(_owner) { }
 
        public bool IsUp { get; set; }
        public override void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(owner.transform.x, owner.transform.y).IsExplored && !Clairvoince) 
            {
                return;
            }
            symbol = IsUp ? '<' : '>';
            owner.name = IsUp ? "Stairs leading up" : "Stairs leading down";

            if (map.IsInFov(owner.transform.x, owner.transform.y) || Clairvoince)
            {
                color = Colors.Player;
            }
            else
            {
                color = Colors.Floor;
            }

            console.Set(owner.transform.x, owner.transform.y, color, null, symbol);
        }


    }
}
