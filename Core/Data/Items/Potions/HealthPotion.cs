using sharpRoguelike.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Items
{
    [Serializable]
    public class HealthPotion : Entity
    {
        public static HealthPotion Create()
        {
            HealthPotion potion=  new HealthPotion
            {
   
                name = "Health Potion",
            };
            potion.renderer = new Renderer(potion);
            potion.renderer.symbol = '!';
            potion.renderer.color = Colors.HealthPotion;
            potion.transform = new Transform();

            potion.effect = new HealPotionUse();
            potion.effect.owner = potion;
            return potion;

        }


    }
}
