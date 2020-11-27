using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Components.Usables;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Items
{
    [Serializable]
    class Dagger : Entity
    {
        public static Dagger Create()
        {
            Dagger dagger = new Dagger
            {
                symbol = '/',
                color = Colors.LowTierWeapon,
                name = "Dagger",
                description = "A crude dagger - adds 2 to attack"
            };
            dagger.equipment = new Equipment(dagger, EquipSlotType.WEAPON);
            dagger.equipment.attackModifier = 100;

            dagger.effect = new Equipable();
            dagger.effect.owner = dagger;

            

            return dagger;

        }
    }
}
