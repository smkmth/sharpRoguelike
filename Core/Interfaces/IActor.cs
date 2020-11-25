using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Interfaces
{
    interface IActor
    {
        string name { get; set; }
        int awareness { get; set; }
    }
}
