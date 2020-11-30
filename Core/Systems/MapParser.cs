using RogueSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace sharpRoguelike.Core.Systems
{
    class Room
    {
        public List<Point> points;

    }

    struct CellCopy
    {
        public bool walkable;
        public int x;
        public int y;

        public CellCopy(bool walkable, int x, int y)
        {
            this.walkable = walkable;
            this.x = x;
            this.y = y;
        }
    }

    class MapParser
    {
        DungeonMap map;
        int mapWidth;
        int mapHeight;
        List<Room> rooms;
        CellCopy[] cells;

        public MapParser(DungeonMap _map, int _mapWidth, int _mapHeight)
        {
            map = _map;
            mapWidth = _mapWidth;
            mapHeight = _mapHeight;

        }


        public DungeonMap Pass()
        {


            foreach (ICell cell in map.GetCellsInColumns(mapWidth - 1))
            {
                map.SetCellProperties(cell.X, cell.Y, false, false);
            }
            foreach (ICell cell in map.GetCellsInColumns(0))
            {
                map.SetCellProperties(cell.X, cell.Y, false, false);
            }
            foreach (ICell cell in map.GetCellsInRows(mapHeight - 1))
            {
                map.SetCellProperties(cell.X, cell.Y, false, false);
            }
            foreach (ICell cell in map.GetCellsInRows(0))
            {
                map.SetCellProperties(cell.X, cell.Y, false, false);
            }

            cells = new CellCopy[mapWidth * mapHeight];
            foreach (ICell cell in map.GetAllCells())
            {
                cells[cell.X + mapWidth * cell.Y] = new CellCopy(cell.IsWalkable, cell.X, cell.Y );
            }

            rooms = GetRooms(cells);
            Room StartRoom = rooms[0]; // rooms[Game.Random.Next(0, rooms.Count)];

            PlacePlayer(StartRoom.points[0].X, StartRoom.points[0].Y);

            return map;
            


        }

        public List<Room> GetRooms(CellCopy[] runmap)
        {
            List<Room> rooms = new List<Room>();
            for(int x = 1; x < mapWidth-1; x++)
            {
                for (int y =1; y < mapHeight-1; y++)
                {
                    
                    if (runmap[x + mapWidth * y].walkable)
                    {
                        Room room = new Room();
                        room.points = FloodFill(new Point(x, y), runmap);
                        rooms.Add(room);
                    }
                }
            }
          
          
            return rooms;
        }

        private List<Point> FloodFill( Point pt, CellCopy[] runmap)
        {
            List<Point> contiguousPoints = new List<Point>();
            Stack<Point> points = new Stack<Point>();

            points.Push(pt);

            while (points.Count > 0)
            {
                Point a = points.Pop();
                if (a.X < mapWidth -1 && a.X > 0 && a.Y < mapHeight -1 && a.Y > 0)
                {

                    if (runmap[a.X + mapWidth * a.Y].walkable)
                    {
                        runmap[a.X + mapWidth * a.Y].walkable = false;
                        contiguousPoints.Add(new Point(a.X, a.Y));
                        points.Push(new Point(a.X - 1, a.Y));
                        points.Push(new Point(a.X + 1, a.Y));
                        points.Push(new Point(a.X, a.Y - 1));
                        points.Push(new Point(a.X, a.Y + 1));
                    }
                }
            }
            return contiguousPoints;
        }


        public void PlacePlayer( int x, int y)
        {
            Entity player = Game.Player;
            if (player == null)
            {
                player = new Entity();
            }
            player.x = x;
            player.y = y;

            map.AddPlayer(player);
        }
    }
}
