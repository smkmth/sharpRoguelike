using RLNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Systems
{
   

    public class StatDisplay
    {
        int stepDistance = 20;
        int offset = 5;
        public void DrawPlayer(RLConsole con, Entity player)
        {

            con.Print(1, 1, $"Name:    {player.name}", Colors.Text);
            con.Print(1, 3, $"Health:  {player.attacker.Health}/{player.attacker.MaxHealth}", Colors.Text);
            con.Print(1, 5, $"Attack:  {player.attacker.Attack} ({player.attacker.AttackChance}%)", Colors.Text);
            con.Print(1, 7, $"Defense: {player.attacker.Defense} ({player.attacker.DefenseChance}%)", Colors.Text);

        }

        public void DrawMonsters(RLConsole con, List<Monster> monsters)
        {
            int moncount = 0;
            int xoffset = 0;
            int yoffset = 1;
            foreach (Monster monster in monsters)
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

                con.SetBackColor(xPosition, yoffset + 2, width, 1, Swatch.Primary);
                con.SetBackColor(xPosition + width, yoffset+2, remainingWidth, 1, Swatch.PrimaryDarkest);

                moncount++;
            }


        }
    }
}
