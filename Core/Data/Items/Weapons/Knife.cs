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
 
                name = "Knife",
                description = "A combat knife - adds 2 to attack"
            };
            knife.renderer = new Renderer(knife);
            knife.renderer.symbol = '/';
            knife.renderer.color = Colors.LowTeirItem;
            knife.transform = new Transform();
            knife.equipment = new Equipment(knife, EquipSlotType.WEAPON);
            knife.equipment.attackModifier = 2;
            knife.effect = new EquipmentUse();
            knife.effect.owner = knife;

            

            return knife;

        }
    }
}
