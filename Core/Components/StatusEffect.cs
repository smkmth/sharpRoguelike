using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{   
    //this sucks figure somthing beter laters (immunities) TODO
    public enum StatusEffectsComp
    {
        Slimed

    }
    [Serializable]
    public class StatusEffect
    {
        public Entity owner;
        public int counter;
        public int start_counter;
        public StatusEffectsComp comp;
        public string statusEffectAppliedName;

        public StatusEffect(Entity _owner, int _counter, StatusEffectsComp _comp)
        {
            owner = _owner;
            counter = _counter;
            start_counter = _counter;
            comp = _comp;
            
        }

        public virtual void ApplyEffect()
        {

        }

        public virtual void DecreaseCounter()
        {
            counter--;
            if (counter <= 0)
            {
                RemoveEffect();
            }
        }

        //actually remove effect from owner
        public virtual void RemoveEffect() 
        {
            ResolveEffect();
            owner.statusEffects.Remove(this);
        }

        //reverse effects of status effect 
        public virtual void ResolveEffect()
        {

        }
    }
}
