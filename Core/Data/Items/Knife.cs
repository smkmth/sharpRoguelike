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
            Knife dagger = new Knife
            {
                symbol = '/',
                color = Colors.LowTeirItem,
                name = "Knife",
                description = "A combat knife - adds 2 to attack"
            };
            dagger.equipment = new Equipment(dagger, EquipSlotType.WEAPON);
            dagger.equipment.attackModifier = 2;

            dagger.effect = new Equipable();
            dagger.effect.owner = dagger;

            

            return dagger;

        }
    }
}
