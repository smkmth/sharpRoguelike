using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Components.Usables;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Data.Items.Weapons
{
    [Serializable]
    class Club : Entity
    {
        public static Club Create()
        {

            Club club = new Club
            {
                symbol = '/',
                color = Colors.LowTeirItem,
                name = "Club",
                description = "A large blunt object, adds 10 to chance to hit"
            };
            club.equipment = new Equipment(club, EquipSlotType.WEAPON);
            club.equipment.attackChanceModifier = 10;

            club.effect = new EquipmentUse();
            club.effect.owner = club;



            return club;
        }
    }
}
