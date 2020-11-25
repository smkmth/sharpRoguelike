using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Items
{
    public class HealthPotion : Item
    {
        public static HealthPotion Create()
        {
            return new HealthPotion
            {
                symbol = '!',
                color = Colors.HealthPotion,
                name = "Health Potion"
            };

        }

        public override bool Use(char usageChar, Actor user)
        {
            switch (usageChar)
            {
                case 'A':
                    if (user.Health == user.MaxHealth)
                    {
                        Game.MessageLog.Add("Tried to drink health potion, but was already at max health");
                        return false;
                    }
                    else
                    {
                        user.Health += 20;
                        if (user.Health > user.MaxHealth)
                        {
                            user.Health = user.MaxHealth;
                        }
                        Game.MessageLog.Add($"{user.name} gulped down the health potion - and their wounds began to mend.");
                        return true;

                    }
                    break;

            }

            return false;


        }
    }
}
