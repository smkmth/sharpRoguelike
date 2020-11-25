using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
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
            }
        }

        public int MaxHealth
        {
            get
            {
                return _maxHealth;
            }
            set
            {
                _maxHealth = value;
            }
        }

    }


}
