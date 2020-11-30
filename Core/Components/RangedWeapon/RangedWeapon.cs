using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    public enum AmmoType
    {
        Bullets
    }

    [Serializable]
    public class RangedWeapon 
    {
        public int ammo;
        public Equipment owner;
        public int damage;
        public string targetString;

        public RangedWeapon(Equipment _owner)
        {
            owner = _owner;
        }
        
        public virtual void TargetCallback(int x, int y)
        {

        }

        public virtual void TargetCancelledCallback(bool canceled)
        {

        }

        public virtual bool Load()
        {
            return false;

        }

    }
}
