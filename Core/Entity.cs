using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Interfaces;
using System.Collections.Generic;

namespace sharpRoguelike.Core
{
    public class Entity : IDrawable
    {

        public RLColor color { get; set; }
        public char symbol { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string name { get; set; }

        public Inventory inventory = null; //does this entity have an inventory
        public Attacker attacker = null;    //does this entity attack
        public Actor actor = null;          //does this entity move
        public Useable effect = null;       //does this enity have an effect when used
        public Entity corpse = null;        //does this entity have a corpse
        public Surface surface = null;      //does this entity have an effect when walked on

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

        public virtual void Create()
        {

        }

        public virtual void DrawStats(RLConsole statConsole, int pos) {}
    }
}
