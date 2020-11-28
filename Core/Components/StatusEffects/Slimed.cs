using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.StatusEffects
{
    [Serializable]
    class Slimed : StatusEffect
    {
        public Slimed(Entity _owner, int _counter, StatusEffectsComp _comp) :base (_owner, _counter, _comp)
        {
            statusEffectAppliedName = "slimed";

        }

        public override void ApplyEffect()
        {
            if (counter == start_counter)
            {
                if (owner.actor != null)
                {
                    owner.actor.speedModifier += 15;
                     
                }
               
            }
            else
            {
                if (owner.attacker != null)
                {
                    owner.attacker.Health -= 2;
                }
            }
        }

        public override void RemoveEffect()
        {
            base.RemoveEffect();
            if (owner.actor != null)
            {
                owner.actor.speedModifier -= 15;
            }
        }
    }
}
