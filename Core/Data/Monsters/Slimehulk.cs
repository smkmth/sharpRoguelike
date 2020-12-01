using RogueSharp.DiceNotation;
using sharpRoguelike.Core.Behaviors;
using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Components.StatusEffects;
using sharpRoguelike.Core.Components.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Data.Monsters
{
    [Serializable]
    class Slimehulk : Entity
    {

        public static Slimehulk Create(int level)
        {

            Slimehulk slimeHulk = new Slimehulk
            {
                name = "slimeHulk",

                description = "a yellowtinged gelatinous monster of slime"
            };
            slimeHulk.transform = new Transform();
            slimeHulk.renderer = new Renderer(slimeHulk);
            slimeHulk.renderer.color = Colors.SlimehulkColor;
            slimeHulk.renderer.symbol = 'S';
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
           
            };

            corpse.transform = new Transform();
            corpse.renderer = new Renderer(corpse);
            corpse.renderer.color = Colors.SlimehulkColor;
            corpse.renderer.symbol = '%';

            slimeHulk.corpse = corpse;

            slimeHulk.corpse.inventory = new Inventory(slimeHulk);


            slimeHulk.ai = new AI();
            slimeHulk.ai.owner = slimeHulk;
            slimeHulk.ai.behaviorStack = new List<Interfaces.IBehavior>();
            slimeHulk.ai.behaviorStack.Add(new StandardMoveAndAttack());

            slimeHulk.immuneStatusEffects = new List<StatusEffectsComp>();
            slimeHulk.immuneStatusEffects.Add(StatusEffectsComp.Slimed);

            return slimeHulk;

        }
        public override void OnMove(int previous_x, int previous_y, int end_x, int end_y)
        {
            base.OnMove(previous_x, previous_y, end_x, end_y);

            SlimeSurface surface = new SlimeSurface();

            Game.DungeonMap.CreateSurface(previous_x, previous_y, 2, "Slime", Colors.SlimehulkColor, '~', surface);
        }
    }
}
