using sharpRoguelike.Core.Components.StatusEffects;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.Surfaces
{
    class SlimeSurface : Surface
    {
        public override bool WalkOverSurface(Entity entity)
        {
            return entity.AddStatusEffect(new Slimed(entity, 5, StatusEffectsComp.Slimed));

        }
    }
}
