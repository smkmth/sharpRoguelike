using sharpRoguelike.Core.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
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

            owner.color = Colors.Player;
            owner.symbol = '@';
            owner.name = "Rogue";

            owner.attacker = new Attacker(owner);
            owner.attacker.Attack = 2;
            owner.attacker.AttackChance = 50;
            owner.attacker.Defense = 2;
            owner.attacker.DefenseChance = 20;
            owner.attacker.Health = 100;
            owner.attacker.MaxHealth = 100;

            owner.actor = new Actor(owner);
            owner.actor.Awareness = 15;
            owner.actor.Speed = 10;
            owner.inventory = new Inventory(owner);
            owner.inventory.AddItem(HealthPotion.Create());
        }
    }
}
