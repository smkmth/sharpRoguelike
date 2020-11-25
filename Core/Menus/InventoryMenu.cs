using RLNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Menus
{
    enum InventoryState
    {
        INVENTORY,
        SELECTED_ITEM

    }

    public class InventoryMenu
    {
        InventoryState currentInventoryState;
        Inventory inv;
        int offset = 5;

        Entity currentlySelectedItem;

        public InventoryMenu(Inventory _inv)
        {
            inv = _inv;

        }

        public void OnFirstEnter(RLConsole con)
        {
            con.Clear();
            currentInventoryState = InventoryState.INVENTORY;
            currentlySelectedItem = null;
        }

        public void Draw(RLConsole con)
        {
            con.Clear();

            con.Print(2, 0, "Inventory", RLColor.White);

            string backToGame = "(escape) back to game";
            con.Print(con.Width - backToGame.Length, 0, backToGame, RLColor.White);

            for (int i = 0; i < inv.storedItems.Count; i++)
            {
                RLColor color = RLColor.White;
                if(currentlySelectedItem != null && inv.storedItems[i] == currentlySelectedItem)
                {
                    color = RLColor.Yellow;
                }
                con.Print(2, i + offset, "(" + Convert.ToChar(i + 97).ToString() + ") " +  inv.storedItems[i].name, color);

            }

            if (currentInventoryState == InventoryState.SELECTED_ITEM)
            {
                string itemname = $"Selected: {currentlySelectedItem.name}";
                con.Print(con.Width - itemname.Length, 2, itemname, RLColor.White);

                string useItem = $" (a) use item ";
                con.Print(con.Width - useItem.Length, 4, useItem, RLColor.White);

                string dropItem = $" (d) drop item ";
                con.Print(con.Width - dropItem.Length, 6, dropItem, RLColor.White);

                string goBack = "(backspace) go back to inventory selection ";
                con.Print(con.Width - goBack.Length, 8, goBack, RLColor.White);
            }
        }

        public void HandleInput(RLKeyPress keypress, RLConsole con)
        {
            if (currentInventoryState == InventoryState.INVENTORY)
            {
               
                //interprit selection as item choice
                char itemSelection = keypress.Key.ToString().ToCharArray()[0];
                int itemindex = Convert.ToInt32(itemSelection - 65);

                if (itemindex >= 0 && itemindex < inv.storedItems.Count)
                {
                    con.Clear();

                    currentlySelectedItem = inv.storedItems[itemindex];
                    currentInventoryState = InventoryState.SELECTED_ITEM;
                    return;
                }

            }

            if (currentInventoryState == InventoryState.SELECTED_ITEM)
            {
                if (keypress.Key == RLKey.BackSpace)
                {
                    con.Clear();
                    currentInventoryState = InventoryState.INVENTORY;
                    currentlySelectedItem = null;
                }


                //interprit selection as selection
                char useSelection = keypress.Key.ToString().ToCharArray()[0];

                if(useSelection == 'D')
                {
                    inv.DropItem(currentlySelectedItem);
                    currentlySelectedItem = null;
                    currentInventoryState = InventoryState.INVENTORY;

                    return;
                }
                else
                {   
                    if (currentlySelectedItem.effect.Use(useSelection, inv.owner, inv.owner ))
                    {
                        inv.ConsumeItem(currentlySelectedItem);
                        currentlySelectedItem = null;
                        currentInventoryState = InventoryState.INVENTORY;
                    }
                    return;
                }
                    
                
                }

            }


        }

    }
}
