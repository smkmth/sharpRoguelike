/*
The MIT License(MIT)
Copyright(c) mxgmn 2016.
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
The software is provided "as is", without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In no event shall the authors or copyright holders be liable for any claim, damages or other liability, whether in an action of contract, tort or otherwise, arising from, out of or in connection with the software or the use or other dealings in the software.
*/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using RogueSharp;

namespace sharpRoguelike.Core.Systems
{
    public class WaveFunctionCollapseMap 
    {
        public Model model;
        public int width;
        public int height;
        DungeonMap map;

        public WaveFunctionCollapseMap(string name, int N ,int width, int height, bool peridoicInput, bool peridoicOutput, int symmetry, int ground)
        {
            this.width = width;
            this.height = height;
            model = new OverlappingModel(name, N, width, height, peridoicInput, peridoicOutput, symmetry, ground);
        }

        public DungeonMap Run(int intSeed)
        {
            bool finish = model.Run(30, 0);
            for (int k = 0; k < 10; k++)
            {
                int seed = Game.Random.Next(0, 1000000);
                bool finished = model.Run(seed, 0);
           
                if (finished)
                {
                    Console.WriteLine("DONE");

                    model.Graphics().Save($"{k} qud.png");

                    map = MakeMapFromImage(model.Graphics());

                    return map;
                }
                else
                {
                    Console.WriteLine("CONTRADICTION");
                }
            }
            return null;
        }

        public static void PlacePlayer(DungeonMap map, int x, int y)
        {
            Entity player = Game.Player;
            if (player == null)
            {
                player = new Entity();
            }
            player.x = x;
            player.y = y;

            map.AddPlayer(player);
        }

        public static DungeonMap MakeMapFromImage(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            Color[] colors = new Color[image.Width * image.Height];
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    colors[x + image.Width * y] = image.GetPixel(x, y);

                }

            }

            DungeonMap map = new DungeonMap();
            map.Initialize(width, height);
            int lastx = 0;
            int lasty = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = colors[x + width * y];
                 
                    if (color.R == 255 && color.B == 0 && color.G == 0)
                    {
                        map.SetCellProperties(x, y, true, true);
                        map.GetCell(x, y);


                    }
                    else if (color.R == 0 && color.B == 255 && color.G == 0)
                    {
                        map.SetCellProperties(x, y, true, true);
                        Door door = new Door
                        {
                            x = x,
                            y = y,
                            isOpen = false

                        };
                        map.Doors.Add(door);
                        map.Entities.Add(door);
                    }
                    else if (color.R == 0 && color.B == 0 && color.G == 0)
                    {
                        map.SetCellProperties(x, y, false, false);

                    }
                    else
                    {
                        lastx = x;
                        lasty = y;
                        map.SetCellProperties(x, y, true, true);

                    }
                    
                 
                }
            }
            
            PlacePlayer(map, lastx, lasty);

            return map;
        }
    }

  
}

