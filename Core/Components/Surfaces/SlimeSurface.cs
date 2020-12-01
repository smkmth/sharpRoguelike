using sharpRoguelike.Core.Components.StatusEffects;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.Surfaces
{
    [Serializable]
    class SlimeSurface : Surface
    {
        public SlimeSurface()
        {
            surfaceResistance = 60;
        }
        public override bool WalkOverSurface(Entity entity)
        {
            return entity.statusEffectHolder.AddStatusEffect(new Slimed(entity, 5, StatusEffectsComp.Slimed));

        }
    }
}
