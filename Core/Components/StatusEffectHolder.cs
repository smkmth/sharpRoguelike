using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    public class StatusEffectHolder
    {
        public Entity owner;
        public List<StatusEffect> statusEffects;                //is this entity under a status effect 
        public List<StatusEffectsComp> immuneStatusEffects;     //is this entity immune from a status effect 

        public StatusEffectHolder(Entity _owner)
        {
            owner = _owner;
            statusEffects = new List<StatusEffect>();
        }
        public void ApplyStatusEffects()
        {
            if (statusEffects == null)
            {
                return;
            }

            for (int i = 0; i < statusEffects.Count; i++)
            {
                statusEffects[i].ApplyEffect();
                statusEffects[i].DecreaseCounter();

            }
        }
        public void RemoveAllStatusEffects()
        {

            if (statusEffects != null)
            {

                foreach (StatusEffect currentEffect in statusEffects)
                {
                    currentEffect.ResolveEffect();

                }
                statusEffects.Clear();
            }

        }
        public void RemoveStatusEffect(StatusEffectsComp effect)
        {
            if (statusEffects != null)
            {
                StatusEffect targetEffect = null;

                foreach (StatusEffect currentEffect in statusEffects)
                {
                    if (effect == currentEffect.comp)
                    {
                        currentEffect.ResolveEffect();
                        targetEffect = currentEffect;
                    }

                }

                statusEffects.Remove(targetEffect);
            }

        }

        public bool AddStatusEffect(StatusEffect effect)
        {
            if (statusEffects == null)
            {
                statusEffects = new List<StatusEffect>();
            }

            if (immuneStatusEffects != null)
            {
                foreach (StatusEffectsComp immune_effect in immuneStatusEffects)
                {
                    if (effect.comp == immune_effect)
                    {
                        return false;
                    }

                }
            }

            var dupes = statusEffects.Where(p => p.comp == effect.comp);
            if (dupes.Count() <= 0)
            {
                statusEffects.Add(effect);
                return true;
            }
            return false;
        }

    }
}
