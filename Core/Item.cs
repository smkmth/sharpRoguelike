﻿using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core
{
    //a description of a item in the world
    public class Item : Entity
    {
        public virtual bool Use(char useageChar, Actor user)
        {
            Game.MessageLog.Add($"Tried to use {name}");
            return false;
        }
    }
}
