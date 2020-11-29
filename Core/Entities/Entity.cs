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
        RLSerialiseableColor s_color;
        [NonSerialized()] private RLColor _color;
        public RLColor color
        {
            get { return _color; }
            set { _color = value; }
        }

        public char symbol { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public bool stackable = false;

        public Inventory inventory = null;              //does this entity have an inventory
        public Attacker attacker = null;                //does this entity attack
        public Actor actor = null;                      //does this entity move
        public Useable effect = null;                   //does this enity have an effect when used
        public Entity corpse = null;                    //does this entity have a corpse
        public Surface surface = null;                  //does this entity have an effect when walked on
        public AI ai = null;                            //does this entity have AI
        public List<EquipmentSlot> equipmentSlots;      //does this entity have any equipment equipped

        public Player player = null;                    //is this entity a player
        public Equipment equipment =null;               //is this entity equipment
        public List<StatusEffect> statusEffects;        //is this entity under a status effect 
        public List<StatusEffectsComp> immuneStatusEffects;  //is this entity immune from a status effect 


        public bool Clairvoince =false;
        public virtual void Draw(RLConsole con, IMap map)
        {
            ICell cell = map.GetCell(x, y);

            if (!cell.IsExplored && !Clairvoince)
            {
                return;
            }

            if (map.IsInFov(x, y) || Clairvoince)
            {
                con.Set(x, y, color, Colors.FloorBackgroundFov, symbol);

            }
            else
            {
                con.Set(x, y, Colors.Floor, Colors.FloorBackground, ' ');
            }
        }

        public void LoadEntityColor()
        {
            _color = new RLColor(s_color.r, s_color.g, s_color.b);
            

        }

        public void SaveEntityColor()
        {
            s_color = new RLSerialiseableColor(_color.r, _color.g, _color.b);
         
        }

        public virtual void OnMove(int previous_x, int previous_y, int end_x, int end_y)
        {

        }

        public void ApplyStatusEffects()
        {
            if (statusEffects == null)
            {
                return;
            }

            for(int i = 0; i < statusEffects.Count; i++)
            {
                statusEffects[i].ApplyEffect();
                statusEffects[i].DecreaseCounter();

            }
        }
        public void RemoveAllStatusEffects()
        {

            if (statusEffects != null)
            {

                foreach (StatusEffect currentEffect in statusEffects)
                {
                    currentEffect.ResolveEffect();

                }
                statusEffects.Clear();
            }
           
        }
        public void RemoveStatusEffect(StatusEffectsComp effect)
        {
            if (statusEffects != null)
            {
                StatusEffect targetEffect = null;

                foreach(StatusEffect currentEffect in statusEffects)
                {
                    if (effect == currentEffect.comp)
                    {
                        currentEffect.ResolveEffect();
                        targetEffect = currentEffect;
                    }

                }

                statusEffects.Remove(targetEffect);
            }

        }

        public bool AddStatusEffect(StatusEffect effect)
        {
            if (statusEffects == null)
            {
                statusEffects = new List<StatusEffect>();
            }

            if (immuneStatusEffects != null)
            {
                foreach(StatusEffectsComp immune_effect in immuneStatusEffects)
                {
                    if (effect.comp == immune_effect)
                    {
                        return false;
                    }
                   
                }
            }

            var dupes = statusEffects.Where(p => p.comp == effect.comp);
            if (dupes.Count() <= 0)
            {
                statusEffects.Add(effect);
                return true;
            }
            return false;
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
