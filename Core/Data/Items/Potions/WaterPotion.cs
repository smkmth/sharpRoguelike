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

                name = "Water Potion",
            };
            potion.renderer = new Components.Renderer(potion);
            potion.renderer.symbol = '!';
            potion.renderer.color = Colors.ShallowWater;

            potion.transform = new Components.Transform();
            potion.effect = new WaterPotionUse();
            potion.effect.owner = potion;
            return potion;

        }
    }
}
