using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Components;
using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sharpRoguelike.Core
{
    //struct so i can pull out the color of an entity or thing or whatever
    //and serialise it as RLColor is locked off to me
    [Serializable]
    public struct RLSerialiseableColor
    {
        public float r;
        public float g;
        public float b;

        public RLSerialiseableColor(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }

    //the main class that things 'are'. Everything in the game 
    //is an entity - entities have components - that i can query. 
    //a player is nto a unique type of entity - but an entity with
    //a player component attached to it. I just to a null check 
    //to see if an entity has some relevant component attached. 
    //there are some unique entities - but they are only unique 
    //in the way they are drawn - not functionality. 

    //components can express a has a or is a relationship. a entity 
    //with a player component is a player, a component with a effect
    //component can be 'used' in a inventory - so it is an item
    [Serializable]
    public class Entity 
    {

        public bool stackable = false;
        public int stackcount =0;

        public Transform transform = null;              //does this entity have a physical location
        public Renderer renderer = null;                //does this entity have a renderer
        public AI ai = null;                            //does this entity have AI
        
        public Inventory inventory = null;                      //does this entity have an inventory
        public Attacker attacker = null;                        //does this entity attack
        public Actor actor = null;                              //does this entity move
        public Useable effect = null;                           //does this enity have an effect when used
        public Entity corpse = null;                            //does this entity have a corpse
        public Surface surface = null;                          //does this entity have an effect when walked on
        public List<EquipmentSlot> equipmentSlots;              //does this entity have any equipment equipped
        public Player player = null;                            //is this entity a player
        public Equipment equipment =null;                       //is this entity equipment
        public StatusEffectHolder statusEffectHolder;           //can this entity get status effects

        public string name { get; set; }
        public string description { get; set; }

        public virtual void OnMove(int previous_x, int previous_y, int end_x, int end_y)
        {

        }

       
        public Equipment GetEquippedRangedWeapon()
        {
            Equipment rangedWeapon = null;
            foreach(EquipmentSlot slot in equipmentSlots)
            {
                if (slot.equipType == EquipSlotType.RANGED_WEAPON)
                {
                    if (slot.attachedEquipment != null)
                    {
                        rangedWeapon = slot.attachedEquipment;
                    }
                }
            }

            return rangedWeapon;

        }



    }
}
