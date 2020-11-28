using sharpRoguelike.Core.Components.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components.Usables
{
    [Serializable]
    class WaterPotionEffect : Potion
    {
        public override bool DrinkEffect(Entity user, Entity target)
        {

            Game.MessageLog.Add("You swig down the water potion, its refreshing.", Colors.NormalMessage);
            user.RemoveStatusEffect(StatusEffectsComp.Slimed);
            return base.DrinkEffect(user, target);
        }

        public override void ResolveThrow(int x, int y)
        {
            Game.MessageLog.Add("The potion bottle smashes on the ground - spilling yellow slime everywhere!", Colors.NormalMessage);

            Entity liquid = new Entity();
            liquid.name = "Water";
            liquid.color = Colors.ShallowWater;
            liquid.symbol = '~';
            liquid.surface = new WaterSurface();
            Game.DungeonMap.CreateSurface(x, y, 4, liquid);
        }
    }
}
