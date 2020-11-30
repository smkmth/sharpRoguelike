using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    [Serializable]
    class RifleEffect : RangedWeapon
    {
        public int clip;

        public RifleEffect(Equipment _owner, int _clip): base (_owner)
        {
            clip = _clip;
        }

        public override void TargetCallback(int x, int y)
        {
            if (ammo <= 0)
            {
                if (owner.ownerHolder != null)
                {
                    Load();
                    return;
                }
            }

            var entities = Game.DungeonMap.GetAllEntitiesAt(x, y);
            foreach (Entity entity in entities)
            {
                if (Game.DungeonMap.IsInFov(x, y))
                {
                    if (entity.attacker != null)
                    {
                        entity.attacker.Health -= damage;
                        ammo--;
                        Game.MessageLog.Add($"The shot rings out loud and true - striking the target for {damage} damage", Colors.CombatMessage);
                        return;
                    }

                }
            }
            Game.MessageLog.Add($"Nothing is there to hit!", Colors.HelpMessage);


        }

        public override void TargetCancelledCallback(bool canceled)
        {
            Game.CurrentGameMode = GameMode.PLAYING;
        }

        public override bool Load()
        {
            if (owner.ownerHolder != null)
            {
                ammo = owner.ownerHolder.inventory.LoadAmmoOfType(AmmoType.Bullets,clip);
                if (ammo > 0)
                {
                    Game.MessageLog.Add("Loading ammo into rifle...", Colors.NormalMessage);
                    return true;
                }
                else
                {
                    Game.MessageLog.Add("Out of ammo!", Colors.NormalMessage);
                    return false;
                }
            }
            return false;
        }

    }
}
