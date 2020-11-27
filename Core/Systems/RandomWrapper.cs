using RogueSharp.Random;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Systems
{
    // fuck having to do this down to the center of the earth
    [Serializable]
    public class RandomWrapper
    {

        [NonSerialized()]public IRandom random;

        public int ticker;
        public RandomWrapper(IRandom _random)
        {
            random = _random;

        }

        public int Next(int min, int max)
        {
            ticker++;
            return random.Next(min, max);
            
        }


        public void Restore()
        {
            for(int i = 0; i < ticker; i++)
            {
                random.Next(0,0);
            }

        }
    }
}
