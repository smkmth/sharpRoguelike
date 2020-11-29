using sharpRoguelike.Core.Components.Usables;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Data.Items
{
    [Serializable]
    class SlimePotion : Entity
    {
        public static SlimePotion Create()
        {
            SlimePotion potion = new SlimePotion
            {
                symbol = '!',
                color = Colors.SlimehulkColor,
                name = "Slime Potion",
            };
            potion.effect = new SlimePotionEffect();
            potion.effect.owner = potion;
            return potion;

        }
    }
}
