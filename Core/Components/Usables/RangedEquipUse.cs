using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.Usables
{
    [Serializable]
    class RangedEquipUse : EquipmentUse
    {
        RifleEffect rEffect;
        public RangedEquipUse(RifleEffect _rEffect)
        {
            rEffect = _rEffect;
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
                rEffect.Load();
                return true;
            }
            return true;

        }
    }
}
