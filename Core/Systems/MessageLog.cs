using RLNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Systems
{
    //resposible for handling messages that appear on the right of the screen
    //that tell you whats going on. easy enough really just manages a queue of 
    //'messages' holding a message string and message color - with some rudimentary
    //text wrapping
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
        private static readonly int maxLines = 79;

        private readonly Queue<Message> lines;
        Message[] arlines;
        bool update;
        int screenWidth;

        public MessageLog(int _screenWidth )
        {
            screenWidth = _screenWidth - 1; //leave ourselves a buffer
            lines = new Queue<Message>();
            update = true;
        }
        static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        public void Add(string message, RLColor color )
        {
            var brokenStrings = ChunksUpto(message, screenWidth);
     
            foreach (var brokestring in brokenStrings)
            {
                lines.Enqueue(new Message(brokestring, color));
                lines.Enqueue(new Message("                                     ", color));

            }
            lines.Enqueue(new Message("-------------------------------------", color));

            while(lines.Count > maxLines)
            {
                lines.Dequeue();
            }
            update = true;
        }

        public void Draw(RLConsole con)
        {
            con.Print(1, 1, "Messages", RLColor.White);
            if (update)
            {
                arlines = lines.ToArray();
                update = false;
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
