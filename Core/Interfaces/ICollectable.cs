using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Interfaces
{
    interface ICollectable
    {
        bool Collect(RLConsole console, IMap imap);
    }
}
