using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Data.Items
{
    class Bullet : Entity
    {
        public static BearTrap Create()
        {
            BearTrap beartrap = new BearTrap
            {
                symbol = ',',
                color = Colors.LowTeirItem,
                name = "A Bullet",
                description = "ammo for rifles"
            };



            return beartrap;

        }
    }
}
