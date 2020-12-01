using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Data.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{


    //an inventory is a space to put items - inventories are quired by inventory menus, and
    //they can hold any kind of entity. This is kind of flexible because it means anything can
    //potentially be picked up by anything with an inventory.
    //the only really intersting thing to note is that dropping items has some functionality
    //here - it instantiates the item into the world. 
    [Serializable]
    public class Inventory
    {
        public Entity owner;
        public List<Entity> storedItems;
        public int maxCapacity = 26;
        public Inventory(Entity _owner)
        {
            owner = _owner;
            storedItems = new List<Entity>(maxCapacity);
        }

        public bool AddItem(Entity item, bool silent = false)
        {
       
            if (storedItems.Count + 1 >= maxCapacity)
            {
                if (!silent)
                {
                    Game.MessageLog.Add($"Cant pick up {item.name}, inventory is full", Colors.ErrorMessage);
                }
                return false;
            }
            else
            {
                if (!silent)
                {
                    Game.MessageLog.Add($"Picked up {item.name}.", Colors.NormalMessage);
                }
                if (item.stackable)
                {
                    foreach (Entity check in storedItems)
                    {
                        if (check.GetType() == item.GetType())
                        {
                            check.stackcount += item.stackcount;
                            return true;
                        }
                        

                    }

                }
                storedItems.Add(item);
                return true;
            }
        }

        public void DropItem(Entity item)
        {
            item.transform.x = owner.transform.x;
            item.transform.y = owner.transform.y;
            Game.DungeonMap.AddItem(item);
            storedItems.Remove(item);
            Game.MessageLog.Add($"Dropped {item.name}.", Colors.NormalMessage);
        }

        public void ConsumeItem(Entity item)
        {
            storedItems.Remove(item);

        }

            /*
        public int LoadAmmoOfType(AmmoType type, int clip)
        {
            BulletStack bullets = new BulletStack();
            foreach(Entity item in storedItems)
            {
                if (item is BulletStack)
                {
                    bullets = item as BulletStack;
                }
            }

            if ( (bullets.stackcount - clip) >= 0)
            {
                bullets.stackcount -= clip;
                if (bullets.stackcount == 0)
                {
                    storedItems.Remove(bullets);
                }
                return clip;
            }
            else
            {
                bullets.stackcount -= clip;
                clip += bullets.stackcount;
                storedItems.Remove(bullets);
                return clip;
            }

            return 0;
            
        }

            */

    }
}
