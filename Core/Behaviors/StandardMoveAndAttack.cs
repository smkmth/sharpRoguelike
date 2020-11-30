﻿using RogueSharp;
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
                monsterFov.ComputeFov(monster.x, monster.y, monster.actor.Awareness, true);
                if (monsterFov.IsInFov(player.x, player.y))
                {
                    monster.ai.lastSeenPlayerX = player.x;
                    monster.ai.lastSeenPlayerY = player.y;
                    Game.MessageLog.Add($" The {monster.name} sees {player.name}", Colors.NormalMessage);
                    monster.ai.TurnsAlerted = 1;
                }

            }
            else
            {
                monsterFov.ComputeFov(monster.x, monster.y, monster.actor.Awareness, true);
                if (monsterFov.IsInFov(player.x, player.y))
                {
                    monster.ai.lastSeenPlayerX = player.x;
                    monster.ai.lastSeenPlayerY = player.y;
                    PathToSpot(monster, commandSystem, player.x, player.y);
                    return true;
                }
                else
                {
                    Console.WriteLine($"{monster.name} lost player - heading to last location");
                    if (PathToSpot(monster, commandSystem, monster.ai.lastSeenPlayerX, monster.ai.lastSeenPlayerY))
                    {
                        Console.WriteLine($"{monster.name} reached last location");
                        return false;

                    }

                }
                //after monster has moved - check monster still alive
                if (monster.attacker != null)
                {
                    monster.ai.TurnsAlerted++;
                    if(monster.ai.TurnsAlerted > monster.ai.cooldown)
                    {
                        Console.WriteLine($"{monster.name} lost player - forgetting about player");
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

        public static bool PathToSpot(Entity monster, CommandSystem commandSystem, int x, int y)
        {
            DungeonMap dungeonMap = Game.DungeonMap;
            bool wasWalkable = true;

            dungeonMap.SetIsWalkable(monster.x, monster.y, true);
            wasWalkable = dungeonMap.IsWalkable(x, y);
            dungeonMap.SetIsWalkable(x, y, true);
            PathFinder pf = new PathFinder(dungeonMap);
            Path path = null;
            try
            {
                path = pf.ShortestPath(
                    dungeonMap.GetCell(monster.x, monster.y),
                    dungeonMap.GetCell(x, y));

            }
            catch (PathNotFoundException)
            {
                Game.MessageLog.Add($"{monster.name} waits for a turn", Colors.NormalMessage);
            }
            dungeonMap.SetIsWalkable(monster.x, monster.y, false);
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

    }

  
}
