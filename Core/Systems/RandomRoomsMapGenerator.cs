using RogueSharp;
using RogueSharp.MapCreation;
using sharpRoguelike.Core.Data.Items;
using sharpRoguelike.Core.Data.Monsters;
using sharpRoguelike.Core.Items;
using sharpRoguelike.Core.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sharpRoguelike.Core
{
    //responsible for placing all the entities and creating the rooms - builds 
    //a 'map' and returns in the CreateMap function
    public class RandomRoomsMapGenerator
    {
        private readonly int width;
        private readonly int height;
        private readonly int maxRooms;
        private readonly int roomsMinSize;
        private readonly int roomsMaxSize;
        private readonly int mapLevel;

        private DungeonMap map;

        public RandomRoomsMapGenerator(int _width, int _height, int _maxRooms,int  _roomsMinSize, int _roomsMaxSize, int _mapLevel)
        {
            width = _width;
            height = _height;
            maxRooms = _maxRooms;
            roomsMinSize = _roomsMinSize;
            roomsMaxSize = _roomsMaxSize;
            mapLevel = _mapLevel;
            map = new DungeonMap();

        }

        public DungeonMap CreateMap(bool addEntities)
        {
            map.Initialize(width, height);
          
            
            
            for (int r = maxRooms; r > 0; r--)
            {
                int roomWidth = Game.Random.Next(roomsMinSize, roomsMaxSize);
                int roomHeight = Game.Random.Next(roomsMinSize, roomsMaxSize);
                int roomXPosition = Game.Random.Next(0, width - roomWidth - 1);
                int roomYPosition = Game.Random.Next(0, height - roomHeight - 1);


                var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);

                bool newRoomIntersects = map.Rooms.Any(room => newRoom.Intersects(room));

                if (!newRoomIntersects)
                {
                    map.Rooms.Add(newRoom);
                }
            }

        

            for (int r=0; r < map.Rooms.Count; r++)
            {

                CreateRoom(map.Rooms[r]);
            
                if (r > 0)
                {
                    int previousRoomCenterX = map.Rooms[r - 1].Center.X;
                    int previousRoomCenterY = map.Rooms[r - 1].Center.Y;
                    int currentRoomCenterX =  map.Rooms[r].Center.X;
                    int currentRoomCenterY =  map.Rooms[r].Center.Y;

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


            //redo borders
            foreach(ICell cell in map.GetCellsInColumns( width -1))
            {
                map.SetCellProperties(cell.X, cell.Y, false, false);
            }
            foreach (ICell cell in map.GetCellsInRows( height -1))
            {
                map.SetCellProperties(cell.X, cell.Y, false, false);
            }

            foreach (Rectangle room in map.Rooms)
            {
                if (addEntities)
                {
                    CreateDoors(room);
                }

            }

            if (addEntities)
            {

                CreateStairs();
                PlacePlayer();
                PlaceMonsters();
                PlaceItems();
            }
            
            return map;
        }

        private void CreateRoom(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
            {
                for (int y = room.Top + 1; y < room.Bottom; y++)
                {
                    map.SetCellProperties(x, y, true, true, false);
                }
            }


            bool secondRect = true;

            if (secondRect)
            {
                ApplySecondRect(room);
            }

          
        }

     
     
        public void ApplySecondRect(Rectangle room)
        {
            int roomWidth = Game.Random.Next(2, room.Width);
            int roomHeight = Game.Random.Next(2, room.Height);
            int roomXPosition = Game.Random.Next(room.Left, room.Right);
            int roomYPosition = Game.Random.Next(room.Top, room.Bottom);
            if (roomXPosition + roomWidth >= width)
            {
                int toLose = (roomXPosition + roomWidth) - width;
                if ((roomXPosition - toLose) > room.Left)
                {
                    roomXPosition -= toLose;
                }
                else
                {
                    roomWidth -= toLose;
                }
            }

            if (roomYPosition + roomHeight >= height)
            {
                int toLose = (roomYPosition + roomHeight) - height;
                if ((roomYPosition - toLose) > room.Bottom)
                {
                    roomXPosition -= toLose;
                }
                else
                {
                    roomHeight -= toLose;
                }
            }

            for (int x = roomXPosition; x < roomXPosition + roomWidth; x++)
            {
                for (int y = roomYPosition; y < roomYPosition + roomHeight; y++)
                {
                    map.SetCellProperties(x, y, true, true, false);
                }
            }
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

        private void PlacePlayer()
        {
            Entity player = Game.Player;
            if (player == null)
            {
                player = new Entity();
            }
            player.x = map.Rooms[0].Center.X;
            player.y = map.Rooms[0].Center.Y;

            map.AddPlayer(player);
        }

        private void PlaceMonsters()
        {
            foreach(var room in map.Rooms)
            {
                if (Game.Random.Next(0,10) < 7)
                {
                    var numberOfMonsters = Game.Random.Next(1,4);
                    for(int i = 0; i < numberOfMonsters; i++)
                    {
                        Point randomRoomLocation = new Point(0, 0);
                        if (map.GetRandomWalkableLocationInRoom(room, out randomRoomLocation))
                        {
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

        private void PlaceItems()
        {
            foreach(var room in map.Rooms)
            {
                if (Game.Random.Next(0,10) < 4)
                {
                    var items = Game.Random.Next(1,2);
                    for (int i = 0; i < items; i++)
                    {
                        Point randomRoomLocation = new Point(0, 0);
                        if (map.GetRandomWalkableLocationInRoom(room, out randomRoomLocation))
                        {
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
        }

        private void CreateDoors(Rectangle room)
        {
            int xMin = room.Left;
            int xMax = room.Right;
            int yMin = room.Top;
            int yMax = room.Bottom;
            List<ICell> borderCells = map.GetCellsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(map.GetCellsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(map.GetCellsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(map.GetCellsAlongLine(xMax, yMin, xMax, yMax));

            foreach(Cell cell in borderCells)
            {
                if (IsPotentialDoor(cell))
                {
                    map.SetCellProperties(cell.X, cell.Y, false, true);
                    bool doorOpen = false;
                    if (Game.Random.Next(0, 100) > 75)
                    {
                        doorOpen = true;
                    }
                    Door door = new Door
                    {
                        x = cell.X,
                        y = cell.Y,
                        isOpen = doorOpen

                    };
                    map.Doors.Add(door);
                    map.Entities.Add(door);

                }
            }
        }

        private bool IsPotentialDoor(ICell cell)
        {
            if (!cell.IsWalkable)
            {
                return false;
            }
            // Store references to all of the neighboring cells 
            ICell right = map.GetCell(cell.X + 1, cell.Y);          
            ICell left = map.GetCell(cell.X - 1, cell.Y);
            ICell top = map.GetCell(cell.X, cell.Y - 1);
            ICell bottom = map.GetCell(cell.X, cell.Y + 1);



            if (map.GetDoor(cell.X, cell.Y) != null ||
                    map.GetDoor(right.X, right.Y) != null ||
                    map.GetDoor(left.X, left.Y) != null ||
                    map.GetDoor(top.X, top.Y) != null ||
                    map.GetDoor(bottom.X, bottom.Y) != null)
            {
                return false;
            }

            if (right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable)
            {
               
                return true;
            }

            if (!right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable)
            {
                return true;
            }

            return false;
        }

        private void CreateStairs()
        {
            if (mapLevel > 1)
            {

                Stairs upstairs = new Stairs
                {
                    x = map.Rooms.First().Center.X + 1,
                    y = map.Rooms.First().Center.Y,
                    IsUp = true
                
                };
                map.Entities.Add(upstairs);
                map.StairsUp = upstairs;

            }
            Stairs downstairs = new Stairs
            {
                x = map.Rooms.Last().Center.X,
                y = map.Rooms.Last().Center.Y -1,
                IsUp = false
            };

            map.Entities.Add(downstairs);
            map.StairsDown = downstairs;

        }

    }


}
