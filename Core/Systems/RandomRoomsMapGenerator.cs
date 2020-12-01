using RogueSharp;
using RogueSharp.MapCreation;
using sharpRoguelike.Core.Data.Items;
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
        public List<Rectangle> Rooms;

        public RandomRoomsMapGenerator(int _width, int _height, int _maxRooms,int  _roomsMinSize, int _roomsMaxSize, int _mapLevel)
        {
            width = _width;
            height = _height;
            maxRooms = _maxRooms;
            roomsMinSize = _roomsMinSize;
            roomsMaxSize = _roomsMaxSize;
            mapLevel = _mapLevel;
            map = new DungeonMap();
            Rooms = new List<Rectangle>();

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

                bool newRoomIntersects = Rooms.Any(room => newRoom.Intersects(room));

                if (!newRoomIntersects)
                {
                    Rooms.Add(newRoom);
                }
            }

        

            for (int r=0; r < Rooms.Count; r++)
            {

                CreateRoom(Rooms[r]);
            
                if (r > 0)
                {
                    int previousRoomCenterX = Rooms[r - 1].Center.X;
                    int previousRoomCenterY = Rooms[r - 1].Center.Y;
                    int currentRoomCenterX =  Rooms[r].Center.X;
                    int currentRoomCenterY =  Rooms[r].Center.Y;

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

           

            if (addEntities)
            {

                PlacePlayer();
  
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

            //map.AddPlayer( map.Rooms[0].Center.X, map.Rooms[0].Center.Y);
        }

    
     



    }


}
