using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    //a equipment slot is a space for owners to hold onto equipment, its quiered differently 
    //to an inventory - and it also has a 'type' like -head - or chest or weapon or somthing
    [Serializable]
    public class EquipmentSlot
    {
        public Equipment attachedEquipment;
        public Entity owner;
        public EquipSlotType equipType;
        public EquipmentSlot(Entity _owner, EquipSlotType _equipType)
        {
            equipType = _equipType;

        }
    }
}
