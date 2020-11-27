using sharpRoguelike.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Systems
{
    //just a helper class so i can force give items to stuff without having to
    //boilerplate this checking shit over and over again. - 'oh i want a goblin to have like 
    // - a dagger and a hat - okay - then foreach slots in slots - if weapon - attach weapon 
    //foreach slots in slots if hat ect ect.
    public static class EquipmentManager
    {
        public static void EquipItem(Entity target, Entity item)
        {
            if (target.equipmentSlots != null && item.equipment != null)
            {
                foreach (EquipmentSlot slot in target.equipmentSlots)
                {
                    if (slot.equipType == item.equipment.slotType)
                    {
                        slot.attachedEquipment = item.equipment;
                    }
                }
            }

        }

    }
}
