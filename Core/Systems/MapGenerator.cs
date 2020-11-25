using RogueSharp;
using RogueSharp.DiceNotation;
using sharpRoguelike.Core.Items;
using sharpRoguelike.Core.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sharpRoguelike.Core
{
    public class MapGenerator
    {
        private readonly int width;
        private readonly int height;
        private readonly int maxRooms;
        private readonly int roomsMinSize;
        private readonly int roomsMaxSize;


        private readonly DungeonMap map;

        public MapGenerator(int _width, int _height, int _maxRooms,int  _roomsMinSize, int _roomsMaxSize, int mapLevel)
        {
            width = _width;
            height = _height;
            maxRooms = _maxRooms;
            roomsMinSize = _roomsMinSize;
            roomsMaxSize = _roomsMaxSize;
            map = new DungeonMap();

        }

        public DungeonMap CreateMap()
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

        

            for (int r=1; r < map.Rooms.Count; r++)
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

            foreach (Rectangle room in map.Rooms)
            {
                CreateRoom(room);
                CreateDoors(room);

            }
            CreateStairs();
            PlacePlayer();
            PlaceMonsters();
            PlaceItems();
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
            Player player = Game.Player;
            if (player == null)
            {
                player = new Player();
            }
            player.x = map.Rooms[0].Center.X;
            player.y = map.Rooms[0].Center.Y;

            map.AddPlayer(player);
        }

        private void PlaceMonsters()
        {
            foreach(var room in map.Rooms)
            {
                if (Dice.Roll("1D10") < 7)
                {
                    var numberOfMonsters = Dice.Roll("1D4");
                    for(int i = 0; i < numberOfMonsters; i++)
                    {
                        Point randomRoomLocation = new Point(0, 0);
                        if (map.GetRandomWalkableLocationInRoom(room, out randomRoomLocation))
                        {
                            var monster = Kobold.Create(1);
                            monster.x = randomRoomLocation.X;
                            monster.y = randomRoomLocation.Y;
                            map.AddMonster(monster);
                        }
                    }
                }
            }
        }

        private void PlaceItems()
        {
            foreach(var room in map.Rooms)
            {
                if (Dice.Roll("1D10") < 7)
                {
                    var items = Dice.Roll("1D4");
                    for (int i = 0; i < items; i++)
                    {
                        Point randomRoomLocation = new Point(0, 0);
                        if (map.GetRandomWalkableLocationInRoom(room, out randomRoomLocation))
                        {
                            var potion = HealthPotion.Create();
                            potion.x = randomRoomLocation.X;
                            potion.y = randomRoomLocation.Y;
                            map.AddItem(potion);
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
                    Door door = new Door
                    {
                        x = cell.X,
                        y = cell.Y,
                        isOpen = false

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
            Stairs upstairs = new Stairs
            {
                x = map.Rooms.First().Center.X + 1,
                y = map.Rooms.First().Center.Y,
                IsUp = true
                
            };

            Stairs downstairs = new Stairs
            {
                x = map.Rooms.First().Center.X,
                y = map.Rooms.First().Center.Y -1,
                IsUp = false
            };

            map.Entities.Add(downstairs);
            map.Entities.Add(upstairs);
            map.StairsDown = downstairs;
            map.StairsUp = upstairs;

        }

    }


}
