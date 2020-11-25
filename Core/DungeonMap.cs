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


    }
}
