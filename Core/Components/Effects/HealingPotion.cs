using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    class HealingPotion : Useable
    {
        public HealingPotion()
        {
            usageChars = new List<char>();
            usageChars.Add('A');
            usageChars.Add('B');
            displayChars = new List<char>();
            displayChars.Add('a');
            displayChars.Add('b');
            usageNames = new List<string>();
            usageNames.Add("Drink");
            usageNames.Add("Throw");
        }
        public override bool Use( char usageChar, Entity user, Entity target)
        {
            if (usageChar == 'A')
            {

                if (target.attacker != null)
                {

                    if (target.attacker.Health == target.attacker.MaxHealth)
                    {
                        Game.MessageLog.Add("Tried to drink health potion, but was already at max health");
                        return false;
                    }
                    else
                    {
                        target.attacker.Health += 20;
                        if (target.attacker.Health > target.attacker.MaxHealth)
                        {
                            target.attacker.Health = target.attacker.MaxHealth;
                        }
                        Game.MessageLog.Add($"{target.name} gulped down the health potion - and their wounds began to mend.");
                        return true;

                    }
                }
                else
                {
                    return false;
                }           
            } 
            else if (usageChar == 'B')
            {
                Game.MessageLog.Add("You take aim with the potion bottle. Left Click to throw, Right Click to cancel.");

                Game.CurrentGameMode = GameMode.TARGETING;
                Game.targetCallback = ResolveThrow;
                Game.targetCancelCallback = CancelThrow;
                return true;
            }

            return false;
        }

        public void ResolveThrow(int x, int y)
        {
            Game.MessageLog.Add("The potion bottle smashes on the ground - spilling healing liquid everywhere!");
            Game.DungeonMap.CreateSurface(x, y, 3, "Healing Liquid", RLNET.RLColor.Red);
        }

        public void CancelThrow(bool canceled)
        {
            Game.MessageLog.Add("Cancelled throw!");

            Game.Player.inventory.AddItem(owner,true);
        }

    }
}
