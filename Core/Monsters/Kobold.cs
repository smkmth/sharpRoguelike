using RogueSharp.DiceNotation;
using sharpRoguelike.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Monsters
{
    public class Kobold : Monster
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
            actor.Speed = 14;

            kobold.actor = actor;

            return kobold;

        }

    }
}
