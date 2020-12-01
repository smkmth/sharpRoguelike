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

                name = "Chest",
                description = "A cloesd wooden chest."
            };

            chest.renderer = new Renderer(chest);
            chest.renderer.symbol = '&';
            chest.renderer.color = Colors.HighTierItem;

            chest.transform = new Transform();

            chest.inventory = new Inventory(chest);
            chest.inventory.AddItem(GoldStack.Create(50));


            return chest;

        }

    }
}
