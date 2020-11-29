using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.Surfaces
{
    [Serializable]
    class WaterSurface : Surface
    {
        public WaterSurface()
        {
            surfaceResistance = 20;
        }

        public override bool WalkOverSurface(Entity entity)
        {
            return true;

        }
    }
}
