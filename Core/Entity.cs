using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace sharpRoguelike.Core
{
    [Serializable]
    public struct RLSerialiseableColor
    {
        public float r;
        public float g;
        public float b;

        public RLSerialiseableColor(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }

    [Serializable]
    public class Entity : IDrawable
    {
        RLSerialiseableColor s_color;
        [NonSerialized()] private RLColor _color;
        public RLColor color
        {
            get { return _color; }
            set { _color = value; }
        }
        public char symbol { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string name { get; set; }

        public Inventory inventory = null;  //does this entity have an inventory
        public Attacker attacker = null;    //does this entity attack
        public Actor actor = null;          //does this entity move
        public Useable effect = null;       //does this enity have an effect when used
        public Entity corpse = null;        //does this entity have a corpse
        public Surface surface = null;      //does this entity have an effect when walked on
        public Player player = null;        //is this entity a player
        public AI ai = null;                //does this entity have AI


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

        public void LoadEntityColor()
        {
            _color = new RLColor(s_color.r, s_color.g, s_color.b);
            

        }

        public void SaveEntityColor()
        {
            s_color = new RLSerialiseableColor(_color.r, _color.g, _color.b);
         
        }


        public virtual void DrawStats(RLConsole statConsole, int pos) {}
    }
}
