using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    [Serializable]
    public enum EquipSlotType
    {
        WEAPON,
        CHEST,
        LEGS,
        RING
    }

    [Serializable]
    public class Equipment
    {
        public Entity ownerItem;
        public Entity ownerCreature;

        public int attackModifier;
        public int attackChanceModifier;
        public int defenceModfier;
        public int defenceChanceModifier;
        public int maxHealthModifier;

        public string effectString;
        public EquipSlotType slotType;

        public Equipment(Entity _owner, EquipSlotType _slotType)
        {
            ownerItem = _owner;
            slotType = _slotType;
        }
    }
}
