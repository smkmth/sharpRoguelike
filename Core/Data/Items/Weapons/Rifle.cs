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
                name = "Rifle",
                description = "A long barreled hunting rifle"
            };
            rifle.transform = new Transform();

            rifle.renderer = new Renderer(rifle);
            rifle.renderer.symbol = '¬';
            rifle.renderer.color = Colors.MidTierItem;

            rifle.equipment = new Equipment(rifle, EquipSlotType.RANGED_WEAPON);
            RifleEffect rifleEffect = new RifleEffect(rifle.equipment, 5);
            rifle.equipment.ranged = rifleEffect; 
            rifle.equipment.ranged.damage = "1D5";
            rifle.equipment.ranged.loudness = 40;
            rifle.equipment.ranged.targetString = "Rogue aims the rifle. Left Click a target to shoot, Right Click to cancel";
            rifle.effect = new RangedEquipUse(rifleEffect);
            rifle.effect.owner = rifle;
            return rifle;

        }
    }
}
