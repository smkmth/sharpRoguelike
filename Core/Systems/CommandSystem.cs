using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Systems
{
    public class CommandSystem
    {
        public bool MovePlayer(Direction direction)
        {
            int x = Game.Player.x;
            int y = Game.Player.y;

            switch( direction)
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
                return false;
            }


        }


    }

    
}
