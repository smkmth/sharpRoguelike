using sharpRoguelike.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Data.Items
{
    [Serializable]
    class Chest : Entity
    {
        public static Chest Create()
        {
            Chest chest = new Chest
            {
                symbol = '&',
                color = Colors.HighTierItem,
                name = "Chest",
                description = "A cloesd wooden chest."
            };
            chest.transform = new Transform();
            chest.inventory = new Inventory(chest);
            chest.inventory.AddItem(GoldStack.Create(50));


            return chest;

        }

    }
}
