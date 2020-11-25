using RogueSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    public class MapGenerator
    {
        private readonly int width;
        private readonly int height;

        private readonly DungeonMap map;

        public MapGenerator(int _width, int _height)
        {
            width = _width;
            height = _height;

            map = new DungeonMap();

        }

        public DungeonMap CreateMap()
        {
            map.Initialize(width, height);
            foreach(Cell cell in map.GetAllCells())
            {
                map.SetCellProperties(cell.X, cell.Y, true, true, true);
            }

            foreach(Cell cell in map.GetCellsInRows(0, height - 1))
            {
                map.SetCellProperties(cell.X, cell.Y, false, false, true);
            }

            foreach (Cell cell in map.GetCellsInColumns(0, width - 1))
            {
                map.SetCellProperties(cell.X, cell.Y, false, false, true);
            }

            return map;
        }

    }
}
