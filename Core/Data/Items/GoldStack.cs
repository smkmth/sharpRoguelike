using sharpRoguelike.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Data.Items
{
    [Serializable]
    class GoldStack : Entity
    {
        public static GoldStack Create(int count)
        {
            GoldStack gold = new GoldStack
            {

                name = $"A pile of gold",
                description = "small round units of currency"
            };

            gold.transform = new Transform();
            gold.renderer = new Renderer(gold);
            gold.renderer.symbol = '.';
            gold.renderer.color = Colors.Gold;
            gold.stackable = true;
            gold.stackcount = count;
            return gold;

        }
    }
}
