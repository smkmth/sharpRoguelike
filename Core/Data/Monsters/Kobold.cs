using RogueSharp.DiceNotation;
using sharpRoguelike.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Monsters
{
    [Serializable]
    public class Kobold : Entity
    {
        public static Kobold Create(int level)
        {

            Kobold kobold = new Kobold
            {
                name = "Kobold",
                color = Colors.KoboldColor,
                symbol = 'k'
            };

            Attacker attacker = new Attacker(kobold);
            attacker.Attack = Dice.Roll("1D3") + level / 3;
            attacker.AttackChance = Dice.Roll("25D3");
            attacker.Defense = Dice.Roll("1D3") + level / 3;
            attacker.DefenseChance = Dice.Roll("10D4");

            int health = Dice.Roll("2D5");
            attacker.Health = health;
            attacker.MaxHealth = health;

            kobold.attacker = attacker;

            Actor actor = new Actor(kobold);
            actor.Awareness = 10;
            actor.Speed = 9;

            kobold.actor = actor;

            Entity corpse = new Entity()
            {
                name = "Kolbold Corpse",
                color = Colors.Corpse,
                symbol = '%'
            };
            kobold.corpse = corpse;

            kobold.ai = new AI();
            kobold.ai.owner = kobold;
            return kobold;

        }

    }
}
