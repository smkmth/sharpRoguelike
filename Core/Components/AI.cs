using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Behaviors;
using sharpRoguelike.Core.Interfaces;
using sharpRoguelike.Core.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    //if its a thing that moves around - it needs to have this component on it - will 
    //call its perform action function when it gets told to by command system HandleTurnOrder

    [Serializable]
    public class AI 
    {
        public Entity owner;
        public int? TurnsAlerted { get; set; }
        public int lastSeenPlayerX;
        public int lastSeenPlayerY;
        public int longPathWait =20;
        public int cooldown = 150;
        public List<IBehavior> behaviorStack;
        [NonSerialized] public Path currentPath;
        public void AlertAI()
        {
            TurnsAlerted = 1;
            lastSeenPlayerX = Game.Player.transform.x;
            lastSeenPlayerY = Game.Player.transform.y;
        }

        public virtual void PerformAction(CommandSystem commandSystem)
        {
            //works like filters - false results didnt act and want new routines
            foreach(IBehavior behavior in behaviorStack)
            {
                if (behavior.Act(owner, commandSystem))
                {
                    return;
                }
            }
        }
    }
}
