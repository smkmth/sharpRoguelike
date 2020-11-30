using RLNET;
using sharpRoguelike.Core.Behaviors;
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

        public void AlertAI()
        {
            TurnsAlerted = 1;

        }

        public virtual void PerformAction(CommandSystem commandSystem)
        {
            var behavior = new StandardMoveAndAttack();
            behavior.Act(owner, commandSystem);
        }
    }
}
