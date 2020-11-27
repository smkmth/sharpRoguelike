using RLNET;
using sharpRoguelike.Core.Behaviors;
using sharpRoguelike.Core.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    [Serializable]
    public class AI 
    {
        public Entity owner;
        public int? TurnsAlerted { get; set; }

        public virtual void PerformAction(CommandSystem commandSystem)
        {
            var behavior = new StandardMoveAndAttack();
            behavior.Act(owner, commandSystem);
        }
    }
}
