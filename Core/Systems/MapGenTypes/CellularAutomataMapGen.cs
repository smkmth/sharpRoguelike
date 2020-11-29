using RogueSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Systems.MapGenTypes
{
    public class CellularAutomataMapGen
    {
        private readonly int width;
        private readonly int height;

        private readonly int percentAreWalls;
        private readonly int mapLevel;

        private DungeonMap map;

        public CellularAutomataMapGen(int _width, int _height, int _percentAreWalls, int _mapLevel)
        {
            width = _width;
            height = _height;
            percentAreWalls = _percentAreWalls;
            mapLevel = _mapLevel;

            map = new DungeonMap();

        }
        public DungeonMap CreateMap()
        {
            map.Initialize(width, height);

            return map;

        }

		public void MakeCaverns()
		{
			// By initilizing column in the outter loop, its only created ONCE
			for (int column = 0, row = 0; row <= height - 1; row++)
			{
				for (column = 0; column <= width - 1; column++)
				{
					bool walllogic = PlaceWallLogic(column, row);
					map.SetIsWalkable(row, column, walllogic);
				}
			}
		}

		public bool PlaceWallLogic(int x, int y)
		{
			int numWalls = GetAdjacentWalls(x, y, 1, 1);


			if (map.GetCell(x,y).IsWalkable)
			{
				if (numWalls >= 4)
				{
					return true;
				}
				if (numWalls < 2)
				{
					return false;
				}

			}
			else
			{
				if (numWalls >= 5)
				{
					return true;
				}
			}
			return false;
		}

		public int GetAdjacentWalls(int x, int y, int scopeX, int scopeY)
		{
			int startX = x - scopeX;
			int startY = y - scopeY;
			int endX = x + scopeX;
			int endY = y + scopeY;

			int iX = startX;
			int iY = startY;

			int wallCounter = 0;

			for (iY = startY; iY <= endY; iY++)
			{
				for (iX = startX; iX <= endX; iX++)
				{
					if (!(iX == x && iY == y))
					{
						if (IsWall(iX, iY))
						{
							wallCounter += 1;
						}
					}
				}
			}
			return wallCounter;
		}

		bool IsWall(int x, int y)
		{
			// Consider out-of-bound a wall
			if (IsOutOfBounds(x, y))
			{
				return true;
			}
			
			ICell cell = map.GetCell(x, y);

			return cell.IsWalkable;
	
		}

		bool IsOutOfBounds(int x, int y)
		{
			if (x < 0 || y < 0)
			{
				return true;
			}
			else if (x > width - 1 || y > height - 1)
			{
				return true;
			}
			return false;
		}

	



		public void BlankMap()
		{
			for (int column = 0, row = 0; row < height; row++)
			{
				for (column = 0; column < width; column++)
				{
					map.SetIsWalkable(row, column ,false);

				}
			}
		}

		public void RandomFillMap()
		{
			// New, empty map

			int mapMiddle = 0; // Temp variable
			for (int column = 0, row = 0; row < height; row++)
			{
				for (column = 0; column < width; column++)
				{
					// If coordinants lie on the the edge of the map (creates a border)
					if (column == 0)
					{
						map.SetIsWalkable(column, row,true);
					}
					else if (row == 0)
					{
						map.SetIsWalkable(column, row, true);
					}
					else if (column == width - 1)
					{
						map.SetIsWalkable(column, row, true);
					}
					else if (row == width - 1)
					{
						map.SetIsWalkable(column, row, true);
					}
					// Else, fill with a wall a random percent of the time
					else
					{
						mapMiddle = (height / 2);

						if (row == mapMiddle)
						{
							map.SetIsWalkable(column, row, false);
						}
						else
						{
							map.SetIsWalkable(column, row, RandomPercent(percentAreWalls));
						}
					}
				}
			}
		}

		bool RandomPercent(int percent)
		{
			if (percent >= Game.Random.Next(1, 101))
			{
				return true;
			}
			return false;
		}

	
	}
}
