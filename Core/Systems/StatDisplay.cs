using RLNET;
using sharpRoguelike.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Systems
{
   
    //responsible for displaying the monsters / players health on the bottom of the screen 
    public class StatDisplay
    {
        int stepDistance = 20;
        int offset = 10;

        public string CapFirstLetter(string str)
        {
            str = char.ToUpper(str[0]) + str.Substring(1);
            return str;
        }

        public void DrawPlayer(RLConsole con, Entity player)
        {
            string statuseffectString = "";
            if (player.statusEffectHolder.statusEffects != null)
            {
                int effectcounter = 0;
                
                foreach (StatusEffect effect in player.statusEffectHolder.statusEffects)
                {
                    if(effectcounter == 0)
                    {
                        string capsString= CapFirstLetter(effect.statusEffectAppliedName);
                        statuseffectString += $"{capsString}";
                    }
                    else if (effectcounter == player.statusEffectHolder.statusEffects.Count)
                    {
                        statuseffectString += $" {effect.statusEffectAppliedName} ";

                    }
                    else
                    {
                        statuseffectString += $", {effect.statusEffectAppliedName}";
                    }

                }
            }


            con.Print(1, 1, $"Name: {statuseffectString} {player.name}", Colors.Text);
            con.Print(1, 3, $"Health:  {player.attacker.Health}/{player.attacker.MaxHealth}", Colors.Text);
            con.Print(1, 5, $"Attack:  {player.attacker.Attack} ({player.attacker.AttackChance}%)", Colors.Text);
            con.Print(1, 7, $"Defense: {player.attacker.Defense} ({player.attacker.DefenseChance}%)", Colors.Text);
            con.Print(1, 9, $"Speed: {player.actor.Speed}", Colors.Text);
         
          

        }

        public void DrawMonsters(RLConsole con, List<Entity> monsters)
        {
            int moncount = 0;
            int xoffset = 0;
            int yoffset = 1;
            foreach (Entity monster in monsters)
            {
                yoffset = 1;
                if ((moncount % 2) == 0)
                {
                    yoffset = 5;
                    xoffset++;
                }
                int xPosition = offset + (stepDistance * xoffset);

                con.Print(xPosition, yoffset, monster.name, Colors.Text);

                int width = Convert.ToInt32(((double)monster.attacker.Health / (double)monster.attacker.MaxHealth) * 16.0);
                int remainingWidth = 16 - width;

                con.SetBackColor(xPosition, yoffset + 2, width, 1, Colors.Primary);
                con.SetBackColor(xPosition + width, yoffset+2, remainingWidth, 1, Colors.PrimaryDarkest);

                moncount++;
            }


        }
    }
}
