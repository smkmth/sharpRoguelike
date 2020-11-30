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
                symbol = '.',
                color = Colors.Gold,
                name = $"A pile of gold",
                description = "small round units of currency"
            };
            gold.stackable = true;
            gold.stackcount = count;
            return gold;

        }
    }
}
