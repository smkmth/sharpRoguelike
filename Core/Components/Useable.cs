using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    //a description of a item in the world
    public class Useable
    {
        public Entity owner;
        public virtual bool Use(char useageChar, Entity user, Entity target = null)
        {
            Game.MessageLog.Add($"Tried to use {owner.name}");
            return false;
        }
    }
}
