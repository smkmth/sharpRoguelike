using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Components.Usables;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Items
{
    [Serializable]
    class Knife : Entity
    {
        public static Knife Create()
        {
            Knife knife = new Knife
            {
                symbol = '/',
                color = Colors.LowTeirItem,
                name = "Knife",
                description = "A combat knife - adds 2 to attack"
            };
            knife.equipment = new Equipment(knife, EquipSlotType.WEAPON);
            knife.equipment.attackModifier = 2;

            knife.effect = new EquipmentUse();
            knife.effect.owner = knife;

            

            return knife;

        }
    }
}
