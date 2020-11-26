using System;
using System.Collections.Generic;
using System.Text;
using RLNET;

namespace sharpRoguelike.Core
{
    class Colors
    {

        public static RLColor FloorBackground = RLColor.Black;
        public static RLColor Floor = Swatch.AlternateDarkest;
        public static RLColor FloorBackgroundFov = Swatch.DbDark;
        public static RLColor FloorFov = Swatch.Alternate;

        public static RLColor WallBackground = Swatch.SecondaryDarkest;
        public static RLColor Wall = Swatch.Secondary;
        public static RLColor WallBackgroundFov = Swatch.SecondaryDarker;
        public static RLColor WallFov = Swatch.SecondaryLighter;

        public static RLColor TextHeading = Swatch.DbLight;


        public static RLColor Player = Swatch.Primary;


        public static RLColor Text = Swatch.DbLight;
        public static RLColor Gold = Swatch.DbSun;
        
        //items
        public static RLColor HealthPotion = Swatch.DbBlood;
        public static RLColor DoorBackground = Swatch.ComplimentDarkest;
        public static RLColor Door = Swatch.ComplimentLighter;
        public static RLColor DoorBackgroundFov = Swatch.ComplimentDarker;
        public static RLColor DoorFov = Swatch.ComplimentLightest;

        //monsters
        public static RLColor KoboldColor = Swatch.DbBrightWood;

        public static RLColor HelpMessage = new RLColor(254, 203, 0);
        public static RLColor ErrorMessage = new RLColor(194, 27, 7);
        public static RLColor CombatMessage = Swatch.DbBlood;
        public static RLColor NormalMessage = new RLColor(239,234,224);
        public static RLColor HealMessage = Swatch.DbGrass;



    }
}
