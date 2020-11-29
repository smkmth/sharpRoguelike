using sharpRoguelike.Core.Components.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    [Serializable]
    class SetTrapUse : Useable
    {
        public SetTrapUse()
        {
            usageChars = new List<char>();
            usageChars.Add('A');
            displayChars = new List<char>();
            displayChars.Add('a');
            usageNames = new List<string>();
            usageNames.Add("Set Trap");

        }

        public override bool Use(char usageChar, Entity user, Entity target)
        {
            if (usageChar == 'A')
            {
                Game.MessageLog.Add($"{user.name} pulls the spring back on a deadly trap", Colors.NormalMessage);

                Entity trap = new Entity();
                trap.name = "BearTrap";
                trap.color = Colors.BearTrap;
                trap.symbol = '^';
                trap.surface = new TrapSurface(user);

                Game.DungeonMap.CreateSurface(user.x, user.y, 4, trap);
                return true;
            }
            return base.Use(usageChar, user, target);
        }
    }
}
