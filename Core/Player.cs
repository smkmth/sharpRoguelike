using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    public class Player : Actor
    {
        public Player()
        {
            awareness = 15;
            name = "Rogue";
            color = Colors.PlayerColor;
            symbol = '@';
            x = 10;
            y = 10;

        }
    }
}
