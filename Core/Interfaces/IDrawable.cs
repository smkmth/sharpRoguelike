using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Interfaces
{
    interface IDrawable
    {
        RLColor color { get; set; }
        char symbol { get; set; }
        int x { get; set; }
        int y { get; set; }

        void Draw(RLConsole console, IMap imap);
    }
}
