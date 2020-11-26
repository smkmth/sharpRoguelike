using RogueSharp;
using RogueSharp.DiceNotation;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Systems
{
    public class CommandSystem
    {

        public bool IsPlayerTurn { get; set; }

        public void EndPlayerTurn()
        {
            IsPlayerTurn = false;
        }

        public void ActivateMonsters()
        {
            IScheduleable scheduleable = Game.SchedulingSytem.Get();
            if (scheduleable.owner is Player)
            {
                IsPlayerTurn = true;
                Game.SchedulingSytem.Add(Game.Player.actor);

            }
            else
            {
                Monster monster = scheduleable.owner as Monster;
                if (monster != null)
                {
                    monster.PerformAction(this);
                    Game.SchedulingSytem.Add(monster.actor);
                }
                ActivateMonsters();

            }
        }

        public void MoveMonster( Monster monster, ICell cell)
        {
            if (!Game.DungeonMap.SetActorPosition(monster, cell.X, cell.Y))
            {
                if (Game.Player.x == cell.X && Game.Player.y == cell.Y)
                {
                    Attack(monster, Game.Player);

                }
            }
        }

        public bool MovePlayer(Direction direction)
        {
            int x = Game.Player.x;
            int y = Game.Player.y;

            switch ( direction)
            {
                case Direction.Up:
                    {
                        y = Game.Player.y - 1;
                        break;
                    }
                case Direction.Down:
                    {
                        y = Game.Player.y + 1;
                        break;
                    }
                case Direction.Left:
                    {
                        x = Game.Player.x - 1;

                        break;
                    }
                case Direction.Right:
                    {
                        x = Game.Player.x + 1;

                        break;
                    }
                default:
                    {
                        return false;
                    }
            }

            if (Game.DungeonMap.SetActorPosition(Game.Player, x, y))
            {
                return true;
            }
            else
            {
                List<Entity> targets = Game.DungeonMap.GetAllEntitiesAt(x, y);
                foreach(Entity target in targets)
                {
                    if (target.attacker != null)
                    {
                        Attack(Game.Player, target);

                    }
                }
                if (targets.Count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }


        }

        public void Attack(Entity attacker, Entity defender)
        {
            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defenseMessage = new StringBuilder();

            int hits = ResolveAttack(attacker, defender, attackMessage);

            int blocks = ResolveDefense(defender, hits, attackMessage, defenseMessage);

            Game.MessageLog.Add(attackMessage.ToString(), Colors.CombatMessage);
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
            {
                Game.MessageLog.Add(defenseMessage.ToString(), Colors.CombatMessage);
            }

            int damage = hits - blocks;

            ResolveDamage(defender, damage);
        }

        // The attacker rolls based on his stats to see if he gets any hits
        private static int ResolveAttack(Entity attacker, Entity defender, StringBuilder attackMessage)
        {
            int hits = 0;

            attackMessage.AppendFormat("{0} attacks {1} and rolls: ", attacker.name, defender.name);

            // Roll a number of 100-sided dice equal to the Attack value of the attacking actor
            DiceExpression attackDice = new DiceExpression().Dice(attacker.attacker.Attack, 100);
            DiceResult attackResult = attackDice.Roll();

            // Look at the face value of each single die that was rolled
            foreach (TermResult termResult in attackResult.Results)
            {
                attackMessage.Append(termResult.Value + ", ");
                // Compare the value to 100 minus the attack chance and add a hit if it's greater
                if (termResult.Value >= 100 - attacker.attacker.AttackChance)
                {
                    hits++;
                }
            }

            return hits;
        }

        // The defender rolls based on his stats to see if he blocks any of the hits from the attacker
        private static int ResolveDefense(Entity defender, int hits, StringBuilder attackMessage, StringBuilder defenseMessage)
        {
            int blocks = 0;

            if (hits > 0)
            {
                attackMessage.AppendFormat("scoring {0} hits.", hits);
                defenseMessage.AppendFormat("  {0} defends and rolls: ", defender.name);

                // Roll a number of 100-sided dice equal to the Defense value of the defendering actor
                DiceExpression defenseDice = new DiceExpression().Dice(defender.attacker.Defense, 100);
                DiceResult defenseRoll = defenseDice.Roll();

                // Look at the face value of each single die that was rolled
                foreach (TermResult termResult in defenseRoll.Results)
                {
                    defenseMessage.Append(termResult.Value + ", ");
                    // Compare the value to 100 minus the defense chance and add a block if it's greater
                    if (termResult.Value >= 100 - defender.attacker.DefenseChance)
                    {
                        blocks++;
                    }
                }
                defenseMessage.AppendFormat("resulting in {0} blocks.", blocks);
            }
            else
            {
                attackMessage.Append("and misses completely.");
            }

            return blocks;
        }

        // Apply any damage that wasn't blocked to the defender
        private static void ResolveDamage(Entity defender, int damage)
        {
            if (damage > 0)
            {
                defender.attacker.Health = defender.attacker.Health - damage;

                Game.MessageLog.Add($"  {defender.name} was hit for {damage} damage", Colors.CombatMessage);

                if (defender.attacker.Health <= 0)
                {
                    ResolveDeath(defender);
                }
            }
            else
            {
                Game.MessageLog.Add($"  {defender.name} blocked all damage",Colors.CombatMessage);
            }
        }

        // Remove the defender from the map and add some messages upon death.
        private static void ResolveDeath(Entity defender)
        {
            if (defender is Player)
            {
                Game.MessageLog.Add($"  {defender.name} was killed, GAME OVER MAN!",Colors.CombatMessage);
            }
            else 
            {
                Game.DungeonMap.RemoveMonster((Monster)defender);
                defender.corpse.x = defender.x;
                defender.corpse.y = defender.y;
                Game.DungeonMap.AddItem(defender.corpse);
                Game.MessageLog.Add($" {defender.name} died !", Swatch.DbBlood);
            }
        }


    }

    
}
