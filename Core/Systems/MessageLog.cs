using RLNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace sharpRoguelike.Core.Systems
{
    public class MessageLog
    {
        private static readonly int maxLines = 9;

        private readonly Queue<string> lines;
        string[] arlines;
        bool update;

        public MessageLog()
        {
            lines = new Queue<string>();
        }

        public void Add(string message)
        {
            lines.Enqueue(message);
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
                con.Print(1, i + 1, arlines[i], RLColor.White); 
            }
        }


    }
}
