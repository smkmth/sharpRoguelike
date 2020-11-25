using RLNET;
using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    public class Player : Entity
    {
        public Player()
        {

            color = Colors.Player;
            symbol = '@';
            name = "Rogue";


            attacker = new Attacker(this);
            attacker.Attack = 2;
            attacker.AttackChance = 50;
            attacker.Defense = 2;
            attacker.DefenseChance = 40;
            attacker.Health = 100;
            attacker.MaxHealth = 100;

            actor = new Actor(this);
            actor.Awareness = 15;
            actor.Speed = 10;
            
            inventory = new Inventory(this);
            inventory.AddItem(HealthPotion.Create());
            inventory.AddItem(HealthPotion.Create());
            inventory.AddItem(HealthPotion.Create());
            //TODO wierd - move this 
            Game.playerInventory = new Menus.InventoryMenu(inventory);
        }

        public override void DrawStats(RLConsole statConsole, int pos)
        {

            statConsole.Print(1, 1, $"Name:    {name}", Colors.Text);
            statConsole.Print(1, 3, $"Health:  {attacker.Health}/{attacker.MaxHealth}", Colors.Text);
            statConsole.Print(1, 5, $"Attack:  {attacker.Attack} ({attacker.AttackChance}%)", Colors.Text);
            statConsole.Print(1, 7, $"Defense: {attacker.Defense} ({attacker.DefenseChance}%)", Colors.Text);
        }

    }
}
