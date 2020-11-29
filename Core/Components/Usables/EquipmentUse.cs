using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.Usables
{
    [Serializable]
    public class EquipmentUse : Useable
    {
        public EquipmentUse()
        {
            usageChars = new List<char>();
            usageChars.Add('A');
            displayChars = new List<char>();
            displayChars.Add('a');
            usageNames = new List<string>();
            usageNames.Add("Equip/Unequip");
        }

        public override bool Use(char usageChar, Entity user, Entity target)
        {
            if (usageChar == 'A')
            {
                if (user.equipmentSlots != null && owner.equipment != null)
                {
                    foreach(EquipmentSlot slot in user.equipmentSlots)
                    {
                        if (slot.equipType == owner.equipment.slotType)
                        {
                            if (slot.attachedEquipment == owner.equipment)
                            {
                                slot.attachedEquipment = null;
                                return false;
                            }
                            else
                            {
                                slot.attachedEquipment = owner.equipment;
                                return true;
                            }
                            return true;
                        }
                    }
                }
                
            }

            return true;
        }
    }
}
