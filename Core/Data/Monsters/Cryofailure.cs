using RogueSharp.DiceNotation;
using sharpRoguelike.Core.Behaviors;
using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Data.Items;
using sharpRoguelike.Core.Data.Items.Weapons;
using sharpRoguelike.Core.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Data.Monsters
{
    [Serializable]
    public class Cryofailure : Entity
    {
        

        public static Cryofailure Create(int level)
        {

            Cryofailure cryoFailure = new Cryofailure
            {
                name = "Cryofailure",

                description = "a slippery accident of science born from a pod"
            };
            cryoFailure.transform = new Transform();
            cryoFailure.renderer = new Renderer(cryoFailure);
            cryoFailure.renderer.color = Colors.KoboldColor;
            cryoFailure.renderer.symbol = 'k';

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
            actor.Speed = 11;

            cryoFailure.actor = actor;

            Entity corpse = new Entity()
            {
                name = "Cryofailure Corpse",
  
            };
            corpse.transform = new Transform();
            corpse.renderer = new Renderer(corpse);
            corpse.renderer.color = Colors.KoboldColor;
            corpse.renderer.symbol = '%';



            cryoFailure.corpse = corpse;

            cryoFailure.corpse.inventory = new Inventory(cryoFailure);

            cryoFailure.equipmentSlots = new List<EquipmentSlot>();
            cryoFailure.equipmentSlots.Add(new EquipmentSlot(cryoFailure, EquipSlotType.CHEST));
            cryoFailure.equipmentSlots.Add(new EquipmentSlot(cryoFailure, EquipSlotType.WEAPON));
            cryoFailure.equipmentSlots.Add(new EquipmentSlot(cryoFailure, EquipSlotType.LEGS));
            cryoFailure.equipmentSlots.Add(new EquipmentSlot(cryoFailure, EquipSlotType.RING));
            cryoFailure.equipmentSlots.Add(new EquipmentSlot(cryoFailure, EquipSlotType.RING));
            cryoFailure.equipmentSlots.Add(new EquipmentSlot(cryoFailure, EquipSlotType.RANGED_WEAPON));

            bool rags  = false;
            bool club  = false;
            bool bullet= false;
            bool gold = false;
            for(int i = 0; i < 3; i++)
            {
                int lootroll = Game.Random.Next(0, 100);
                if (lootroll < 10)
                {
                    if (!bullet)
                    {
                        bullet = true;
                        cryoFailure.corpse.inventory.AddItem(BulletStack.Create(Game.Random.Next(2,5)), true);
                    }
                }
                else if (lootroll < 20)
                {
                    if (!gold)
                    {
                        gold = true;
                        cryoFailure.corpse.inventory.AddItem(GoldStack.Create(Game.Random.Next(5, 10)), true);
                    }

                }
                else if (lootroll < 50)
                {
                    if (!rags)
                    {
                        rags = true;
                        EquipmentManager.EquipItem(cryoFailure, TornRags.Create());
                    }
                }
                else if (lootroll < 80)
                {
                    if (!club)
                    {
                        club = true;
                        EquipmentManager.EquipItem(cryoFailure, Club.Create());
                    }
                }
            }

            cryoFailure.ai = new AI();
            cryoFailure.ai.owner = cryoFailure;
            cryoFailure.ai.behaviorStack = new List<Interfaces.IBehavior>();
            cryoFailure.ai.behaviorStack.Add(new StandardMoveAndAttack());
            cryoFailure.statusEffectHolder = new StatusEffectHolder(cryoFailure);

            return cryoFailure;

        }

    }

    
}
