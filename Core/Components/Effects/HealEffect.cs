using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    class HealEffect : Useable
    {
        public HealEffect()
        {
            usageChar = 'A';
            displayChar = 'a';
            usageName = "Drink";
        }
        public override bool Use( Entity user, Entity target)
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

        
    }
}
