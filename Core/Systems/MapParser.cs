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
    public class Room
    {
        public List<Point> points;


        public Room()
        {
            points = new List<Point>();
        }

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



    public class MapSegment
    {
        public int xpos;
        public int ypos;
        public System.Drawing.Point sizeOfCopy;
        public DungeonMap map;

        public MapSegment(int xpos, int ypos, System.Drawing.Point sizeOfCopy, DungeonMap map)
        {
            this.xpos = xpos;
            this.ypos = ypos;
            this.map = map;
            this.sizeOfCopy = sizeOfCopy;
        }
    }

    class MapParser
    {
        public DungeonMap newMap;

        int mapWidth;
        int mapHeight;
        int mapLevel;

        List<Room> rooms;
        Room StartRoom;

        public int tinyRoomDef =3;
        public int smallRoomDef =5;
        public int midRoomDef = 10;
        public int largeRoomDef = 50;
        public int massiveRoomDef =100;
        public int superMassiveRoomDef = 500;
        public bool roomDebugging;

        public MapParser( int _mapWidth, int _mapHeight, int _mapLevel)
        {
            mapWidth = _mapWidth;
            mapHeight = _mapHeight;
            mapLevel = _mapLevel;
        }

        public DungeonMap SpliceMapSegemnts(List<MapSegment> splicedMapSegments)
        {
            DungeonMap splicedMap = new DungeonMap();
            splicedMap.Initialize(mapWidth, mapHeight);

            SerialiseableCells[,] copycells = new SerialiseableCells[mapWidth, mapHeight];
            for (int i = 0; i < splicedMapSegments.Count; i++)
            {
                copycells = CopyCells(copycells, splicedMapSegments[i].sizeOfCopy.X, splicedMapSegments[i].sizeOfCopy.Y, splicedMapSegments[i].map, splicedMapSegments[i].xpos, splicedMapSegments[i].ypos);
            }

            splicedMap = MakeMapFromCells(copycells);
            return splicedMap;
        }
        
        public DungeonMap Pass(DungeonMap originalMap, bool doBorders, bool doRooms, bool doCorridors, bool placeEntities)
        {
            newMap = originalMap;

            if (doBorders)
            {

                foreach (ICell cell in newMap.GetCellsInColumns(mapWidth - 1))
                {
                    newMap.SetCellProperties(cell.X, cell.Y, false, false);
                }
                foreach (ICell cell in newMap.GetCellsInColumns(0))
                {
                    newMap.SetCellProperties(cell.X, cell.Y, false, false);
                }
                foreach (ICell cell in newMap.GetCellsInRows(mapHeight - 1))
                {
                    newMap.SetCellProperties(cell.X, cell.Y, false, false);
                }
                foreach (ICell cell in newMap.GetCellsInRows(0))
                {
                    newMap.SetCellProperties(cell.X, cell.Y, false, false);
                }

            }


            if (doRooms)
            {

                SerialiseableCells[] cells = new SerialiseableCells[mapWidth * mapHeight];
                foreach (ICell cell in newMap.GetAllCells())
                {
                    cells[cell.X + mapWidth * cell.Y] = new SerialiseableCells( cell.X, cell.Y , cell.IsTransparent, cell.IsWalkable, cell.IsExplored);
                }

                rooms = GetRooms(cells);

                if (doCorridors)
                {

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
                }


                StartRoom = rooms[Game.Random.Next(0, rooms.Count)];
                PlacePlayer(StartRoom.points[0].X, StartRoom.points[0].Y);

                if (placeEntities)
                {
                    CreateStairs();
                    PlaceMonsters();
                    PlaceItems();

                }

                if (roomDebugging)
                {

                    int counter = 0;

                    int tinyRooms = 0;
                    int smallRooms = 0;
                    int midRooms = 0;
                    int largeRooms = 0;
                    int massiveRooms = 0;
                    int superMassiveRooms = 0;

                    foreach (Room room in rooms)
                    {
                        Entity surface = new Entity();
                        surface.symbol = '~';
                        surface.color = new RLNET.RLColor(Game.Random.Next(0,255), Game.Random.Next(0, 255), Game.Random.Next(0, 255));
                        surface.name = $"room - {counter} + room size - {room.points.Count} ";
                    
                        counter++;
                        foreach(Point point in room.points)
                        {
                            newMap.CreateSurface(point.X, point.Y, 1, surface, SurfaceKerning.TopLeft);
                        }

                        int rpc = room.points.Count; 

                        if (rpc < tinyRoomDef)
                        {
                            tinyRooms++;
                        }
                        else if (rpc < smallRoomDef)
                        {
                            smallRooms++;
                        }
                        else if (rpc < midRoomDef)
                        {
                            midRooms++;

                        }
                        else if (rpc < largeRoomDef)
                        {
                            largeRooms++;

                        }
                        else if (rpc < massiveRoomDef)
                        {
                            massiveRooms++;

                        }
                        else
                        {
                            superMassiveRooms++;
                        }

                    }

                    Console.WriteLine($"tiny rooms ={tinyRooms} small rooms= {smallRooms} mid rooms = {midRooms} large rooms =={largeRooms} massive rooms= {massiveRooms} supermassive rooms {superMassiveRooms}");
                }



            }
            else
            {
                PlacePlayer(0, 0);
            }
            return newMap;



        }

        public SerialiseableCells[,] CopyCells(SerialiseableCells[,] copyCells, int width, int height, DungeonMap fromMap , int to_x, int to_y)
        {

            SerialiseableCells[,] copy = new SerialiseableCells[mapWidth, mapHeight];
            for (int x = 0; x <  width; x++)
            {
                for (int y =0; y <  height; y++)
                {
                    copy[x,y].walkable = fromMap.GetCell(x, y).IsWalkable;
                }
            }
       

            for (int x = to_x; x < (to_x+ width) ; x++)
            {
                for (int y =to_y; y < (to_y +height); y++)
                { 
                    if (x < mapWidth && x >= 0 && y < mapHeight && y >= 0)
                    {
                        copyCells[x,y].walkable = copy[x - to_x, y - to_y].walkable;
                    }
                }
            }


            return copyCells;

        }

        public DungeonMap MakeMapFromCells(SerialiseableCells[,] copycells)
        {
            DungeonMap copyMap = new DungeonMap();
            copyMap.Initialize(mapWidth, mapHeight);
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (y < mapWidth  && x >= 0 && y < mapHeight && y >= 0)
                    {
                        SerialiseableCells copycell = copycells[x, y];
                        copyMap.SetCellProperties(x, y, copycell.walkable, copycell.walkable);
                    }

                }
            }
            return copyMap;
        }

       

        public List<Room> GetRooms(SerialiseableCells[] runmap)
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

        private List<Point> FloodFill( Point pt, SerialiseableCells[] runmap)
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
                newMap.SetCellProperties(x, yPosition, true, true);
            }
        }

        // Carve a tunnel out of the map parallel to the y-axis
        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                newMap.SetCellProperties(xPosition, y, true, true);
            }
        }
        public void PlacePlayer(int x, int y)
        {
            newMap.AddPlayer( x, y);
        }

        private void CreateStairs()
        {
            if (mapLevel > 1)
            {

                Point stairpoint = StartRoom.GetRandomPointInRoom();

                Stairs upstairs = new Stairs
                {
           
                    IsUp = true

                };
                upstairs.transform.x = stairpoint.X;
                upstairs.transform.x = stairpoint.Y;

                newMap.Entities.Add(upstairs);
                newMap.StairsUp = upstairs;

            }

            Point endPoint = rooms[Game.Random.Next(0, rooms.Count)].GetRandomPointInRoom();

            Stairs downstairs = new Stairs
            {
               
                IsUp = false
            };
            downstairs.transform = new Components.Transform();
            downstairs.transform.x = endPoint.X;
            downstairs.transform.y = endPoint.Y;
            newMap.Entities.Add(downstairs);
            newMap.StairsDown = downstairs;

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
                            potion.transform.x = randomRoomLocation.X;
                            potion.transform.y = randomRoomLocation.Y;
                            newMap.AddItem(potion);
                        }
                        else if (roll < 50)
                        {
                            var chest = Chest.Create();
                            chest.transform.x = randomRoomLocation.X;
                            chest.transform.y = randomRoomLocation.Y;
                            newMap.AddItem(chest);
                        }
                        else if (roll < 75)
                        {
                            var potion = WaterPotion.Create();
                            potion.transform.x = randomRoomLocation.X;
                            potion.transform.y = randomRoomLocation.Y;
                            newMap.AddItem(potion);
                        }
                        else
                        {
                            var potion = SlimePotion.Create();
                            potion.transform.x = randomRoomLocation.X;
                            potion.transform.y = randomRoomLocation.Y;
                            newMap.AddItem(potion);

                        }
                        
                    }
                }

            }
        }


        private void PlaceMonsters()
        {
            foreach (var room in rooms)
            {
                
                int numberOfMonsters = 0;
                if (room.points.Count < 10)
                {
                    numberOfMonsters = 0;
                    Console.WriteLine($"room size is 10 - using {numberOfMonsters}");
                }
                else if (room.points.Count < 30)
                {
                    numberOfMonsters = Game.Random.Next(0, 5);
                    Console.WriteLine($"room size is 30 - using {numberOfMonsters}");

                }
                else if (room.points.Count < 100)
                {
                    numberOfMonsters = Game.Random.Next(3, 6);
                    Console.WriteLine($"room size is 100 - using {numberOfMonsters}");

                }
                else 
                {
                    numberOfMonsters = Game.Random.Next(6, 10);
                    Console.WriteLine($"room size is massive - using {numberOfMonsters}");

                }
                for (int i = 0; i < numberOfMonsters; i++)
                {
                    Point randomRoomLocation = room.GetRandomPointInRoom();

                    int monsterRoll = Game.Random.Next(0, 100);
                    if (monsterRoll < 70)
                    {
                        var monster = Cryofailure.Create(mapLevel);
                        monster.transform.x = randomRoomLocation.X;
                        monster.transform.y = randomRoomLocation.Y;
                        newMap.AddMonster(monster);
                    }
                    else
                    {
                        var monster = Slimehulk.Create(mapLevel);
                        monster.transform.x = randomRoomLocation.X;
                        monster.transform.y = randomRoomLocation.Y;
                        newMap.AddMonster(monster);
                    }
                        
                    
                }
            }
        }

    }
}
