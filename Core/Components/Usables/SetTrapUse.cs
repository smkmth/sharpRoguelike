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


                TrapSurface surface = new TrapSurface(user);
                Game.DungeonMap.CreateSurface(user.transform.x, user.transform.y, 4, "Bear Trap", Colors.BearTrap, '^', surface);
                return true;
            }
            return base.Use(usageChar, user, target);
        }
    }
}
