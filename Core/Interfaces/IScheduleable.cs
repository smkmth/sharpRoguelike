using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Interfaces
{
   public interface IScheduleable
    {
        int Time { get; }

        Entity owner { get; }
    }
}
