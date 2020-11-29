using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Components.Usables;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Data.Items.Weapons
{
    [Serializable]
    class Rifle : Entity
    {
        public static Rifle Create()
        {
            Rifle rifle = new Rifle
            {
                symbol = '¬',
                color = Colors.MidTierItem,
                name = "Rifle",
                description = "A long barreled hunting rifle"
            };
            rifle.equipment = new Equipment(rifle, EquipSlotType.RANGED_WEAPON);
            rifle.equipment.ranged = new RifleEffect(rifle.equipment);
            rifle.equipment.ranged.damage = 5;
            rifle.equipment.ranged.targetString = "Rogue aims the rifle. Left Click a target to shoot, Right Click to cancel";
            rifle.effect = new EquipmentUse();
            rifle.effect.owner = rifle;
            return rifle;

        }
    }
}
