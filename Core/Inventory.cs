using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    public class Inventory
    {
        public List<Item> storedItems;
        public int maxCapacity = 26;
        public Inventory()
        {
            storedItems = new List<Item>(maxCapacity);
        }

        public bool AddItem(Item item)
        {
            if (storedItems.Count + 1 >= maxCapacity)
            {
                Game.MessageLog.Add($"Cant pick up {item.name}, inventory is full");
                return false;
            }
            else
            {
                Game.MessageLog.Add($"Picked up {item.name}.");
                storedItems.Add(item);
                return true;
            }
        }


     
    }
}
