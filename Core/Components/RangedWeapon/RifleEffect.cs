using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    [Serializable]
    class RifleEffect : RangedWeapon
    {
        public RifleEffect(Equipment _owner): base (_owner)
        {

        }

        public override void TargetCallback(int x, int y)
        {
            var entities = Game.DungeonMap.GetAllEntitiesAt(x, y);

            foreach(Entity entity in entities)
            {
                if (Game.DungeonMap.IsInFov(x, y))
                {
                    if (entity.attacker != null)
                    {
                        entity.attacker.Health -= damage;
                        Game.MessageLog.Add($"The shot rings out loud and true - striking the target for {damage} damage", Colors.CombatMessage);

                    }
                }
            }
            Game.MessageLog.Add($"Nothing is there to hit!", Colors.HelpMessage);


        }

        public override void TargetCancelledCallback(bool canceled)
        {
            Game.CurrentGameMode = GameMode.PLAYING;
        }
    }
}
