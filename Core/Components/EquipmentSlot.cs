using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    [Serializable]
    public class EquipmentSlot
    {
        public string name;
        public Equipment attachedEquipment;
        public Entity owner;
        public EquipSlotType equipType;
        public EquipmentSlot(Entity _owner, EquipSlotType _equipType)
        {
            equipType = _equipType;

        }
    }
}
