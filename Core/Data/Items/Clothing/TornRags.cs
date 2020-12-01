using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Components.Usables;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Data.Items
{
    [Serializable]
    class TornRags : Entity
    {
        public static TornRags Create()
        {
            TornRags tornRags = new TornRags
            {
                name = "Torn Rags",
                description = "Shreded and torn rags."
            };
            tornRags.renderer = new Renderer(tornRags);
            tornRags.renderer.symbol = '(';
            tornRags.renderer.color = Colors.LowTeirItem;
            tornRags.transform = new Transform();
            tornRags.equipment = new Equipment(tornRags, EquipSlotType.CHEST);
            tornRags.equipment.defenceModfier = 1;

            tornRags.effect = new EquipmentUse();
            tornRags.effect.owner = tornRags;
            return tornRags;

        }
    }
}
