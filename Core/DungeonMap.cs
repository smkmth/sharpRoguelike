using System;
using System.Collections.Generic;
using System.Text;
using RLNET;
using RogueSharp;

namespace sharpRoguelike.Core
{
    public class DungeonMap : Map 
    {
        public void Draw(RLConsole mapConsole )
        {
            mapConsole.Clear();
            foreach(Cell cell in GetAllCells())
            {

                SetConsoleSymbolForCell(mapConsole, cell);
            }
        }

        public void SetConsoleSymbolForCell(RLConsole con, Cell cell)
        {
            if (!cell.IsExplored)
            {
                return;

            }

            if (IsInFov(cell.X, cell.Y))
            {
                if (cell.IsWalkable)
                {
                    con.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                }
                else
                {
                    con.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
                }
            }
            else
            {
                if (cell.IsWalkable)
                {
                    con.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackground, ' ');
                }
                else
                {
                    con.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackground, '#');
                }
            }
        }

        public void UpdatePlayerFOV()
        {
            Player player = Game.Player;
            ComputeFov(player.x, player.y, player.awareness, true);
            foreach(Cell cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);

                }
            }

        }

        public bool SetActorPosition(Actor actor, int x, int y)
        {
            if (GetCell(x, y).IsWalkable)
            {
                SetIsWalkable(actor.x, actor.y, true);
                actor.x = x;
                actor.y = y;

                SetIsWalkable(actor.x, actor.y, false);
                if (actor is Player)
                {
                    UpdatePlayerFOV();
                }
                return true;

            }
                return false;

        }

        public void SetIsWalkable(int x, int y, bool walkable)
        {
            SetCellProperties(x, y, GetCell(x,y).IsTransparent, walkable, GetCell(x,y).IsExplored);

        }

    }
}
