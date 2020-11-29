using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.Usables
{
    class RangedEquipUse : EquipmentUse
    {
        public RangedEquipUse()
        {
            usageChars = new List<char>();
            usageChars.Add('B');
            displayChars = new List<char>();
            displayChars.Add('b');
            usageNames = new List<string>();
            usageNames.Add("Load");

        }

        public override bool Use(char usageChar, Entity user, Entity target)
        {
            if (usageChar == 'B')
            {
                
            }
            return base.Use(usageChar, user, target);
        }
    }
}
