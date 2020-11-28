using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.Surfaces
{
    [Serializable]
    class WaterSurface : Surface
    {
        public override bool WalkOverSurface(Entity entity)
        {
            return true;

        }
    }
}
