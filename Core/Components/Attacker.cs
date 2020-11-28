using sharpRoguelike.Core.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    [Serializable]
    // attacker is mostly a plain old data file - it just holds on to the owners base attack stats
    // but it also pulls and modifies those stats with equipment if the owner has equipment
    public class Attacker
    {

        public Attacker(Entity _owner)
        {
            owner = _owner;
        }

        public Entity owner;
        private int _attack;
        private int _attackChance;
        private int _defense;
        private int _defenseChance;
        private int _maxHealth;
        private int _health;

        public int Attack
        {
            get
            {
                if (owner.equipmentSlots != null)       //can have equipment
                {
                    foreach(EquipmentSlot slot in owner.equipmentSlots)
                    {
                        if (slot.attachedEquipment != null)
                        {
                            return _attack + slot.attachedEquipment.attackModifier; 
                        }
                     }
                }
                return _attack;
            }
            set
            {
                _attack = value;
            }
        }

        public int AttackChance
        {
            get
            {
                if (owner.equipmentSlots != null)       //can have equipment
                {
                    foreach (EquipmentSlot slot in owner.equipmentSlots)
                    {
                        if (slot.attachedEquipment != null)
                        {
                            return _attackChance + slot.attachedEquipment.attackChanceModifier;
                        }
                    }
                }

                return _attackChance;
            }
            set
            {
                _attackChance = value;
            }
        }


        public int Defense
        {
            get
            {
                if (owner.equipmentSlots != null)       //can have equipment
                {
                    foreach (EquipmentSlot slot in owner.equipmentSlots)
                    {
                        if (slot.attachedEquipment != null)
                        {
                            return _defense + slot.attachedEquipment.attackModifier;
                        }
                    }
                }
                return _defense;
            }
            set
            {
                _defense = value;
            }
        }

        public int DefenseChance
        {
            get
            {
                if (owner.equipmentSlots != null)       //can have equipment
                {
                    foreach (EquipmentSlot slot in owner.equipmentSlots)
                    {
                        if (slot.attachedEquipment != null)
                        {
                            return _defenseChance + slot.attachedEquipment.defenceChanceModifier;
                        }
                    }
                }
                return _defenseChance;
            }
            set
            {
                _defenseChance = value;
            }
        }


        public int Health
        {
            get
            {
              
                return _health;
            }
            set
            { 
                _health = value;
                if (_health <= 0)
                {
                    CommandSystem.ResolveDeath(owner);
                }
            }
        }

        public int MaxHealth
        {
            get
            {

                if (owner.equipmentSlots != null)       //can have equipment
                {
                    foreach (EquipmentSlot slot in owner.equipmentSlots)
                    {
                        if (slot.attachedEquipment != null)
                        {
                            return _maxHealth + slot.attachedEquipment.maxHealthModifier;
                        }
                    }
                }
                return _maxHealth;
            }
            set
            {
                _maxHealth = value;
            }
        }

    }


}
