using sharpRoguelike.Core.Components.Surfaces;
using sharpRoguelike.Core.Components.Usables;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    [Serializable]
    class HealPotionUse : PotionUse
    {

        public override bool DrinkEffect(Entity user, Entity target)
        {
            if (target.attacker != null)
            {

                if (target.attacker.Health == target.attacker.MaxHealth)
                {
                    Game.MessageLog.Add("tried to drink health potion, but health was already at max", Colors.HelpMessage);
                    return false;
                }
                else
                {
                    target.attacker.Health += 20;
                    if (target.attacker.Health > target.attacker.MaxHealth)
                    {
                        target.attacker.Health = target.attacker.MaxHealth;
                    }
                    Game.MessageLog.Add("drank down a bottle of health potion - and you feel your wounds sew back together.", Colors.HealMessage);
                    return true;

                }
            }
            else
            {
                return base.DrinkEffect(user, target);
            }

        }


        public override void ResolveThrow(int x, int y)
        {
            base.ResolveThrow(x, y);

            Game.MessageLog.Add("The potion bottle smashes on the ground - spilling healing liquid everywhere!", Colors.NormalMessage);

            HealingSurface surf= new HealingSurface();

            Game.DungeonMap.CreateSurface(x, y, 4, "Healing Liquid", Colors.CombatMessage, '~', surf);
        }

    }
}
