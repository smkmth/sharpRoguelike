using RLNET;
using sharpRoguelike.Core.Behaviors;
using sharpRoguelike.Core.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    public class Monster : Entity
    {
        public int? TurnsAlerted { get; set; }

        public virtual void PerformAction(CommandSystem commandSystem)
        {
            var behavior = new StandardMoveAndAttack();
            behavior.Act(this, commandSystem);
        }

        public override void DrawStats(RLConsole statConsole, int pos)
        {

            int yPosition = 13 + (pos * 2);

            statConsole.Print(1, yPosition, symbol.ToString(), color);

            int width = Convert.ToInt32(((double)attacker.Health / (double)attacker.MaxHealth) * 16.0);
            int remainingWidth = 16 - width;

            statConsole.SetBackColor(3, yPosition, width, 1, Swatch.Primary);
            statConsole.SetBackColor(3 + width , yPosition, remainingWidth, 1, Swatch.PrimaryDarkest);

            statConsole.Print(2, yPosition, $": {name}", Swatch.DbLight);




        }
    }
}
