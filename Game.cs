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

        private static RLConsole menuConsole;
        
        public static bool fullScreen = false;
        public static Player Player { get;  set; }
        
        public static SaveLoadSystem saveLoad { get; private set; }

        public static InventoryMenu playerInventory { get; private set; }

        public static MainMenu mainMenu { get; private set; }
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
        public static int seed;



        static void Main(string[] args)
        {
            saveLoad = new SaveLoadSystem();
            CurrentGameMode = GameMode.MAINMENU;
            string fontFileName = "terminal8x8.png";
            string consoleTitle = "alchymia";
            rootConsole = new RLRootConsole(fontFileName, screenWidth, screenHeight, 8,8, 1.4f, consoleTitle);
            rootConsole.SetWindowState(RLWindowState.Maximized);
          
            mapConsole = new RLConsole(mapWidth, mapHeight);
            statConsole = new RLConsole(statWidth, statHeight);
            lookConsole = new RLConsole(lookWidth, lookHeight);
            messageConsole = new RLConsole(messageWidth, messageHeight);
            menuConsole = new RLConsole(screenWidth, screenHeight);
            
            CommandSystem = new CommandSystem();
            SchedulingSytem = new SchedulingSystem();
            MessageLog = new MessageLog();
            mainMenu = new MainMenu();

            shouldUpdateDraw = false;
            mainMenu.OnFirstEnter();

            rootConsole.Render += OnRootConsoleRender;
            rootConsole.Update += OnRootConsoleUpdate;
            rootConsole.Run();


        }

        public static void ResetGame()
        {
            CurrentGameMode = GameMode.MAINMENU;
            messageConsole.Clear();
            MessageLog.Clear();
        }


        public static void StartGame(int? enter_seed=null)
        {
            //get seed
            if (enter_seed != null)
            {
                seed = (int)enter_seed;
            }
            else
            {
                seed = (int)DateTime.UtcNow.Ticks;
            }

            Random = new DotNetRandom(seed);
            //gen map
            MapGenerator mapGenerator = new MapGenerator(mapWidth, mapHeight, 20, 7, 14, mapLevel);
            DungeonMap = mapGenerator.CreateMap(true);
            Player.ResetPlayer();

            //start messages
            MessageLog.Add("The rogue arrives on level 1", Colors.NormalMessage);
            MessageLog.Add($" level created with seed : ' {seed}' , Map Level : '{mapLevel}'", Colors.NormalMessage);

            StartGeneric();


        }

        public static void LoadGame()
        {
            saveLoad.LoadSeed();

            Random = new DotNetRandom(seed);
            //gen map
            MapGenerator mapGenerator = new MapGenerator(mapWidth, mapHeight, 20, 7, 14, mapLevel);
            DungeonMap = mapGenerator.CreateMap(false);
            saveLoad.LoadGame();

            DungeonMap.LoadEntities();

            StartGeneric();
        }

        public static void StartGeneric()
        {
            DungeonMap.UpdatePlayerFOV();

            playerInventory = new InventoryMenu(Player.inventory);
            shouldUpdateDraw = true;
            CurrentGameMode = GameMode.PLAYING;
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            if (CurrentGameMode == GameMode.MAINMENU)
            {
                HandleMainMenu();
            }
            else
            { 
                HandleGame();
            }
       
            // In OnRootConsoleUpdate() replace the if ( didPlayerAct ) block

        }
        public static void HandleMainMenu()
        {
            RLKeyPress keyPress = rootConsole.Keyboard.GetKeyPress();
            if (keyPress != null)
            {
                mainMenu.HandleInput(keyPress, menuConsole);
            }
        }

        public static void HandleGame()
        {
            bool didPlayerAct = false;
            RLKeyPress keyPress = rootConsole.Keyboard.GetKeyPress();
            HandleMouse();

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
                                DungeonMap = mapGenerator.CreateMap(true);
                                MessageLog = new MessageLog();
                                CommandSystem = new CommandSystem();
                                rootConsole.Title = $"RougeSharp RLNet Tutorial - Level {mapLevel}";
                                didPlayerAct = true;

                            }
                        }
                        else if (keyPress.Key == RLKey.I)
                        {

                            CurrentGameMode = GameMode.INVENTORY;
                            playerInventory.OnFirstEnter(menuConsole);
                        }
                        else if (keyPress.Key == RLKey.S)
                        {
                            MessageLog.Add("Game Saved!", Colors.HelpMessage);
                            saveLoad.SaveGame();
                            saveLoad.SaveSeed();
                        }
                        else if( keyPress.Key == RLKey.L)
                        {
                            saveLoad.LoadGame();
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
                            playerInventory.HandleInput(keyPress, menuConsole);

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
        }

        public static Action<int, int> targetCallback;
        public static Action<bool> targetCancelCallback;
        private static void HandleMouse()
        {
            int mx = rootConsole.Mouse.X;
            int my = rootConsole.Mouse.Y - lookHeight;

            if (rootConsole.Mouse.GetLeftClick())
            {
                if (CurrentGameMode == GameMode.TARGETING)
                {
                    if (DungeonMap.IsInFov(mx, my))
                    {
                        targetCallback.Invoke(mx, my);
                        CurrentGameMode = GameMode.PLAYING;
                        targetCallback = null;
                        targetCancelCallback = null;
                    }
                }
            }
            if (rootConsole.Mouse.GetRightClick())
            {
                if (CurrentGameMode == GameMode.TARGETING)
                {
                    targetCancelCallback.Invoke(true);
                    CurrentGameMode = GameMode.PLAYING;
                    targetCallback = null;
                    targetCancelCallback = null;
                }
            }
            if (mx > 0 && my > 0)
            {
                List<string> names = DungeonMap.InterigateEntityAtLocation(mx, my);
                if (names.Count > 0)
                {
                    lookConsole.Clear();
                    for (int i = 0; i < names.Count; i++)
                    {
                        lookConsole.Print(2, i, names[i], RLColor.White);
                    }
                }
                else
                {
                    lookConsole.Clear();
                }
            }
        }

        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {

            if (CurrentGameMode == GameMode.MAINMENU)
            {
                DrawMainMenu();
                return;

            }

            if (CurrentGameMode == GameMode.INVENTORY)
            {

                DrawInventory();
                return;

            }

            if (CurrentGameMode == GameMode.TARGETING)
            {
                DrawMainGame();
                return;

            }

            DrawMainGame();

        }

        private static void DrawMainMenu()
        {
        
            RLConsole.Blit(menuConsole, 0, 0, screenWidth, screenHeight - messageHeight, rootConsole, 0, 0);
            RLConsole.Blit(messageConsole, 0, 0, messageWidth, messageHeight, rootConsole, 0, screenHeight - messageHeight);

            rootConsole.Draw();
            mainMenu.Draw(menuConsole);

        }

        private static void DrawInventory()
        {
            mapConsole.Clear();
            statConsole.Clear();

            RLConsole.Blit(menuConsole, 0, 0, screenWidth, screenHeight - messageHeight, rootConsole, 0, 0);
            RLConsole.Blit(messageConsole, 0, 0, messageWidth, messageHeight, rootConsole, 0, screenHeight - messageHeight);

            rootConsole.Draw();
            playerInventory.Draw(menuConsole);
            MessageLog.Draw(messageConsole);

        }

        private static void DrawMainGame()
        {
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
                Player.DrawStats(statConsole, 0);
            }
        }

   
    }
}
