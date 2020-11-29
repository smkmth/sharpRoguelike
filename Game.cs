using System;
using System.Collections.Generic;
using RLNET;
using RogueSharp.Random;
using sharpRoguelike.Core;
using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Menus;
using sharpRoguelike.Core.Systems;

namespace sharpRoguelike
{
    //the main class - and yeah this is a mess sorry lol. everything here is static 
    //so i can access it everywhere - and you might be saying - hey doesnt that lead to 
    //data getting modified all over the place? and - yeah kind of but like its fine
    //for now. 

    //when running the game make sure to tweak the scale factor - because this api shits
    //the bed when it comes to screen res - not found a good solution - just fuck around with the 
    //value untill you can see the whole console - the stat bar the message console ect.

    //class is split mostly between draw stuff at the bottom - update stuff (getting keys) 
    //in the middle, start game stuff towards the top and the entry point at the v.top
    public static class Game
    {

        public static readonly int screenWidth = 150;
        public static readonly int screenHeight = 80;
        private static RLRootConsole rootConsole;
        

        private static readonly int messageWidth = 40;
        private static readonly int messageHeight = screenHeight;
        private static RLConsole messageConsole;

        private static readonly int lookWidth = screenWidth - 40;
        private static readonly int lookHeight = 11;
        private static RLConsole lookConsole;

        private static readonly int statWidth = screenWidth - messageWidth;
        private static readonly int statHeight = 11;
        private static RLConsole statConsole;


        private static readonly int mapWidth = screenWidth - messageWidth;
        private static readonly int mapHeight = screenHeight - (statHeight + lookHeight);
        private static RLConsole mapConsole;

        private static RLConsole menuConsole;
        
        public static Entity Player { get;  set; }
        
        public static SaveLoadSystem saveLoad { get; private set; }

        public static InventoryMenu playerInventory { get; private set; }
        public static StatDisplay statDisplay { get; private set; }
        public static MainMenu mainMenu { get; private set; }
        public static CommandSystem CommandSystem { get; private set; }
        public static DungeonMap DungeonMap { get; private set; }
        public static MessageLog MessageLog { get; set; }
        public static SchedulingSystem SchedulingSytem { get; private set; }
        public static RandomWrapper Random { get; set; }

        public static bool fullScreen = false;
        public static bool didPlayerAct;
        public static bool shouldUpdateDraw =true;
        public static int steps;
        public static int mapLevel =1;
        public static GameMode CurrentGameMode;
        public static int seed;
        public static float scaleFactor = 1f;

        public static bool DebugCheats = true;
        public static bool wallhack = false;
        public static bool seeallwalls = false;
        public static bool seeallentities = false;

        static void Main(string[] args)
        {
            //get font file from folder dir
            string fontFileName = "terminal8x8.png";
            string consoleTitle = "alchymia";

            rootConsole = new RLRootConsole(fontFileName, screenWidth, screenHeight, 8,8, scaleFactor, consoleTitle);
            rootConsole.SetWindowState(RLWindowState.Maximized);
          
            CurrentGameMode = GameMode.MAINMENU;

            // new all the stuff that needs newing 

            saveLoad = new SaveLoadSystem();
            mapConsole = new RLConsole(mapWidth, mapHeight);
            messageConsole = new RLConsole(messageWidth, messageHeight);
            lookConsole = new RLConsole(lookWidth, lookHeight);
            statConsole = new RLConsole(statWidth, statHeight);
            menuConsole = new RLConsole(screenWidth, screenHeight);
            CommandSystem = new CommandSystem();
            SchedulingSytem = new SchedulingSystem();
            MessageLog = new MessageLog(messageWidth);
            mainMenu = new MainMenu();
            statDisplay = new StatDisplay();

            //set up main menu
            mainMenu.OnFirstEnter();

            //attach all the hooks to the console update and render
            rootConsole.Render += OnRootConsoleRender;
            rootConsole.Update += OnRootConsoleUpdate;

            //all downhill from here
            rootConsole.Run();


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

        }

        public static void HandleMainMenu()
        {
            shouldUpdateDraw = true;
            RLKeyPress keyPress = rootConsole.Keyboard.GetKeyPress();
            if (keyPress != null)
            {
                mainMenu.HandleInput(keyPress, menuConsole);
            }
        }
        public static void StartGame(int? enter_seed = null)
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

            Random = new RandomWrapper(new DotNetRandom(seed));
            //gen map
            RandomRoomsMapGenerator mapGenerator = new RandomRoomsMapGenerator(mapWidth, mapHeight, 20, 7, 14, mapLevel);
            DungeonMap = mapGenerator.CreateMap(true);
            Player.player.ResetPlayer();

            //start messages
            MessageLog.Add($"Level created with seed : ' {seed}' , Map Level : '{mapLevel}'", Colors.NormalMessage);

            StartGeneric();


        }

        public static void LoadGame()
        {
            
            saveLoad.LoadSeed();

            //gen map
            //MapGenerator mapGenerator = new MapGenerator(mapWidth, mapHeight, 20, 7, 14, mapLevel);
            DungeonMap = new DungeonMap();
            DungeonMap.Initialize(mapWidth, mapHeight);
            saveLoad.LoadGame();
            DungeonMap.LoadEntities();
            
            StartGeneric();
        }

        //always do this - if we are loading or starting fresh
        public static void StartGeneric()
        {
            DungeonMap.UpdatePlayerFOV();

            playerInventory = new InventoryMenu();
            CurrentGameMode = GameMode.PLAYING;
            MessageLog.Add($"The rogue arrives on level {mapLevel} ", Colors.NormalMessage);

        }

        public static void ResetGame()
        {
            
            CurrentGameMode = GameMode.MAINMENU;
            mainMenu.OnFirstEnter();
            statConsole.Clear();
            MessageLog.Clear();
        }


        //get keyboard input while in game
        public static void HandleGame()
        {
            shouldUpdateDraw = false;

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
                            MessageLog.Add("Game Saved!", Colors.HelpMessage);
                            saveLoad.SaveGame();
                            saveLoad.SaveSeed();
                            rootConsole.Close();
                        }
                        else if (keyPress.Key == RLKey.G)
                        {
                            Entity pickup = DungeonMap.GetItemAt(Player.x, Player.y);
                            if (pickup != null)
                            {
                                if (Player.inventory.AddItem(pickup))
                                {
                                    DungeonMap.RemoveItem(pickup);
                                    didPlayerAct = true;
                                }
                            }
                           
                        }
                        else if (keyPress.Key == RLKey.S)
                        {
                            CurrentGameMode = GameMode.INVENTORY;
                            List<Entity> checkThis = DungeonMap.GetAllEntitiesAt(Player.x,Player.y);
                            List<Inventory> inventories = new List<Inventory>();
                            foreach(Entity entity in checkThis)
                            {
                                if(entity.player == null && entity.inventory != null)
                                {
                                    inventories.Add(entity.inventory);
                                }
                            }
                            if (inventories.Count == 1)
                            {
                                playerInventory.OpenInventory(menuConsole, inventories[0],null,  true);
                                
                            }
                            else if (inventories.Count > 1)
                            {
                                playerInventory.OpenInventorySelect(menuConsole, inventories);

                            }
                        }
                        else if (keyPress.Key == RLKey.F)
                        {
                            Equipment rangedWeapon = Player.GetEquippedRangedWeapon();
                            if (rangedWeapon != null && rangedWeapon.ranged != null)
                            {
                                MessageLog.Add(rangedWeapon.ranged.targetString, Colors.HelpMessage);
                                targetCallback = rangedWeapon.ranged.TargetCallback;
                                targetCancelCallback = rangedWeapon.ranged.TargetCancelledCallback;
                                CurrentGameMode = GameMode.TARGETING;
                                
                            }
                        }
                        else if (keyPress.Key == RLKey.Period)
                        {
                            if (DungeonMap.CanMoveDownToNextLevel())
                            {
                                RandomRoomsMapGenerator mapGenerator = new RandomRoomsMapGenerator(mapWidth, mapHeight, 20, 7, 13, ++mapLevel);
                                DungeonMap = mapGenerator.CreateMap(true);
                                MessageLog = new MessageLog(messageWidth);
                                CommandSystem = new CommandSystem();
                                MessageLog.Add($"Rogue travels down the stairs to level {mapLevel}",Colors.NormalMessage);
                                rootConsole.Title = $"RougeSharp RLNet Tutorial - Level {mapLevel}";
                                didPlayerAct = true;

                            }
                        }
                        else if (keyPress.Key == RLKey.I)
                        {

                            CurrentGameMode = GameMode.INVENTORY;
                            playerInventory.OpenInventory(menuConsole, Player.inventory, Player.equipmentSlots);
                        }
                        else if( keyPress.Key == RLKey.L)
                        {
                            saveLoad.LoadGame();
                        }
                        if (DebugCheats)
                        {
                            if (keyPress.Key == RLKey.E)
                            {
                                seeallwalls = !seeallwalls;
                                DungeonMap.SeeAllWalls(seeallwalls);
                                DungeonMap.UpdatePlayerFOV();
                            }
                            else if (keyPress.Key == RLKey.Q)
                            {
                                seeallentities = !seeallentities;
                                DungeonMap.SeeAllEntities(seeallentities);
                                DungeonMap.UpdatePlayerFOV();

                            }
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
                CommandSystem.HandleTurnOrder();
                shouldUpdateDraw = true;
            }
            shouldUpdateDraw = true;                            //TODO figure out why this is being funny

        }

        public static Action<int, int> targetCallback;          //subscibe to this when in targeting mode - invoke when you have selected the tile
        public static Action<bool> targetCancelCallback;        //subscribe to this when in targeting mode - invoke if player cancels

        //get mouse input when in game
        private static void HandleMouse()
        {
            int mx = rootConsole.Mouse.X;
            int my = rootConsole.Mouse.Y - lookHeight;          //mouse is messured from top left of screen so the look console offsets this value 


            if (CurrentGameMode == GameMode.TARGETING) 
            {
                if (rootConsole.Mouse.GetLeftClick())
                {
               
                    if (DungeonMap.IsInFov(mx, my))
                    {
                        targetCallback.Invoke(mx, my);
                        CurrentGameMode = GameMode.PLAYING;
                        targetCallback = null;
                        targetCancelCallback = null;
                        CommandSystem.EndPlayerTurn();

                    }
                
                }
                if (rootConsole.Mouse.GetRightClick())
                {
                
                        targetCancelCallback.Invoke(true);
                        CurrentGameMode = GameMode.PLAYING;
                        targetCallback = null;
                        targetCancelCallback = null;
                
                }
            }
            //tell us what entities are under the mouse pointer
            if (mx > 0 && my > 0)
            {
                List<Entity> entities = DungeonMap.InterigateEntityAtLocation(mx, my);
                if (entities.Count > 0)
                {
                    lookConsole.Clear();
                    for (int i = 0; i < entities.Count; i++)
                    {
                        string nametoprint = entities[i].name;
                        lookConsole.Print(2, i, entities[i].name, RLColor.White);
                        if (!string.IsNullOrEmpty(entities[i].description))
                        {
                            lookConsole.Print(nametoprint.Length + 2, i, ": " + entities[i].description, RLColor.White);
                        }
        
                    }
                }
                else
                {
                    lookConsole.Clear();
                }

            }
        }

        //render function - called every frame - game works using mutliple
        //consoles that each draw to a main console - flow should go - clear
        //all subconsoles - draw subconsoles - draw root console -blit 
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
                //weird placement - but yeah sort this out later
                Equipment rangedWeapon = Player.GetEquippedRangedWeapon();
                if (rangedWeapon != null && rangedWeapon.ranged != null)
                {
                    string gunString = "Aiming with " + rangedWeapon.ownerItem.name + " you have " + rangedWeapon.ranged.ammo + " shots ";
                    lookConsole.Print(lookConsole.Width - gunString.Length, 2, gunString, RLColor.White);
                }
                DrawMainGame();
                return;

            }

            DrawMainGame();

        }

        private static void DrawMainMenu()
        {
        
            mainMenu.Draw(menuConsole);
            rootConsole.Draw();
            RLConsole.Blit(menuConsole, 0, 0, screenWidth, screenHeight, rootConsole, 0, 0);
            RLConsole.Blit(statConsole, 0, 0, statWidth, statHeight, rootConsole, 0, screenHeight );

        }

        private static void DrawInventory()
        {
            mapConsole.Clear();
            messageConsole.Clear();


            MessageLog.Draw(messageConsole);
            playerInventory.Draw(menuConsole, screenWidth -(messageWidth + 2));
            rootConsole.Draw();

            RLConsole.Blit(messageConsole, 0, 0, messageWidth, messageHeight, rootConsole, mapWidth, 0);
            RLConsole.Blit(menuConsole, 0, 0, screenWidth- messageWidth, screenHeight , rootConsole, 0, 0);


        }

        private static void DrawMainGame()
        {

            if (shouldUpdateDraw)
            {

                mapConsole.Clear();
                messageConsole.Clear();
                statConsole.Clear();


                DungeonMap.Draw(mapConsole, statConsole);
                MessageLog.Draw(messageConsole);
                Player.Draw(mapConsole, DungeonMap);
                statDisplay.DrawPlayer(statConsole, Player);
                rootConsole.Draw();
            }
            // Blit the sub consoles to the root console in the correct locations
            RLConsole.Blit(mapConsole, 0, 0, mapWidth, mapHeight, rootConsole, 0, lookHeight);
            RLConsole.Blit(messageConsole, 0, 0, messageWidth, messageHeight, rootConsole, mapWidth , 0);
            RLConsole.Blit(statConsole, 0, 0, statWidth, statHeight, rootConsole, 0, screenHeight - statHeight);
            RLConsole.Blit(lookConsole, 0, 0, lookWidth, lookHeight, rootConsole, 0, 0);

        }

   
    }
}
