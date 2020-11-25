using sharpRoguelike.Core.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Interfaces
{
    interface IBehavior
    {
        bool Act(Monster monster, CommandSystem commandSystem);
    }
}
