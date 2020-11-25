using System;
using System.Collections.Generic;
using RLNET;
using RogueSharp.Random;
using sharpRoguelike.Core;
using sharpRoguelike.Core.Menus;
using sharpRoguelike.Core.Systems;

namespace sharpRoguelike
{
    public static class Game
    {

        public static bool fullScreen = false;
        public static readonly int screenWidth = 150;
        public static readonly int screenHeight = 80;
        private static RLRootConsole rootConsole;
        

        private static readonly int statWidth = 20;
        private static readonly int statHeight = screenHeight;
        private static RLConsole statConsole;

        private static readonly int lookWidth = screenWidth - 20;
        private static readonly int lookHeight = 11;
        private static RLConsole lookConsole;

        private static readonly int messageWidth = screenWidth - statWidth;
        private static readonly int messageHeight = 11;
        private static RLConsole messageConsole;

        private static readonly int mapWidth = screenWidth - statWidth;
        private static readonly int mapHeight = screenHeight - (messageHeight + lookHeight);
        private static RLConsole mapConsole;


        private static RLConsole inventoryConsole;


        public static Player Player { get;  set; }
        public static InventoryMenu playerInventory;
        public static CommandSystem CommandSystem { get; private set; }
        public static DungeonMap DungeonMap { get; private set; }

        public static MessageLog MessageLog { get; set; }
        public static SchedulingSystem SchedulingSytem { get; private set; }

        public static IRandom Random { get; private set; }

        public static bool didPlayerAct;
        public static bool shouldUpdateDraw =true;
        public static int steps;

        public static int mapLevel =1;
        public static GameMode CurrentGameMode;

        static void Main(string[] args)
        {
            int seed = (int)DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);

            string fontFileName = "terminal8x8.png";
            string consoleTitle = "alchymia - " + seed ;
            rootConsole = new RLRootConsole(fontFileName, screenWidth, screenHeight, 8,8, 1f, consoleTitle);
            rootConsole.SetWindowState(RLWindowState.Maximized);
          
            mapConsole = new RLConsole(mapWidth, mapHeight);
            statConsole = new RLConsole(statWidth, statHeight);
            lookConsole = new RLConsole(lookWidth, lookHeight);
            messageConsole = new RLConsole(messageWidth, messageHeight);
            inventoryConsole = new RLConsole(screenWidth, screenHeight);
            
            CommandSystem = new CommandSystem();
            SchedulingSytem = new SchedulingSystem();
            MessageLog = new MessageLog();
            MessageLog.Add("The rogue arrives on level 1");
            MessageLog.Add($" level created with seed : ' {seed}' , Map Level : '{mapLevel}'");

            MapGenerator mapGenerator = new MapGenerator(mapWidth, mapHeight,20,7,14, mapLevel);
            DungeonMap = mapGenerator.CreateMap();

            DungeonMap.UpdatePlayerFOV();

            rootConsole.Render += OnRootConsoleRender;
            rootConsole.Update += OnRootConsoleUpdate;
            rootConsole.Run();

            shouldUpdateDraw = true;
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            bool didPlayerAct = false;
            RLKeyPress keyPress = rootConsole.Keyboard.GetKeyPress();
            int mx = rootConsole.Mouse.X ;
            int my = rootConsole.Mouse.Y - lookHeight;
            

            if (mx > 0 && my > 0)
            {
                List<string> names = DungeonMap.InterigateEntityAtLocation(mx  , my);
                if (names.Count > 0)
                {
                    lookConsole.Clear();
                    for (int i =0; i< names.Count; i++)
                    {
                        lookConsole.Print(2, i, names[i], RLColor.White);
                    }
                }
                else
                {
                    lookConsole.Clear();
                }
            }

            if (CommandSystem.IsPlayerTurn)
            {

                if (keyPress != null)
                {
                    if (CurrentGameMode == GameMode.PLAYING)
                    {

                        if (keyPress.Key == RLKey.Up)
                        {
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                        }
                        else if (keyPress.Key == RLKey.Down)
                        {
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                        }
                        else if (keyPress.Key == RLKey.Left)
                        {
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                        }
                        else if (keyPress.Key == RLKey.Right)
                        {
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                        }
                        else if (keyPress.Key == RLKey.Escape)
                        {
                            rootConsole.Close();
                        }
                        else if (keyPress.Key == RLKey.G)
                        {
                            Entity pickup = DungeonMap.GetItemAt(Player.x, Player.y);
                            if (Player.inventory.AddItem(pickup))
                            {
                                DungeonMap.RemoveItem(pickup);
                            }
                        }
                        else if (keyPress.Key == RLKey.Period)
                        {
                            if (DungeonMap.CanMoveDownToNextLevel())
                            {
                                MapGenerator mapGenerator = new MapGenerator(mapWidth, mapHeight, 20, 7, 13, ++mapLevel);
                                DungeonMap = mapGenerator.CreateMap();
                                MessageLog = new MessageLog();
                                CommandSystem = new CommandSystem();
                                rootConsole.Title = $"RougeSharp RLNet Tutorial - Level {mapLevel}";
                                didPlayerAct = true;

                            }
                        }

                        if (keyPress.Key == RLKey.I)
                        {
                            
                            CurrentGameMode = GameMode.INVENTORY;
                            playerInventory.OnFirstEnter(inventoryConsole);
                        }
                    }
                    else if (CurrentGameMode == GameMode.INVENTORY)
                    {
                        if (keyPress.Key == RLKey.Escape)
                        {
                            CurrentGameMode = GameMode.PLAYING;
                        }
                        else
                        {
                            playerInventory.HandleInput(keyPress, inventoryConsole);

                        }
                    }

                    if (keyPress.Alt && keyPress.Key == RLKey.Enter)
                    {
                        if (!fullScreen)
                        {
                            fullScreen = true;
                            rootConsole.SetWindowState(RLWindowState.Fullscreen);
                        }
                        else
                        {
                            fullScreen = true;
                            rootConsole.SetWindowState(RLWindowState.Normal);

                        }
                    
                    }
                    
                  
                    keyPress = null;

                }
                if (didPlayerAct)
                {
                    shouldUpdateDraw = true;
                    CommandSystem.EndPlayerTurn();
                }
            }
            else
            {
                CommandSystem.ActivateMonsters();
                shouldUpdateDraw = true;
            }
            // In OnRootConsoleUpdate() replace the if ( didPlayerAct ) block
      
        }

        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {

            if (CurrentGameMode == GameMode.INVENTORY)
            {
                mapConsole.Clear();
                statConsole.Clear();

                RLConsole.Blit(inventoryConsole, 0, 0, screenWidth, screenHeight - messageHeight, rootConsole, 0, 0);
                RLConsole.Blit(messageConsole, 0, 0, messageWidth, messageHeight, rootConsole, 0, screenHeight - messageHeight);

                rootConsole.Draw();
                playerInventory.Draw(inventoryConsole);
                MessageLog.Draw(messageConsole);
                return;
            
            }
     
            // Blit the sub consoles to the root console in the correct locations
            RLConsole.Blit(mapConsole, 0, 0, mapWidth, mapHeight, rootConsole, 0, lookHeight);
            RLConsole.Blit(statConsole, 0, 0, statWidth, statHeight, rootConsole, mapWidth, 0);
            RLConsole.Blit(messageConsole, 0, 0, messageWidth, messageHeight, rootConsole, 0, screenHeight - messageHeight);
            RLConsole.Blit(lookConsole, 0, 0, lookWidth, lookHeight, rootConsole, 0, 0);
            

            if (shouldUpdateDraw)
            {
                rootConsole.Draw();

                mapConsole.Clear();
                statConsole.Clear();
                messageConsole.Clear();

                DungeonMap.Draw(mapConsole, statConsole);
                MessageLog.Draw(messageConsole);
                Player.Draw(mapConsole, DungeonMap);
                Player.DrawStats(statConsole,0);
            }

           

     
        }

   
    }
}
