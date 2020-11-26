using RLNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace sharpRoguelike.Core.Menus
{
    public enum MainMenuState
    {
        MAIN_MENU,
        ENTER_SEED
    }
    public class MainMenu
    {
        MainMenuState currentMenuState;
        bool loadpossible = false;
        public void OnFirstEnter()
        {
            currentMenuState = MainMenuState.MAIN_MENU;
            enteredSeed = "";
            if (File.Exists(Path.Combine(Game.saveLoad.savePathName, Game.saveLoad.saveName)))
            {
                loadpossible = true;
            }
        }

        public void Draw(RLConsole con)
        {
            con.Clear();
            switch (currentMenuState)
            {
                case MainMenuState.MAIN_MENU:
                    con.Print((con.Width / 2), (con.Height / 2) - 10, "ALCHYMIA", RLColor.White);
                    con.Print((con.Width / 2), (con.Height / 2) - 5, "Press Enter to start game", RLColor.White);
                    con.Print((con.Width / 2), (con.Height / 2) - 2, "Press S to enter a seed", RLColor.White);
                    if (loadpossible)
                    {
                        con.Print((con.Width / 2), (con.Height / 2) - 2, "Press L to load a game", RLColor.White);
                    }
                    break;

                case MainMenuState.ENTER_SEED:
                    con.Print((con.Width / 2), (con.Height / 2) - 5, "To enter a seed, type a set of numbers, then press enter. ", RLColor.White);
                    con.Print((con.Width / 2), (con.Height / 2) - 10, "You can press escape to go back. ", RLColor.White);
                    con.Print((con.Width / 2), (con.Height / 2), enteredSeed , RLColor.White);
                    break;
            }



        }

        public string enteredSeed;

        public void HandleInput(RLKeyPress keypress, RLConsole con)
        {
            switch (currentMenuState)
            {
                case MainMenuState.MAIN_MENU:
                    if (keypress.Key == RLKey.Enter)
                    {
                        Game.StartGame();
                        return;
                    }

                    char itemSelection = keypress.Key.ToString().ToCharArray()[0];
                    if (itemSelection == 'S')
                    {
                        currentMenuState = MainMenuState.ENTER_SEED;

                    }
                    if (itemSelection == 'L')
                    {
                        if (loadpossible)
                        {
                            Game.LoadGame();
                        }
                    }
                    break;
                case MainMenuState.ENTER_SEED:
                    if (keypress.Key == RLKey.Enter)
                    {
                        if (!string.IsNullOrEmpty(enteredSeed))
                        {

                            Game.StartGame(Int32.Parse(enteredSeed));
                        }
                        return;
                    }
                    else if (keypress.Key == RLKey.Escape)
                    {
                        enteredSeed = "";
                        currentMenuState = MainMenuState.MAIN_MENU;
                    }
                    else if (keypress.Key == RLKey.BackSpace)
                    {
                        if (enteredSeed.Length > 0)
                        {
                            enteredSeed = enteredSeed.Remove(enteredSeed.Length-1);
                        }
                    }
                    else
                    {
                        char num = keypress.Key.ToString().Last();
                        if (num > '0' &&  num < '9')
                        {
                            enteredSeed += num; 
                        }
                   
                        
                    }
                    break;
            }
     
        }

        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

    }
}
