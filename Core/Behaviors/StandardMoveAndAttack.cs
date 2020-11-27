using RogueSharp;
using sharpRoguelike.Core.Interfaces;
using sharpRoguelike.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sharpRoguelike.Core.Behaviors
{
    class StandardMoveAndAttack : IBehavior
    {
        int longPathWait = 0;

        public bool Act(Monster monster, CommandSystem commandSystem)
        {
            DungeonMap dungeonMap = Game.DungeonMap;
            Entity player = Game.Player;
            FieldOfView monsterFov = new FieldOfView(dungeonMap);

            if (!monster.TurnsAlerted.HasValue)
            {
                monsterFov.ComputeFov(monster.x, monster.y, monster.actor.Awareness, true);
                if (monsterFov.IsInFov(player.x, player.y))
                {
                    Game.MessageLog.Add($" The {monster.name} sees {player.name}", Colors.NormalMessage);
                    monster.TurnsAlerted = 1;
                }

            }
            else
            {
                dungeonMap.SetIsWalkable(monster.x, monster.y, true);
                dungeonMap.SetIsWalkable(player.x, player.y, true);
                PathFinder pf = new PathFinder(dungeonMap);
                Path path = null;
                try
                {
                    path = pf.ShortestPath(
                        dungeonMap.GetCell(monster.x, monster.y),
                        dungeonMap.GetCell(player.x, player.y));

                }
                catch (PathNotFoundException)
                {
                    Game.MessageLog.Add($"{monster.name} waits for a turn", Colors.NormalMessage);
                }
                dungeonMap.SetIsWalkable(monster.x, monster.y, false);
                dungeonMap.SetIsWalkable(player.x, player.y, false);
                if (path != null)
                {
                    if (path.Length > 40)
                    {
                        //this monster is going for a - fucking - walk lol 
                        //wait for a few turns 
                        longPathWait++;
                        if (longPathWait < 0)
                        {
                            Game.MessageLog.Add($"{monster.name} waits at the back.", Colors.NormalMessage);
                            return true;
                        }

                    }
                    try
                    {
                        commandSystem.MoveMonster(monster, path.StepForward());
                    }
                    catch(NoMoreStepsException)
                    {
                        Game.MessageLog.Add($"{monster.name} growls in frustration", Colors.NormalMessage);

                    }
                }
                monster.TurnsAlerted++;
                if(monster.TurnsAlerted > 15)
                {
                    monster.TurnsAlerted = null;
                }
            }
            return true;
        }
    
    }
}
