using RogueSharp.DiceNotation;
using sharpRoguelike.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Monsters
{
    [Serializable]
    public class Cryofailure : Entity
    {
        public static Cryofailure Create(int level)
        {

            Cryofailure cryoFailure = new Cryofailure
            {
                name = "Cryofailure",
                color = Colors.KoboldColor,
                symbol = 'k',
                description = "a slippery accident of science born from a pod"
            };

            Attacker attacker = new Attacker(cryoFailure);
            attacker.Attack = Dice.Roll("1D3") + level / 3;
            attacker.AttackChance = Dice.Roll("25D3");
            attacker.Defense = Dice.Roll("1D3") + level / 3;
            attacker.DefenseChance = Dice.Roll("10D4");

            int health = Dice.Roll("2D5");
            attacker.Health = health;
            attacker.MaxHealth = health;

            cryoFailure.attacker = attacker;

            Actor actor = new Actor(cryoFailure);
            actor.Awareness = 10;
            actor.Speed = 9;

            cryoFailure.actor = actor;

            Entity corpse = new Entity()
            {
                name = "Cryofailure Corpse",
                color = Colors.Corpse,
                symbol = '%'
            };

            cryoFailure.corpse = corpse;

            cryoFailure.ai = new AI();
            cryoFailure.ai.owner = cryoFailure;
            return cryoFailure;

        }

    }
}
