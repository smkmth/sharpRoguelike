﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Components;

namespace sharpRoguelike.Core
{
    public enum SurfaceKerning
    {
        TopLeft,
        CenterAligned
    }

    //the main - API - thing in the project - derives from map, which 
    //seems to just be a collection of cells and some ways to access those cells
    //dungon map contains multiple lists of entity 'types' for ease of access more 
    //than anything, and an entity master list that holds all the entities.
    //yes this is wierd since all the things are entities- and TODO refactor this - 
    //but it works for now. 
    public class DungeonMap : Map 
    {
        public List<Entity> Doors;
        public List<Entity> Monsters;
        public List<Entity> Items;
        public List<Entity> Surfaces;
        public List<Entity> Entities;
        public Entity StairsUp;
        public Entity StairsDown;
        public List<SerialiseableCells> s_cells;
        List<Entity> monstersInFov;

        public DungeonMap()
        {
            Game.SchedulingSytem.Clear();
            Monsters = new List<Entity>();
            Doors = new List<Entity>();
            Items = new List<Entity>();
            Surfaces = new List<Entity>();
            Entities = new List<Entity>();
            s_cells = new List<SerialiseableCells>();
            monstersInFov = new List<Entity>();
        }



        new public ICell GetCell(int x, int y)
        {
            if (x >= Width || y >= Height)
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
          
            foreach(Entity surface in Surfaces)
            {
                surface.renderer.Draw(mapConsole, this);
            }
            foreach(Entity item in Items)
            {
                item.renderer.Draw(mapConsole, this);
            }
            foreach(Entity door in Doors)
            {
                door.renderer.Draw(mapConsole, this);
            }
            monstersInFov.Clear();
            foreach (Entity monster in Monsters)
            {
                monster.renderer.Draw(mapConsole, this);
                if (IsInFov(monster.transform.x, monster.transform.y))
                {
                    monstersInFov.Add(monster);
                }
            }
            Game.statDisplay.DrawMonsters(statConsole, monstersInFov);

            //stairs always at the v top 
            StairsUp?.renderer.Draw(mapConsole, this);
            StairsDown?.renderer.Draw(mapConsole, this);

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
            Entity player = Game.Player;
            ComputeFov(player.transform.x, player.transform.y, player.actor.Awareness, true);
            foreach(Cell cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);

                }
            }

        }
        public void Wallhack(bool setOn)
        {
            foreach (Cell cell in GetAllCells())
            {
                SetCellProperties(cell.X, cell.Y, setOn, cell.IsWalkable, setOn);
            }
        }
        public void SeeAllWalls(bool setOn)
        {
            foreach (Cell cell in GetAllCells())
            {
                SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, setOn);
            }
        }
        public void SeeAllEntities(bool setOn)
        {
            foreach(Entity entity in Entities)
            {
                entity.renderer.Clairvoince = setOn;
            }
        }


        public bool SetActorPosition(Entity entity, int x, int y)
        {
            if (GetCell(x, y).IsWalkable)
            {
                SetIsWalkable(entity.transform.x, entity.transform.y, true);
                entity.transform.x = x;
                entity.transform.y = y;
                entity.OnMove(entity.transform.x, entity.transform.y, x, y);

                SetIsWalkable(entity.transform.x, entity.transform.y, false);
                if (entity.player != null)
                {
                    UpdatePlayerFOV();
                }

                Entity surface = GetSurfaceAt(x, y);
                if (surface != null && surface.surface != null)
                {
                    if (surface.surface.WalkOverSurface(entity))
                    {
                        RemoveSurface(surface);
                    }
                }

                OpenDoor(entity, x, y);
                UpdatePlayerFOV();

                return true;

            }
            return false;

        }

        public void SetIsWalkable(int x, int y, bool walkable)
        {
            if (GetCell(x,y) != null)
            {

                SetCellProperties(x, y, GetCell(x,y).IsTransparent, walkable, GetCell(x,y).IsExplored);
            }

        }

        public void AddPlayer(int x, int y)
        {
            if (Game.Player == null)
            {
                Game.Player = new Entity();
            }
            Entity player = Game.Player;
            player.player = new Player(player);
            player.player.ResetPlayer();
            player.transform.x = x;
            player.transform.y = y;
            SetIsWalkable(player.transform.x, player.transform.y, false);
            UpdatePlayerFOV();
            Game.SchedulingSytem.Add(player.actor);
            Entities.Add(player);
        }

        public void AddMonster(Entity monster)
        {
            Monsters.Add(monster);
            SetIsWalkable(monster.transform.x, monster.transform.y, false);
            Game.SchedulingSytem.Add(monster.actor);
            Entities.Add(monster);

        }

        public void RemoveMonster(Entity monster)
        {
            Monsters.Remove(monster);
            Entities.Remove(monster);
            SetIsWalkable(monster.transform.x, monster.transform.y, true);
            Game.SchedulingSytem.Remove(monster.actor);

        }
        

        public Entity GetMonsterAt(int x, int y)
        {
            return Monsters.FirstOrDefault(m => m.transform.x == x && m.transform.y == y);
        }

        public Entity GetItemAt(int x, int y)
        {
            return Items.FirstOrDefault(i => i.transform.x == x && i.transform.y == y);

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
        
        public Entity GetDoor(int x, int y)
        {
            return Doors.SingleOrDefault(d => d.transform.x == x && d.transform.y == y);
        }

        private void OpenDoor(Entity actor, int x, int y)
        {
            Entity door = GetDoor(x, y);
            if (door != null)
            {
 
                var cell = GetCell(x, y);
                SetCellProperties(x, y, true, cell.IsWalkable, cell.IsExplored);
                Game.MessageLog.Add($"{actor.name} opened a door", Colors.NormalMessage);
            }
        }
        
        public bool CanMoveDownToNextLevel()
        {
            Entity player = Game.Player;
            return StairsDown.transform.x == player.transform.x && StairsDown.transform.y == player.transform.y;

        }

        public List<Entity> GetAllEntitiesAt(int x, int y)
        {
            var results = Entities.Where(i => i.transform.x == x && i.transform.y == y);
            List<Entity> entitiesHere = new List<Entity>();           
            foreach(Entity entity in results)
            {
                entitiesHere.Add(entity);
            }
            return entitiesHere;

        }

        public List<Entity> InterigateEntityAtLocation(int x, int y) 
        {
            var results =  Entities.Where(i => i.transform.x == x && i.transform.y == y );
            List<Entity> names = new List<Entity>();
            foreach(Entity entity in results)
            {
                if (IsInFov(entity.transform.x, entity.transform.y) || Game.DebugCheats)
                {
                    names.Add(entity);
                }
             
            }
            return names;

        }

        public void CreateSurface(int x, int y, int range , string name, RLColor color, char symbol, Surface surf , SurfaceKerning kerning = SurfaceKerning.CenterAligned )
        {

            switch (kerning)
            {
                case SurfaceKerning.TopLeft:
                    for (int dx = x; dx < x + range; dx++)
                    {
                        for (int dy = y; dy < y + range; dy++)
                        {
                            if (IsWalkable(dx, dy))
                            {
                                Entity oldSurf = GetSurfaceAt(dx, dy);

                                if (oldSurf != null)
                                {
                                    if (oldSurf.surface != null)
                                    {

                                        if (surf.surfaceResistance < oldSurf.surface.surfaceResistance)
                                        {
                                            oldSurf.surface.surfaceResistance -= surf.surfaceResistance;
                                            break;
                                        }
                                        else
                                        {
                                            RemoveSurface(oldSurf);
                                        }
                                    }
                                }

                                Entity liquidinst = new Entity();
                                liquidinst.name = name;
                                liquidinst.renderer = new Renderer(liquidinst);
                                liquidinst.renderer.color = color;
                                liquidinst.renderer.symbol= symbol;
                                liquidinst.surface = surf;

                                liquidinst.transform = new Transform();
                                liquidinst.transform.x = dx;
                                liquidinst.transform.y = dy;
                                Surfaces.Add(liquidinst);
                                Entities.Add(liquidinst);
                            }

                        }
                    }
                    break;
                case SurfaceKerning.CenterAligned:
                    for (int dx = x - ( range / 2); dx < x + (range /2 ); dx++)
                    {
                        for (int dy = y - (range / 2); dy < y + (range /2); dy++)
                        {
                            if (IsWalkable(dx, dy))
                            {
                                Entity oldSurf = GetSurfaceAt(dx, dy);

                                if (oldSurf != null)
                                {
                                    if (oldSurf.surface != null)
                                    {

                                        if (surf.surfaceResistance < oldSurf.surface.surfaceResistance)
                                        {
                                            oldSurf.surface.surfaceResistance -= surf.surfaceResistance;
                                            break;
                                        }
                                        else
                                        {
                                            RemoveSurface(oldSurf);
                                        }
                                    }
                                }
                                Entity liquidinst = new Entity();
                                liquidinst.name = name;
                                liquidinst.renderer = new Renderer(liquidinst);
                                liquidinst.renderer.color = color;
                                liquidinst.renderer.symbol = symbol;
                                liquidinst.surface = surf;

                                liquidinst.transform = new Transform();
                                liquidinst.transform.x = dx;
                                liquidinst.transform.y = dy;
                                Surfaces.Add(liquidinst);
                                Entities.Add(liquidinst);
                            }

                        }
                    }
                    break;
            }
            
        }

        public Entity GetSurfaceAt(int x, int y)
        {
            return Surfaces.FirstOrDefault(i => i.transform.x == x && i.transform.y == y);

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
                SetCellProperties(cell.x, cell.y, cell.transparent, cell.walkable, cell.explored);
            }
           
            Entities.AddRange(Monsters);
            foreach(Entity monster in Monsters)
            {
                Game.SchedulingSytem.Add(monster.actor);
            }
            Game.SchedulingSytem.Add(Game.Player.actor);
            Entities.AddRange(Doors);
            Entities.AddRange(Items);
            Entities.AddRange(Surfaces);
            foreach (Entity entity in Entities)
            {
                entity.renderer.LoadEntityColor();
                if (entity.corpse != null)
                {
                    entity.corpse.renderer.LoadEntityColor();

                }
            }
            Game.Player.renderer.LoadEntityColor();

       
        }

        
        public void SaveEntityColors()
        {
            foreach(Entity entity in Entities)
            {
                entity.renderer.SaveEntityColor();
                if (entity.corpse != null)
                {
                    entity.corpse.renderer.SaveEntityColor();

                }
            }
            s_cells.Clear();
            foreach(ICell cell in GetAllCells())
            {
                s_cells.Add(new SerialiseableCells(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, cell.IsExplored));
            }
        }

        public int GetDistance(Entity entiyA, Entity entiyB)
        {

            double a = (double)(entiyA.transform.x - entiyB.transform.x);
            double b = (double)(entiyA.transform.y - entiyB.transform.y);

            return (int)(Math.Sqrt(a * a + b * b));

        }

        public int AlertAllInRange(int x, int y, int range)
        {
            int val = 0;

            for (int dx = x - (range / 2); dx < x + (range / 2); dx++)
            {
                for (int dy = y - (range / 2); dy < y + (range / 2); dy++)
                {
                    var entities = GetAllEntitiesAt(dx, dy);
                    foreach(Entity entity in entities)
                    {
                        if (entity.ai != null)
                        {
                            entity.ai.AlertAI();
                            Console.WriteLine("alerted " + entity.name);
                            val++;
                        }
                    }

                }
            }
            return val;
        }
    }

    //another serialisation class - exists so i can set all the tiles i have seen back to explored
    [Serializable]
    public struct SerialiseableCells
    {
        public int x;
        public int y;
        public bool explored;
        public bool transparent;
        public bool walkable;

        public SerialiseableCells(int x, int y, bool transparent,bool walkable, bool explored)
        {
            this.x = x;
            this.y = y;
            this.explored = explored;
            this.transparent = transparent;
            this.walkable = walkable;
        }
    }
}
