using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.Usables
{
    [Serializable]
    class Potion : Useable
    {
        public string drinkDescriptionMaxHealth;
        public string drinkDescription;

        public Potion()
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

        public override bool Use(char usageChar, Entity user, Entity target)
        {
            if (usageChar == 'A')
            {
                return DrinkEffect(user, target);
              
            }
            else if (usageChar == 'B')
            {
                return ThrowEffect(user, target);
            }

            return false;
        }

        public virtual void ResolveThrow(int x, int y)
        {

        }


        public virtual void CancelThrow(bool canceled)
        {
            Game.MessageLog.Add("Cancelled throw!", Colors.HelpMessage);

            Game.Player.inventory.AddItem(owner, true);
        }

        public virtual bool DrinkEffect(Entity user, Entity target)
        {
          
            return true;
        }

        public virtual bool ThrowEffect(Entity user, Entity target)
        {

            Game.MessageLog.Add("You take aim with the potion bottle. Left Click to throw, Right Click to cancel.", Colors.HelpMessage);

            Game.CurrentGameMode = GameMode.TARGETING;
            Game.targetCallback = ResolveThrow;
            Game.targetCancelCallback = CancelThrow;
            return true;
            
        }
    }

}
