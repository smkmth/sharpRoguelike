using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.Surfaces
{ 
    [Serializable]
    class TrapSurface : Surface
    {
        public Entity setter;

        public TrapSurface(Entity _setter)
        {
            setter = _setter;
            surfaceResistance = 50;
        }

        public override bool WalkOverSurface(Entity entity)
        {
            if (entity == setter)
            {
                Game.MessageLog.Add($"{entity.name} nimberly avoid their trap", Colors.NormalMessage);
            }
            else
            {
                Game.MessageLog.Add($"The trap springs, damaging {entity.name} for 10 health", Colors.NormalMessage);
                entity.attacker.Health -= 10;
            }
            return base.WalkOverSurface(entity);
        }

    }
}
