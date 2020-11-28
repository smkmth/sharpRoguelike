using RogueSharp.DiceNotation;
using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Components.StatusEffects;
using sharpRoguelike.Core.Components.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Data.Monsters
{
    class Slimehulk : Entity
    {

        public static Slimehulk Create(int level)
        {

            Slimehulk slimeHulk = new Slimehulk
            {
                name = "slimeHulk",
                color = Colors.SlimehulkColor,
                symbol = 'S',
                description = "a yellowtinged gelatinous monster of slime"
            };

            Attacker attacker = new Attacker(slimeHulk);
            attacker.Attack = Dice.Roll("2D2") + level / 3;
            attacker.AttackChance = Dice.Roll("25D3");
            attacker.Defense = Dice.Roll("1D4") + level / 3;
            attacker.DefenseChance = Dice.Roll("10D4");

            int health = Dice.Roll("4D5");
            attacker.Health = health;
            attacker.MaxHealth = health;

            slimeHulk.attacker = attacker;

            Actor actor = new Actor(slimeHulk);
            actor.Awareness = 20;
            actor.Speed = 20;

            slimeHulk.actor = actor;

            Entity corpse = new Entity()
            {
                name = "slimehulk Corpse",
                color = Colors.Corpse,
                symbol = '%'
            };

            slimeHulk.corpse = corpse;

            slimeHulk.ai = new AI();
            slimeHulk.ai.owner = slimeHulk;

            slimeHulk.immuneStatusEffects = new List<StatusEffectsComp>();
            slimeHulk.immuneStatusEffects.Add(StatusEffectsComp.Slimed);

            return slimeHulk;

        }
        public override void OnMove(int previous_x, int previous_y, int end_x, int end_y)
        {
            base.OnMove(previous_x, previous_y, end_x, end_y);
            Entity liquid = new Entity();
            liquid.name = "Slime";
            liquid.color = Colors.SlimehulkColor;
            liquid.symbol = '~';
            liquid.surface = new SlimeSurface();

            Game.DungeonMap.CreateSurface(x, y, 3, liquid);
        }
    }
}
