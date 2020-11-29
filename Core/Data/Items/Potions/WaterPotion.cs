using sharpRoguelike.Core.Components.Usables;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Data.Items
{
    [Serializable]
    class WaterPotion : Entity
    {
        public static WaterPotion Create()
        {
            WaterPotion potion = new WaterPotion
            {
                symbol = '!',
                color = Colors.ShallowWater,
                name = "Water Potion",
            };
            potion.effect = new WaterPotionEffect();
            potion.effect.owner = potion;
            return potion;

        }
    }
}
