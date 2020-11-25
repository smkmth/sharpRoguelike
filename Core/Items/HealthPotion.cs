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
            return new HealthPotion
            {
                symbol = '!',
                color = Colors.HealthPotion,
                name = "Health Potion",
                effect = new HealEffect()
            };

        }

   
    }
}
