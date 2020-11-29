using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Data.Items
{
    [Serializable]
    class BulletStack : Entity
    {
        public static BulletStack Create()
        {
            BulletStack bulletstack = new BulletStack
            {
                symbol = ',',
                color = Colors.LowTeirItem,
                name = $"A pile of bullets",
                description = "ammo for rifles"
            };
            bulletstack.stackable = true;
            bulletstack.stackcount = 10;
            return bulletstack;

        }
    }
}
