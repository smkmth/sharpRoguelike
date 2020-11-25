using System;
using RLNET;
using sharpRoguelike.Core;

namespace sharpRoguelike
{
    public static class Game
    {
        public static readonly int screenWidth = 100;
        public static readonly int screenHeight = 70;
        private static RLRootConsole rootConsole;

        private static readonly int mapWidth = 80;
        private static readonly int mapHeight = 48;
        private static RLConsole mapConsole;

        private static readonly int messageWidth = 80;
        private static readonly int messageHeight = 11;
        private static RLConsole messageConsole;

        private static readonly int statWidth = 20;
        private static readonly int statHeight = 70;
        private static RLConsole statConsole;

        private static readonly int inventoryWidth = 80;
        private static readonly int inventoryHeight = 11;
        private static RLConsole inventoryConsole;

        public static Player Player { get;  set; }
        public static DungeonMap dungeonMap { get; private set; }

        static void Main(string[] args)
        {

            string fontFileName = "terminal8x8.png";
            string consoleTitle = "alchymia";
            rootConsole = new RLRootConsole(fontFileName, screenWidth, screenHeight, 8,8, 1f, consoleTitle);
            
            mapConsole = new RLConsole(mapWidth, mapHeight);
            statConsole = new RLConsole(statWidth, statHeight);
            inventoryConsole = new RLConsole(inventoryWidth, inventoryHeight);
            messageConsole = new RLConsole(messageWidth, messageHeight);

            Player = new Player();

            MapGenerator mapGenerator = new MapGenerator(mapWidth, mapHeight);
            dungeonMap = mapGenerator.CreateMap();

            dungeonMap.UpdatePlayerFOV();

            rootConsole.Update += OnRootConsoleUpdate;
            rootConsole.Render += OnRootConsoleRender;
            rootConsole.Run();
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            mapConsole.SetBackColor(0, 0, mapWidth, mapHeight, Colors.FloorBackground);
            mapConsole.Print(1, 1, "Map", RLColor.White);

            messageConsole.SetBackColor(0, 0, messageWidth, messageHeight, Swatch.DbDeepWater);
            messageConsole.Print(1, 1, "Messages", RLColor.White);

            statConsole.SetBackColor(0, 0, statWidth, statHeight, Swatch.DbOldStone);
            statConsole.Print(1, 1, "Stats", RLColor.White);

            inventoryConsole.SetBackColor(0, 0, inventoryWidth, inventoryHeight, Swatch.DbWood);
            inventoryConsole.Print(1, 1, "Inventory", RLColor.White);
        }

        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            // Blit the sub consoles to the root console in the correct locations
            RLConsole.Blit(mapConsole, 0, 0, mapWidth, mapHeight,
              rootConsole, 0, inventoryHeight);
            RLConsole.Blit(statConsole, 0, 0, statWidth, statHeight,
              rootConsole, mapWidth, 0);
            RLConsole.Blit(messageConsole, 0, 0, messageWidth, messageHeight,
              rootConsole, 0, screenHeight - messageHeight);
            RLConsole.Blit(inventoryConsole, 0, 0, inventoryWidth, inventoryHeight,
              rootConsole, 0, 0);
            rootConsole.Draw();
            dungeonMap.Draw(mapConsole);
            Player.Draw(mapConsole, dungeonMap);
        }
    }
}
