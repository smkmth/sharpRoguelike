using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Interfaces;


namespace sharpRoguelike.Core
{
    public class Entity : IDrawable
    {

        public RLColor color { get; set; }
        public char symbol { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string name { get; set; }

        public Inventory inventory = null;
        public Attacker attacker = null;
        public Actor actor = null;
        public Useable effect = null;
        public Entity corpse = null;

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


        public virtual void DrawStats(RLConsole statConsole, int pos) {}
    }
}
