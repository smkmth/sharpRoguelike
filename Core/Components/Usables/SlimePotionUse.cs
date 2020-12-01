using sharpRoguelike.Core.Components.StatusEffects;
using sharpRoguelike.Core.Components.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.Usables
{
    [Serializable]
    class SlimePotionUse : PotionUse
    {


        public override bool DrinkEffect(Entity user, Entity target)
        {
            target.attacker.Health -= 20;
            Game.MessageLog.Add("You swig down the slime potion, it tastes foul - and you feel sick", Colors.NormalMessage);

            target.statusEffectHolder.AddStatusEffect(new Slimed(target, 5, StatusEffectsComp.Slimed));
            return true;
        }

        public override void ResolveThrow(int x, int y)
        {
            Game.MessageLog.Add("The potion bottle smashes on the ground - spilling yellow slime everywhere!", Colors.NormalMessage);


            SlimeSurface surface = new SlimeSurface();
            Game.DungeonMap.CreateSurface(x, y, 4, "Slime", Colors.SlimehulkColor, '~', surface);
        }


    }
}
