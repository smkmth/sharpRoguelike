using System;
using System.Collections.Generic;
using System.Text;
using RLNET;
using RogueSharp;
using sharpRoguelike.Core.Interfaces;
namespace sharpRoguelike.Core
{
    [Serializable]
    public class Actor :  IScheduleable
    {
        public Actor(Entity _owner)
        {
            owner = _owner;
        }

        public Entity owner;
        public int Time
        {
            get
            {
                return Speed;
            }
        }

     
        private int _awareness;


        private int _speed;

      

        public int Awareness
        {
            get
            {
                return _awareness;
            }
            set
            {
                _awareness = value;
            }
        }




        public int Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
            }
        }

        Entity IScheduleable.owner
        {
            get
            {
                return owner;
            }
        }
    }
}
