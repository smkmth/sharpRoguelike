using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    //if the entity is a surface - it will do the inhereted walk over surface method
    //when another entity - walks over it - the bool return asks if the entity should be 
    //destroyed after walking over it - so returning true destroys it - false doesnt
    [Serializable]
    public class Surface
    {
        public Entity owner;
        public int surfaceResistance;

        public virtual bool WalkOverSurface(Entity entity)
        {
            return false;
        }
    }
}
