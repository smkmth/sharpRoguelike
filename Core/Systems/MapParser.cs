using RogueSharp;
using sharpRoguelike.Core.Data.Items;
using sharpRoguelike.Core.Data.Monsters;
using sharpRoguelike.Core.Items;
using sharpRoguelike.Core.Monsters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace sharpRoguelike.Core.Systems
{
    class Room
    {
        public List<Point> points;
        public Point Center()
        {
            int xtot = 0;
            int ytot = 0;
            foreach(Point point in points)
            {
                xtot += point.X;
                ytot += point.Y;
            }

            xtot /= points.Count;
            ytot /= points.Count;

            return new Point(xtot, ytot);
        }
       
        public int GetExtremeLeft()
        {
            Point bestfit = points[0];
            foreach (Point point in points)
            {
                if (point.X < bestfit.X)
                {
                    bestfit = point;
                }
            }
            return bestfit.X;
        }
        public Point GetRandomPointInRoom()
        {
            return points[Game.Random.Next(0, points.Count-1)];
        }
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
        int mapLevel;

        List<Room> rooms;
        CellCopy[] cells;
        Room StartRoom;

        public MapParser(DungeonMap _map, int _mapWidth, int _mapHeight, int _mapLevel)
        {
            map = _map;
            mapWidth = _mapWidth;
            mapHeight = _mapHeight;
            mapLevel = _mapLevel;
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
           
            
            List<Room> sortedRooms = rooms.OrderByDescending(o => o.GetExtremeLeft()).ToList();

            for (int r = 0; r < sortedRooms.Count; r++)
            {
                if (r > 0)
                {
                    int previousRoomCenterX =   sortedRooms[r - 1].Center().X;
                    int previousRoomCenterY =   sortedRooms[r - 1].Center().Y;
                    int currentRoomCenterX =    sortedRooms[r].Center().X;
                    int currentRoomCenterY =    sortedRooms[r].Center().Y;

                    if (Game.Random.Next(1, 2) == 1)
                    {
                        CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, previousRoomCenterY);
                        CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, currentRoomCenterX);
                    }
                    else
                    {
                        CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, previousRoomCenterX);
                        CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, currentRoomCenterY);
                    }

                }

            }


            StartRoom = rooms[Game.Random.Next(0, rooms.Count)];
            PlacePlayer(StartRoom.points[0].X, StartRoom.points[0].Y);

            CreateStairs();
            PlaceMonsters();
            PlaceItems();
            

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

        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                map.SetCellProperties(x, yPosition, true, true);
            }
        }

        // Carve a tunnel out of the map parallel to the y-axis
        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                map.SetCellProperties(xPosition, y, true, true);
            }
        }
        public void PlacePlayer(int x, int y)
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

        private void CreateStairs()
        {
            if (mapLevel > 1)
            {

                Point stairpoint = StartRoom.GetRandomPointInRoom();

                Stairs upstairs = new Stairs
                {
                    x = stairpoint.X,
                    y = stairpoint.Y,
                    IsUp = true

                };
                map.Entities.Add(upstairs);
                map.StairsUp = upstairs;

            }

            Point endPoint = rooms[Game.Random.Next(0, rooms.Count)].GetRandomPointInRoom();

            Stairs downstairs = new Stairs
            {
                x = endPoint.X,
                y = endPoint.Y,
                IsUp = false
            };

            map.Entities.Add(downstairs);
            map.StairsDown = downstairs;

        }

        private void PlaceItems()
        {
            foreach (var room in rooms)
            {
                if (Game.Random.Next(0, 10) < 4)
                {
                    var items = Game.Random.Next(1, 2);
                    for (int i = 0; i < items; i++)
                    {
                        Point randomRoomLocation = room.GetRandomPointInRoom();
                    
                        int roll = Game.Random.Next(0, 100);
                        if (roll < 10)
                        {
                            var potion = HealthPotion.Create();
                            potion.x = randomRoomLocation.X;
                            potion.y = randomRoomLocation.Y;
                            map.AddItem(potion);
                        }
                        else if (roll < 50)
                        {
                            var chest = Chest.Create();
                            chest.x = randomRoomLocation.X;
                            chest.y = randomRoomLocation.Y;
                            map.AddItem(chest);
                        }
                        else if (roll < 75)
                        {
                            var potion = WaterPotion.Create();
                            potion.x = randomRoomLocation.X;
                            potion.y = randomRoomLocation.Y;
                            map.AddItem(potion);
                        }
                        else
                        {
                            var potion = SlimePotion.Create();
                            potion.x = randomRoomLocation.X;
                            potion.y = randomRoomLocation.Y;
                            map.AddItem(potion);

                        }
                        
                    }
                }

            }
        }


        private void PlaceMonsters()
        {
            foreach (var room in rooms)
            {
                if (Game.Random.Next(0, 10) < 7)
                {
                    int numberOfMonsters = 0;
                    if (room.points.Count > 20)
                    {
                        numberOfMonsters = Game.Random.Next(1, 4);
                    }
                    else if (room.points.Count > 50)
                    {
                        numberOfMonsters = Game.Random.Next(5, 10);

                    }
                    else if (room.points.Count > 100)
                    {
                        numberOfMonsters = Game.Random.Next(10, 15);

                    }
                    for (int i = 0; i < numberOfMonsters; i++)
                    {
                        Point randomRoomLocation = room.GetRandomPointInRoom();

                        int monsterRoll = Game.Random.Next(0, 100);
                        if (monsterRoll < 70)
                        {
                            var monster = Cryofailure.Create(mapLevel);
                            monster.x = randomRoomLocation.X;
                            monster.y = randomRoomLocation.Y;
                            map.AddMonster(monster);
                        }
                        else
                        {
                            var monster = Slimehulk.Create(mapLevel);
                            monster.x = randomRoomLocation.X;
                            monster.y = randomRoomLocation.Y;
                            map.AddMonster(monster);
                        }
                        
                    }
                }
            }
        }

    }
}
