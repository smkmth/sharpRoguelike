using sharpRoguelike.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Items
{
    public class HealthPotion : Entity
    {
        public static HealthPotion Create()
        {
            HealthPotion potion=  new HealthPotion
            {
                symbol = '!',
                color = Colors.HealthPotion,
                name = "Health Potion",
            };
            potion.effects = new List<Useable>();
            potion.effects.Add(new HealEffect());
            return potion;

        }


    }
}
