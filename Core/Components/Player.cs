using sharpRoguelike.Core.Data.Items;
using sharpRoguelike.Core.Data.Items.Weapons;
using sharpRoguelike.Core.Items;
using sharpRoguelike.Core.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    //the big cahoona - the player is a component that marks an entity as a player- 
    //if an entities .player != null - then you have a player. One thing to note
    //is that the ResetPlayer function here stores all the players data - just 
    //a lazy ease of use thing really. 
    //TODO - let the player migrate by giving another entity a .player
    [Serializable]
    public class Player
    {
        public Entity owner;

        public Player(Entity _owner)
        {
            owner = _owner;
        }


        public void ResetPlayer()
        {
            if (owner.statusEffects != null)
            { 
                owner.statusEffects.Clear();
            }

            owner.color = Colors.Player;
            owner.symbol = '@';
            owner.name = "Rogue";
            owner.transform = new Transform();
            owner.attacker = new Attacker(owner);
            owner.attacker.Attack = 2;
            owner.attacker.AttackChance = 50;
            owner.attacker.Defense = 2;
            owner.attacker.DefenseChance = 20;
            owner.attacker.Health = 100;
            owner.attacker.MaxHealth = 100;
            owner.attacker.Accuracy = 70;

            owner.actor = new Actor(owner);
            owner.actor.Awareness = 15;
            owner.actor.Speed = 10;
            owner.inventory = new Inventory(owner);

            owner.inventory.AddItem(HealthPotion.Create(), true);

            owner.inventory.AddItem(SlimePotion.Create(),true);

            owner.inventory.AddItem(WaterPotion.Create(),true);
            owner.inventory.AddItem(WaterPotion.Create(),true);
            owner.inventory.AddItem(WaterPotion.Create(),true);

            owner.inventory.AddItem(BulletStack.Create(30),true);

            owner.inventory.AddItem(BearTrap.Create(),true);
            owner.inventory.AddItem(BearTrap.Create(),true);
            owner.inventory.AddItem(BearTrap.Create(),true);

            owner.equipmentSlots = new List<EquipmentSlot>();
            owner.equipmentSlots.Add(new EquipmentSlot(owner, EquipSlotType.CHEST));
            owner.equipmentSlots.Add(new EquipmentSlot(owner, EquipSlotType.WEAPON));
            owner.equipmentSlots.Add(new EquipmentSlot(owner, EquipSlotType.LEGS));
            owner.equipmentSlots.Add(new EquipmentSlot(owner, EquipSlotType.RING));
            owner.equipmentSlots.Add(new EquipmentSlot(owner, EquipSlotType.RING));
            owner.equipmentSlots.Add(new EquipmentSlot(owner, EquipSlotType.RANGED_WEAPON));

            EquipmentManager.EquipItem(owner, Rifle.Create());
            EquipmentManager.EquipItem(owner, Knife.Create());
            
            // debug slime immune
            owner.immuneStatusEffects = new List<StatusEffectsComp>();
            owner.immuneStatusEffects.Add(StatusEffectsComp.Slimed);
        }
    }
}
