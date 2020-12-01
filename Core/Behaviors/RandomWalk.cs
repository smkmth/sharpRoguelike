using sharpRoguelike.Core.Interfaces;
using sharpRoguelike.Core.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Behaviors
{
    class RandomWalk : IBehavior
    {
        public bool Act(Entity monster, CommandSystem commandSystem)
        {
            Console.WriteLine("goin random walk");
            return StandardMoveAndAttack.PathToSpot(monster, commandSystem, Game.Player.transform.x, Game.Player.transform.y);
         
        }
    }
}
