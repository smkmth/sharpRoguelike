using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    //a description of a item in the world
    [Serializable]
    public class Useable
    {
        public Entity owner;
        public List<char> usageChars;
        public List<char> displayChars;
        public List<string> usageNames;

        public bool throwable;      //you can throw this object
        public bool targetable;     //can the effect be targeted
        public bool consume;        //does the effect consume the item

        
        public virtual bool Use(char UseLike, Entity user, Entity target = null)
        {
            Game.MessageLog.Add($"Tried to use {owner.name}", Colors.HelpMessage);
            return false;

        }

    }
}
