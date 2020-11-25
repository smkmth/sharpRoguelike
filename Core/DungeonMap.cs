using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RLNET;
using RogueSharp;

namespace sharpRoguelike.Core
{
    public class DungeonMap : Map 
    {
        public List<Rectangle> Rooms;
        public List<Monster> Monsters;
        public List<Door> Doors;
        public Stairs StairsUp;
        public Stairs StairsDown;

        public DungeonMap()
        {
            Game.SchedulingSytem.Clear();

            Rooms = new List<Rectangle>();
            Monsters = new List<Monster>();
            Doors = new List<Door>();
        }

        new public ICell GetCell(int x, int y)
        {
            if (x > Width || y > Height)
            {
                return null;

            }
            else if (x < 0 || y < 0  )
            {

                return null;

            }
            else
            {
                
                return base.GetCell(x, y);
            }
        
           
        }

        public void Draw(RLConsole mapConsole, RLConsole statConsole )
        {
            mapConsole.Clear();
            foreach(Cell cell in GetAllCells())
            {

                SetConsoleSymbolForCell(mapConsole, cell);
            }
            int i = 0;
            foreach(Monster monster in Monsters)
            {
                monster.Draw(mapConsole, this);
                if (IsInFov(monster.x, monster.y))
                {
                    monster.DrawStats(statConsole, i);
                    i++;
                }
            }

            foreach(Door door in Doors)
            {
                door.Draw(mapConsole, this);
            }
            StairsUp.Draw(mapConsole, this);
            StairsDown.Draw(mapConsole, this);
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
            ComputeFov(player.x, player.y, player.Awareness, true);
            foreach(Cell cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);

                }
            }

        }

        public bool SetActorPosition(Actor actor, int x, int y)
        {
            if (GetCell(x, y).IsWalkable)
            {
                SetIsWalkable(actor.x, actor.y, true);
                actor.x = x;
                actor.y = y;

                SetIsWalkable(actor.x, actor.y, false);
                if (actor is Player)
                {
                    UpdatePlayerFOV();
                }
                OpenDoor(actor, x, y);
                return true;

            }
            return false;

        }

        public void SetIsWalkable(int x, int y, bool walkable)
        {
            SetCellProperties(x, y, GetCell(x,y).IsTransparent, walkable, GetCell(x,y).IsExplored);

        }

        public void AddPlayer(Player player)
        {
            Game.Player = player;
            SetIsWalkable(player.x, player.y, false);
            UpdatePlayerFOV();
            Game.SchedulingSytem.Add(player);
        }

        public void AddMonster(Monster monster)
        {
            Monsters.Add(monster);
            SetIsWalkable(monster.x, monster.y, false);
            Game.SchedulingSytem.Add(monster);

        }

        public void RemoveMonster(Monster monster)
        {
            Monsters.Remove(monster);
            SetIsWalkable(monster.x, monster.y, true);
            Game.SchedulingSytem.Remove(monster);

        }

        public Monster GetMonsterAt(int x, int y)
        {
            return Monsters.FirstOrDefault(m => m.x == x && m.y == y);
        }

        public bool GetRandomWalkableLocationInRoom(Rectangle room, out Point point)
        {
            if (DoesRoomHaveWalkableSpace(room))
            {
                for (int i = 0; i < 100; i++)
                {
                    int x = Game.Random.Next(1, room.Width - 2) + room.X;
                    int y = Game.Random.Next(1, room.Height - 2) + room.Y;
                    if (IsWalkable(x, y))
                    {
                        point = new Point(x, y);
                        return true;
                    }
                }
            }
            point = new Point(0, 0);
            return false;
            
        }
        // Iterate through each Cell in the room and return true if any are walkable
        public bool DoesRoomHaveWalkableSpace(Rectangle room)
        {
            for (int x = 1; x <= room.Width - 2; x++)
            {
                for (int y = 1; y <= room.Height - 2; y++)
                {
                    if (IsWalkable(x + room.X, y + room.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Door GetDoor(int x, int y)
        {
            return Doors.SingleOrDefault(d => d.x == x && d.y == y);
        }

        private void OpenDoor(Actor actor, int x, int y)
        {
            Door door = GetDoor(x, y);
            if (door != null)
            {
                door.isOpen = true;
                var cell = GetCell(x, y);
                SetCellProperties(x, y, true, cell.IsWalkable, cell.IsExplored);
                Game.MessageLog.Add($"{actor.Name} opened a door");
            }
        }
        
        public bool CanMoveDownToNextLevel()
        {
            Player player = Game.Player;
            return StairsDown.x == player.x && StairsDown.y == player.y;

        }
    }
}
