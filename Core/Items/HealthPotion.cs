using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Items
{
    public class HealthPotion : Item
    {
        public static HealthPotion Create()
        {
            return new HealthPotion
            {
                symbol = '!',
                color = Colors.HealthPotion,
                name = "Health Potion"
            };

        }
    }
}
