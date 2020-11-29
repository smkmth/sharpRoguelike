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
        RING,
        RANGED_WEAPON
    }
    //equipment expresses that its owner item is a piece of equipment- 
    //i am useing ownerItem here to EMPHASISE that an equipments 'owner'
    //is the 'item' entity that it is contained it - so for instance 
    //it might be owned by a dagger - not as you might think - be owned by 
    //a player or enemy or some such 
    [Serializable]
    public class Equipment
    {
        public Entity ownerItem;
        public Entity ownerHolder;
        public int attackModifier;
        public int attackChanceModifier;
        public int defenceModfier;
        public int defenceChanceModifier;
        public int maxHealthModifier;

        public string effectString;
        public EquipSlotType slotType;

        public RangedWeapon ranged = null;

        public Equipment(Entity _owner, EquipSlotType _slotType)
        {
            ownerItem = _owner;
            slotType = _slotType;
        }
    }
}
