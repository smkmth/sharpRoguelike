using sharpRoguelike.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Systems
{
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
