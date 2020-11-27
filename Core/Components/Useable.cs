using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    //does an item have a 'use' when used in a inventory menu
    //any classes deriving from this have some kind of function 
    //when a inventory menu passes a derived class a char- it can
    //decide what it does after that point.
    [Serializable]
    public class Useable
    {
        public Entity owner;
        public List<char> usageChars;
        public List<char> displayChars;
        public List<string> usageNames;

        
        public virtual bool Use(char UseLike, Entity user, Entity target = null)
        {
            Game.MessageLog.Add($"Tried to use {owner.name}", Colors.HelpMessage);
            return false;

        }

    }
}
