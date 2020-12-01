using RogueSharp;
using sharpRoguelike.Core.Interfaces;
using sharpRoguelike.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sharpRoguelike.Core.Behaviors
{
    [Serializable]
    class StandardMoveAndAttack : IBehavior
    {
        int longPathWait = 0;

        public bool Act(Entity monster, CommandSystem commandSystem)
        {
            DungeonMap dungeonMap = Game.DungeonMap;
            Entity player = Game.Player;
            FieldOfView monsterFov = new FieldOfView(dungeonMap);

            if (!monster.ai.TurnsAlerted.HasValue)  //monster hasnst seen us
            {
                monsterFov.ComputeFov(monster.transform.x, monster.transform.y, monster.actor.Awareness, true);
                if (monsterFov.IsInFov(player.transform.x, player.transform.y))
                {
                    monster.ai.lastSeenPlayerX = player.transform.x;
                    monster.ai.lastSeenPlayerY = player.transform.y;
                    Game.MessageLog.Add($" The {monster.name} sees {player.name}", Colors.NormalMessage);
                    monster.ai.TurnsAlerted = 1;
                }

            }
            else
            {
                monsterFov.ComputeFov(monster.transform.x, monster.transform.y, monster.actor.Awareness, true);
                if (monsterFov.IsInFov(player.transform.x, player.transform.y))
                {
                    monster.ai.lastSeenPlayerX = player.transform.x;
                    monster.ai.lastSeenPlayerY = player.transform.y;
                    PathToSpot(monster, commandSystem, player.transform.x, player.transform.y);
                    return true;
                }
                else
                {
                    if (monster.ai.currentPath == null)
                    {

                        monster.ai.currentPath = StaticPathToSpot(monster, commandSystem, monster.ai.lastSeenPlayerX, monster.ai.lastSeenPlayerY);
                        if (monster.ai.currentPath == null)
                        {
                            Console.WriteLine($"{monster.name} {monster.transform.x} {monster.transform.y} couldnt find a path to {monster.ai.lastSeenPlayerX} {monster.ai.lastSeenPlayerY}");
                        }
                        return true;
                    }
                   
                    if (MoveAlongPath(monster, commandSystem, monster.ai.currentPath))
                    {
                        monster.ai.currentPath = null;
                        
                    }
                    
                   

                }
                //after monster has moved - check monster still alive
                if (monster.attacker != null)
                {
                    monster.ai.TurnsAlerted++;
                    if(monster.ai.TurnsAlerted > monster.ai.cooldown)
                    {
                        monster.ai.TurnsAlerted = null;
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool MoveAlongPath(Entity monster, CommandSystem commandSystem, Path path)
        {
            try
            {
                
                commandSystem.MoveMonster(monster, path.StepForward());
                if (path.Length > 0)
                {
                    return false;

                }
                else
                {
                    return true;
                }
            }
            catch (NoMoreStepsException)
            {
                return true;


            }
        }

        public static bool PathToSpot(Entity monster, CommandSystem commandSystem, int x, int y)
        {
            DungeonMap dungeonMap = Game.DungeonMap;
            bool wasWalkable = true;

            dungeonMap.SetIsWalkable(monster.transform.x, monster.transform.y, true);
            wasWalkable = dungeonMap.IsWalkable(x, y);
            dungeonMap.SetIsWalkable(x, y, true);
            PathFinder pf = new PathFinder(dungeonMap);
            Path path = null;
            try
            {
                path = pf.ShortestPath(
                    dungeonMap.GetCell(monster.transform.x, monster.transform.y),
                    dungeonMap.GetCell(x, y));

            }
            catch (PathNotFoundException)
            {
                return false;
               // Game.MessageLog.Add($"{monster.name} waits for a turn", Colors.NormalMessage);
            }
            dungeonMap.SetIsWalkable(monster.transform.x, monster.transform.y, false);
            dungeonMap.SetIsWalkable(x, y, wasWalkable);
            if (path != null)
            {
                if (path.Length > 40)
                {
                    //this monster is going for a - fucking - walk lol 
                    //wait for a few turns 
                    monster.ai.longPathWait++;
                    if (monster.ai.longPathWait < 20)
                    {
                        Game.MessageLog.Add($"{monster.name} waits at the back.", Colors.NormalMessage);
                        return false;
                    }
                    

                }
                try
                {
                    commandSystem.MoveMonster(monster, path.StepForward());
                    if (path.Length > 0)
                    {
                        return false;

                    }
                    else
                    {
                        return true;
                    }
                }
                catch (NoMoreStepsException)
                {
                    return true;


                }
            }
            return false;
        }


        public static Path StaticPathToSpot(Entity monster, CommandSystem commandSystem, int x, int y)
        {
            DungeonMap dungeonMap = Game.DungeonMap;
            dungeonMap.SetIsWalkable(monster.transform.x, monster.transform.y, true);
            dungeonMap.SetIsWalkable(x, y, true);
            PathFinder pf = new PathFinder(dungeonMap);
            Path path = null;
            try
            {
                path = pf.ShortestPath(
                    dungeonMap.GetCell(monster.transform.x, monster.transform.y),
                    dungeonMap.GetCell(x, y));
                return path;
            }
            catch (PathNotFoundException)
            {
                return null;
                // Game.MessageLog.Add($"{monster.name} waits for a turn", Colors.NormalMessage);
            }

            return null;
        }

    }


}
