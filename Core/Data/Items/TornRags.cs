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
                symbol = '(',
                color = Colors.LowTeirItem,
                name = "Torn Rags",
                description = "Shreded and torn rags."
            };
            tornRags.equipment = new Equipment(tornRags, EquipSlotType.CHEST);
            tornRags.equipment.defenceModfier = 1;

            tornRags.effect = new Equipable();
            tornRags.effect.owner = tornRags;
            return tornRags;

        }
    }
}
