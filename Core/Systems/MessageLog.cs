using RLNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Systems
{
    public class Message
    {
        public string message;
        public RLColor color;

        public Message(string _message, RLColor _color)
        {
            message = _message;
            color = _color;
        }

    }

    public class MessageLog
    {
        private static readonly int maxLines = 9;

        private readonly Queue<Message> lines;
        Message[] arlines;
        bool update;

        public MessageLog()
        {
            lines = new Queue<Message>();
            update = true;
        }

        public void Add(string message ,RLColor color )
        {
            lines.Enqueue(new Message(message,color));
            if (lines.Count > maxLines)
            {
                lines.Dequeue();
            }
            update = true;
        }

        public void Draw(RLConsole con)
        {
            con.Clear();

            if (update)
            {
                arlines = lines.ToArray();
            }


            for(int i =0; i < arlines.Length; i++)
            {
                con.Print(1, i + 1, arlines[i].message, arlines[i].color); 
            }
        }

        public void Clear()
        {
            lines.Clear();
            arlines = lines.ToArray();

            

        }
    }
}
