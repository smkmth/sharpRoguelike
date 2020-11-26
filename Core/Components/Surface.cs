using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    public class Surface
    {
        public Entity owner;
        public virtual bool WalkOverSurface(Entity entity)
        {
            return false;
        }
    }
}
