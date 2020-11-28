using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Components.Usables;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Data.Items
{
    [Serializable]
    class MedicalRags : Entity
    {
        public static MedicalRags Create()
        {
            MedicalRags medicalRags = new MedicalRags
            {
                symbol = '(',
                color = Colors.LowTeirItem,
                name = "Medical Rags",
                description = "Shreded and torn medical rags."
            };
            medicalRags.equipment = new Equipment(medicalRags, EquipSlotType.CHEST);
            medicalRags.equipment.attackModifier = 100;

            medicalRags.effect = new Equipable();
            medicalRags.effect.owner = medicalRags;
            return medicalRags;

        }
    }
}
