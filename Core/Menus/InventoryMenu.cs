using RLNET;
using sharpRoguelike.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sharpRoguelike.Core.Menus
{
    enum InventoryState
    {
        INVENTORY,
        SELECTED_ITEM,
        SELECTED_EQUIPMENT,
        SELECT_INVENTORY
    }

    public class InventoryMenu
    {
        InventoryState currentInventoryState;
        List<Inventory> posInventories;
        public Inventory inv;
        int offset = 5;
        List<EquipmentSlot> equip;
        Entity currentlySelectedItem;
        EquipmentSlot currentlySelectedSlot;
        bool enemy_inventory = false;


        public void OpenInventorySelect(RLConsole con, List<Inventory> _posInventories)
        {
            posInventories = _posInventories;
            currentInventoryState = InventoryState.SELECT_INVENTORY;
            con.Clear();
        }

        public void OpenInventory(RLConsole con, Inventory _inv, List<EquipmentSlot> _equip = null, bool _enemy_inventory=false)
        {

            inv = _inv;
            if (_equip != null)
            {
                equip = _equip;
            }
            else
            {
                equip = null;
            }
            
            enemy_inventory = _enemy_inventory;
            
            con.Clear();
            currentInventoryState = InventoryState.INVENTORY;
            currentlySelectedItem = null;
            currentlySelectedSlot = null;
        }

        public void Draw(RLConsole con, int width)
        {
            con.Clear();

            if(currentInventoryState == InventoryState.SELECT_INVENTORY)
            {
                con.Print(2, 0, "Select Inventory", RLColor.White);
                con.Print(4, 0, "multiple inventories are under this tile, select which one to open.", RLColor.White);

                for (int i = 0; i < posInventories.Count; i++)
                {
                    RLColor color = RLColor.White;
                    string inventoryClear = "";
                    if (posInventories[i].storedItems.Count == 0)
                    {
                        inventoryClear = " (empty)";
                    }
                    con.Print(2, i + 8, "(" + Convert.ToChar(i + 97).ToString() + ") " + posInventories[i].owner.name + inventoryClear , color);
                }
                return;
            }

            con.Print(2, 0, "Inventory", RLColor.White);

            int itemOffset = offset;

            string backToGame = "(escape) back to game";
            con.Print(width - backToGame.Length, 0, backToGame, RLColor.White);

            for (int i = 0; i < inv.storedItems.Count; i++)
            {
                RLColor color = RLColor.White;
                if(currentlySelectedItem != null && inv.storedItems[i] == currentlySelectedItem)
                {
                    color = RLColor.Yellow;
                }
                con.Print(2, i + offset, "(" + Convert.ToChar(i + 97).ToString() + ") " +  inv.storedItems[i].name, color);
                itemOffset++;
            }
            int equipmentOffset = offset;
            if (equip != null)
            {

                con.Print(2, offset + itemOffset, "Equipment ",RLColor.White); ;

                for (int i = 0; i < equip.Count; i++)
                {
                    RLColor color = RLColor.White;
                    if (currentlySelectedSlot != null && equip[i] == currentlySelectedSlot)
                    {
                        color = RLColor.Yellow;
                    }

                    if (equip[i].attachedEquipment != null)
                    {
                        con.Print(2, i + offset + itemOffset + equipmentOffset, "(" + i + ")" + equip[i].equipType.ToString() +": " +
                            equip[i].attachedEquipment.ownerItem.name, color);
                    }
                    else 
                    {
                        con.Print(2, i + offset + itemOffset + equipmentOffset, "(" + i + ")" + equip[i].equipType.ToString() + ": Nothing", color);
                    }

                }
            }

            if (currentInventoryState == InventoryState.SELECTED_ITEM)
            {
                int index = 2;

                string itemname = $"Selected: {currentlySelectedItem.name}";
                con.Print(width - itemname.Length, index, itemname, RLColor.White);
                index+=2;

                
                if (currentlySelectedItem.effect != null)
                {
                    for (int i = 0; i< currentlySelectedItem.effect.usageChars.Count; i++ )
                    {
                        string useItem = $" ({currentlySelectedItem.effect.displayChars[i]}) {currentlySelectedItem.effect.usageNames[i] } ";
                        con.Print(width - useItem.Length, index, useItem, RLColor.White);
                        index += 2;
                    }
                }

                string dropItem = $" (d) drop item ";
                con.Print(width - dropItem.Length, index, dropItem, RLColor.White);
                index += 2;

                if (enemy_inventory)
                {
                    string takeItem = $" (g) take item ";
                    con.Print(width - dropItem.Length, index, takeItem, RLColor.White);
                    index += 2;
                }

                string goBack = "(backspace) go back to inventory selection ";
                con.Print(width - goBack.Length, index, goBack, RLColor.White);
                index += 2;

            }

            if (currentInventoryState == InventoryState.SELECTED_EQUIPMENT)
            {
                int index = 2;
                string equipSlotName = $"Selected: {currentlySelectedSlot.equipType.ToString()}";
                con.Print(width - equipSlotName.Length, index, equipSlotName, RLColor.White);
                index += 2;

                //if we have somthing in that slot...
                if (currentlySelectedSlot.attachedEquipment != null)
                {
                    Entity ownerItem = currentlySelectedSlot.attachedEquipment.ownerItem;
                    
                    string equipment = $"Currently Equipped: {ownerItem.name}";
                    con.Print(width - equipment.Length, index, equipment, RLColor.White);
                    index += 2;


                    if (ownerItem.effect != null)
                    {
                        for (int i = 0; i < ownerItem.effect.usageChars.Count; i++)
                        {
                            string useItem = $"({ownerItem.effect.displayChars[i]})" + $"{ownerItem.effect.usageNames[i] } ";
                            con.Print(width - useItem.Length, index, useItem, RLColor.White);
                            index += 2;
                        }
                    }

                    string equipmentDescription = $"{ownerItem.description}";
                    con.Print(width - equipmentDescription.Length, index, equipmentDescription, RLColor.White);
                    index += 2;
                }
                else
                {
                    int itemIndex = 0;
                    //we dont have somthing in that slot - offer stuff?
                    for (int i = 0; i < inv.storedItems.Count; i++)
                    {
                        if (inv.storedItems[i].equipment != null)
                        {
                            if (inv.storedItems[i].equipment.slotType == currentlySelectedSlot.equipType)
                            {
                                string itemSuggestion = "(" + Convert.ToChar(i + 97).ToString() + ") " + inv.storedItems[i].name;
                                con.Print(width - itemSuggestion.Length, index, itemSuggestion, RLColor.White);
                                itemIndex++;
                                index += 2;
                            }
                        }

                    }
                }
                string goBack = "(backspace) go back to inventory selection ";
                con.Print(width - goBack.Length, index, goBack, RLColor.White);
                index += 2;

            }
        }

        public void HandleInput(RLKeyPress keypress, RLConsole con)
        {
            char useSelection = keypress.Key.ToString().ToCharArray().Last();
            if (currentInventoryState == InventoryState.SELECT_INVENTORY)
            {
                int itemindex = Convert.ToInt32(useSelection - 65);

                if (itemindex >= 0 && itemindex < posInventories.Count)
                {

                    con.Clear();
                    OpenInventory(con,posInventories[itemindex], null, true);

                    return;
                }

            }
            if (currentInventoryState == InventoryState.INVENTORY)
            {
                //if we get a number-  we want to select some equipment
                if (useSelection >= '0' && useSelection < '9')
                {
                    int equipIndex = Convert.ToInt32(useSelection.ToString());

                    if (equipIndex >= 0 && equipIndex < equip.Count)
                    {
                        currentlySelectedSlot = equip[equipIndex];
                        currentInventoryState = InventoryState.SELECTED_EQUIPMENT;
                        return;
                    }
               
                }

                //otherwise selection as item choice
                int itemindex = Convert.ToInt32(useSelection - 65);

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
                    return;
                }
                if (keypress.Key == RLKey.Down || keypress.Key == RLKey.Up || keypress.Key == RLKey.Left || keypress.Key == RLKey.Right)
                {
                    return;
                }

                //interprit selection as selection
                if(useSelection == 'D')
                {
                    inv.DropItem(currentlySelectedItem);
                    currentlySelectedItem = null;
                    currentInventoryState = InventoryState.INVENTORY;
                    return;
                }
                else if (useSelection == 'G' && enemy_inventory)
                {
                    inv.ConsumeItem(currentlySelectedItem);
                    Game.Player.inventory.AddItem(currentlySelectedItem);
                }
                else
                {   
                    if (currentlySelectedItem.effect != null && currentlySelectedItem.effect.usageChars.Count != 0)
                    {
                           
                        if (currentlySelectedItem.effect.Use( useSelection ,inv.owner, inv.owner))
                        {
                            inv.ConsumeItem(currentlySelectedItem);
                            currentlySelectedItem = null;
                            currentInventoryState = InventoryState.INVENTORY;
                        }
           
                        
                    }
                    return;
                }
               
                    
                
            }

            if (currentInventoryState == InventoryState.SELECTED_EQUIPMENT)
            {
                if (keypress.Key == RLKey.BackSpace)
                {
                    con.Clear();
                    currentInventoryState = InventoryState.INVENTORY;
                    currentlySelectedSlot = null;
                    return;
                }
                if (keypress.Key == RLKey.Down || keypress.Key == RLKey.Up || keypress.Key == RLKey.Left || keypress.Key == RLKey.Right)
                {
                    return;
                }
                else
                {
                    if (currentlySelectedSlot.attachedEquipment != null)
                    {

                        Entity equipmentItem = currentlySelectedSlot.attachedEquipment.ownerItem;
                    
                        if (equipmentItem.effect != null && equipmentItem.effect.usageChars.Count != 0)
                        {
                            //if the item is equipment - returning true will equip, false will unequip.
                            if (!equipmentItem.effect.Use(useSelection, inv.owner, inv.owner))
                            {
                                inv.AddItem(equipmentItem);
                                currentlySelectedSlot = null;
                                currentInventoryState = InventoryState.INVENTORY;
                                return;
                            }

                        }
                        return;
                    }
                    else
                    {
                        int itemindex = Convert.ToInt32(useSelection - 65);

                        if (itemindex >= 0 && itemindex < inv.storedItems.Count)
                        {
                            if (inv.storedItems[itemindex].equipment != null && inv.storedItems[itemindex].equipment.slotType == currentlySelectedSlot.equipType)
                            {
                               if ( inv.storedItems[itemindex].effect.Use('A', inv.owner, inv.owner))
                                {
                                    inv.ConsumeItem(inv.storedItems[itemindex]);
                                    currentInventoryState = InventoryState.INVENTORY;
                                    currentlySelectedSlot = null;
                                    return;
                                }
                            }
                            return;
                        }

                    }

                }

            }

        }


    }

}

