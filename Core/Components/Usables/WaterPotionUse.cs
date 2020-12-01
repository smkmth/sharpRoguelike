using sharpRoguelike.Core.Components.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.Usables
{
    [Serializable]
    class WaterPotionUse : PotionUse
    {
        public override bool DrinkEffect(Entity user, Entity target)
        {

            Game.MessageLog.Add("You swig down the water potion, its refreshing.", Colors.NormalMessage);
            user.statusEffectHolder.RemoveStatusEffect(StatusEffectsComp.Slimed);
            return base.DrinkEffect(user, target);
        }

        public override void ResolveThrow(int x, int y)
        {
            Game.MessageLog.Add("The potion bottle smashes on the ground - spilling water everywhere!", Colors.NormalMessage);

            WaterSurface surf = new WaterSurface();

            Game.DungeonMap.CreateSurface(x, y, 4, "Water",  Colors.ShallowWater, '~', surf);
        }
    }
}
