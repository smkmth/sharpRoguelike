﻿using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.Surfaces
{
    [Serializable]
    class WaterSurface : Surface
    {
        public WaterSurface()
        {
            surfaceResistance = 100;
        }

        public override bool WalkOverSurface(Entity entity)
        {
            return true;

        }
    }
}
