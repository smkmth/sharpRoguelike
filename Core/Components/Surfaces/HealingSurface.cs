using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.Surfaces
{
    public class HealingSurface : Surface
    {

        public override bool WalkOverSurface(Entity entity)
        {
            if (entity.attacker != null)
            {
                if (entity.attacker.Health != entity.attacker.MaxHealth)
                {
                    Game.MessageLog.Add($"{entity.name} walked across the healing liquid and felt a bit better", Colors.HealMessage);
                    entity.attacker.Health += 5;
                    if (entity.attacker.Health > entity.attacker.MaxHealth)
                    {
                        entity.attacker.Health = entity.attacker.MaxHealth;
                    }
                    return true;
                }

            }
            return false;

        }
    }
}
