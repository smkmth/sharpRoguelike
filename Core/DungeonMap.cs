﻿using System;
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
        public List<Entity> Items;
        public List<Entity> Surfaces;
        public Stairs StairsUp;
        public Stairs StairsDown;
        public List<Entity> Entities;
        public List<SerialiseableCells> s_cells;
        public DungeonMap()
        {
            Game.SchedulingSytem.Clear();
            Rooms = new List<Rectangle>();
            Monsters = new List<Monster>();
            Doors = new List<Door>();
            Items = new List<Entity>();
            Surfaces = new List<Entity>();
            Entities = new List<Entity>();
            s_cells = new List<SerialiseableCells>();
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
            StairsUp?.Draw(mapConsole, this);
            StairsDown.Draw(mapConsole, this);
            foreach(Entity surface in Surfaces)
            {
                surface.Draw(mapConsole, this);
            }
            foreach(Entity item in Items)
            {
                item.Draw(mapConsole, this);
            }
            foreach(Door door in Doors)
            {
                door.Draw(mapConsole, this);
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
            ComputeFov(player.x, player.y, player.actor.Awareness, true);
            foreach(Cell cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);

                }
            }

        }

        public bool SetActorPosition(Entity actor, int x, int y)
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

                Entity surface = GetSurfaceAt(x, y);
                if (surface != null && surface.surface != null)
                {
                    if (surface.surface.WalkOverSurface(actor))
                    {
                        RemoveSurface(surface);
                    }
                }

                OpenDoor(actor, x, y);
                UpdatePlayerFOV();

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
            Game.SchedulingSytem.Add(player.actor);
            Entities.Add(player);
        }

        public void AddMonster(Monster monster)
        {
            Monsters.Add(monster);
            SetIsWalkable(monster.x, monster.y, false);
            Game.SchedulingSytem.Add(monster.actor);
            Entities.Add(monster);

        }

        public void RemoveMonster(Monster monster)
        {
            Monsters.Remove(monster);
            Entities.Remove(monster);
            SetIsWalkable(monster.x, monster.y, true);
            Game.SchedulingSytem.Remove(monster.actor);

        }
        

        public Monster GetMonsterAt(int x, int y)
        {
            return Monsters.FirstOrDefault(m => m.x == x && m.y == y);
        }

        public Entity GetItemAt(int x, int y)
        {
            return Items.FirstOrDefault(i => i.x == x && i.y == y);

        }

        public void RemoveItem(Entity item)
        {
            Entities.Remove(item);
            Items.Remove(item);
        }

        public void AddItem(Entity item)
        {
            Items.Add(item);
            Entities.Add(item);
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

        private void OpenDoor(Entity actor, int x, int y)
        {
            Door door = GetDoor(x, y);
            if (door != null)
            {
                door.isOpen = true;
                var cell = GetCell(x, y);
                SetCellProperties(x, y, true, cell.IsWalkable, cell.IsExplored);
                Game.MessageLog.Add($"{actor.name} opened a door", Colors.NormalMessage);
            }
        }
        
        public bool CanMoveDownToNextLevel()
        {
            Player player = Game.Player;
            return StairsDown.x == player.x && StairsDown.y == player.y;

        }

        public List<Entity> GetAllEntitiesAt(int x, int y)
        {
            var results = Entities.Where(i => i.x == x && i.y == y);
            List<Entity> entitiesHere = new List<Entity>();           
            foreach(Entity entity in results)
            {
                entitiesHere.Add(entity);
            }
            return entitiesHere;

        }

        public List<string> InterigateEntityAtLocation(int x, int y) 
        {
            var results =  Entities.Where(i => i.x == x && i.y == y );
            List<string> names = new List<string>();
            foreach(Entity entity in results)
            {
                if (IsInFov(entity.x, entity.y))
                {
                    names.Add(entity.name);
                }
            }
            return names;

        }

        public void CreateSurface(int x, int y, int range , Entity liquid)
        {
            for (int dx = x; dx < x+ range; dx++)
            {
                for (int dy =y; dy < y + range; dy++)
                {
                    if (IsWalkable(dx, dy))
                    {
                        Entity oldSurf = GetSurfaceAt(dx, dy);
                        
                        if (oldSurf != null)
                        {
                            RemoveSurface(oldSurf);
                        }

                        Entity liquidinst = new Entity();
                        liquidinst.name = liquid.name;
                        liquidinst.color = liquid.color;
                        liquidinst.symbol = liquid.symbol;
                        liquidinst.surface = liquid.surface;
                        liquidinst.x = dx;
                        liquidinst.y = dy;
                        Surfaces.Add(liquidinst);
                        Entities.Add(liquidinst);
                    }

                }
            }
        }

        public Entity GetSurfaceAt(int x, int y)
        {
            return Surfaces.FirstOrDefault(i => i.x == x && i.y == y);

        }
        public void RemoveSurface(Entity surface)
        {
            Surfaces.Remove(surface);
            Entities.Remove(surface);
        }

        public void LoadEntities()
        {

            foreach (SerialiseableCells cell in s_cells)
            {
                ICell icell = GetCell(cell.x, cell.y);
                SetCellProperties(cell.x, cell.y, icell.IsTransparent, icell.IsWalkable, cell.explored);
            }

            Entities.AddRange(Monsters);
            foreach(Monster monster in Monsters)
            {
                Game.SchedulingSytem.Add(monster.actor);
            }
            Game.SchedulingSytem.Add(Game.Player.actor);
            Entities.AddRange(Doors);
            Entities.AddRange(Items);
            Entities.AddRange(Surfaces);
            foreach (Entity entity in Entities)
            {
                entity.LoadEntityColor();
                if (entity.corpse != null)
                {
                    entity.corpse.LoadEntityColor();

                }
            }
            Game.Player.LoadEntityColor();

       
        }

        
        public void SaveEntityColors()
        {
            foreach(Entity entity in Entities)
            {
                entity.SaveEntityColor();
                if (entity.corpse != null)
                {
                    entity.corpse.SaveEntityColor();

                }
            }

            foreach(ICell cell in GetAllCells())
            {
                s_cells.Add(new SerialiseableCells(cell.X, cell.Y, cell.IsExplored));
            }
        }
    }
    [Serializable]
    public struct SerialiseableCells
    {
        public int x;
        public int y;
        public bool explored;

        public SerialiseableCells(int x, int y, bool explored)
        {
            this.x = x;
            this.y = y;
            this.explored = explored;
        }
    }
}
