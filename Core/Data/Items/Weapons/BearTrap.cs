using sharpRoguelike.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Data.Items
{
    [Serializable]
    class BearTrap : Entity
    {
       
        public static BearTrap Create()
        {
            BearTrap beartrap = new BearTrap
            {

                name = "A Untripped BearTrap",
                description = "Set on ground to damage enemies"
            };
            beartrap.renderer = new Renderer(beartrap);
            beartrap.renderer.symbol = '^';
            beartrap.renderer.color = Colors.LowTeirItem;
            beartrap.transform = new Transform();

            beartrap.effect = new SetTrapUse();
            beartrap.effect.owner = beartrap;

    
            return beartrap;

        }
        
    }
}
