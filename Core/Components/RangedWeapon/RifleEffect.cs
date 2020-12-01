using RogueSharp.DiceNotation;
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
                        int dist = Game.DungeonMap.GetDistance(owner.ownerHolder ,entity);
                        int distMod = 0;
                        if (dist < 4)
                        {
                            distMod += 20;
                        }
                        else if (dist < 7)
                        {
                            distMod += 10;
                        }
                        else if (dist < 10)
                        {
                            distMod = 0;
                        }
                        else
                        {
                            distMod = -10;
                        }

                        int roll = Dice.Roll("1D100");
                        int checkVal = owner.ownerHolder.attacker.Accuracy + distMod;
                        if (roll >= 100)
                        {
                            int damageval = Dice.Roll(damage) * 2;
                            entity.attacker.Health -= damageval;
                            Game.MessageLog.Add($"The shot crits! {damageval} damage!", Colors.CombatMessage);

                        }
                        else if (roll < checkVal)
                        {
                            int damageval = Dice.Roll(damage);
                            entity.attacker.Health -= damageval;
                            Game.MessageLog.Add($"The shot rings out loud and true - striking the target for {damageval} damage", Colors.CombatMessage);

                        }
                        else
                        {
                            Game.MessageLog.Add($"You clean miss the shot", Colors.CombatMessage);

                        }
                        ammo--;
                        if (entity.ai != null)
                        {
                            entity.ai.AlertAI();
                        }
                        int numalerted = Game.DungeonMap.AlertAllInRange(owner.ownerHolder.transform.x, owner.ownerHolder.transform.y, loudness);
                        if (numalerted > 0)
                        {
                            Game.MessageLog.Add($"As the ringing of the shot dies down - you hear noises all around you", Colors.CombatMessage);
                        }

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
