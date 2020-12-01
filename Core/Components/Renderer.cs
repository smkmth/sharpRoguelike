using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Components
{
    [Serializable]
    public class Renderer
    {
        public Entity owner;

        public Renderer(Entity _owner)
        {
            owner = _owner;
        }

        RLSerialiseableColor s_color;
        [NonSerialized()] private RLColor _color;
        public RLColor color
        {
            get { return _color; }
            set { _color = value; }
        }

        RLSerialiseableColor s_bgcolor;
        [NonSerialized()] private RLColor _bgcolor;
        public RLColor backgroundColor
        {
            get { return _bgcolor; }
            set { _bgcolor = value; }
        }


        public char symbol { get; set; }

        public bool Clairvoince = false;


        public virtual void Draw(RLConsole con, IMap map)
        {
            ICell cell = map.GetCell(owner.transform.x, owner.transform.y);

            if (!cell.IsExplored && !Clairvoince)
            {
                return;
            }

            if (map.IsInFov(owner.transform.x, owner.transform.y) || Clairvoince)
            {
                con.Set(owner.transform.x, owner.transform.y, color, Colors.FloorBackgroundFov, symbol);

            }
            else
            {
                con.Set(owner.transform.x, owner.transform.y, Colors.Floor, Colors.FloorBackground, ' ');
            }
        }

        public void LoadEntityColor()
        {
            _color = new RLColor(s_color.r, s_color.g, s_color.b);
            _bgcolor = new RLColor(s_bgcolor.r, s_bgcolor.g, s_bgcolor.b);


        }

        public void SaveEntityColor()
        {
            s_color = new RLSerialiseableColor(_color.r, _color.g, _color.b);
            s_bgcolor = new RLSerialiseableColor(_bgcolor.r, _bgcolor.g, _bgcolor.b);

        }

    }
}
