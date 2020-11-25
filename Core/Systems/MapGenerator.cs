using RogueSharp;
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

        public MapGenerator(int _width, int _height, int _maxRooms,int  _roomsMinSize, int _roomsMaxSize)
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

            foreach(Rectangle room in map.Rooms)
            {
                CreateRoom(room);
            }

            for(int r=1; r < map.Rooms.Count; r++)
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

            PlacePlayer();
            return map;
        }

        private void CreateRoom(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
            {
                for (int y = room.Top + 1; y < room.Bottom; y++)
                {
                    map.SetCellProperties(x, y, true, true, true);
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

    }
}
