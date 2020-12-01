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

                name = "Slime Potion",
            };
            potion.renderer = new Components.Renderer(potion);

            potion.renderer.symbol = '!';
            potion.renderer.color = Colors.SlimehulkColor;
            potion.transform = new Components.Transform();
            potion.effect = new SlimePotionUse();
            potion.effect.owner = potion;
            return potion;

        }
    }
}
