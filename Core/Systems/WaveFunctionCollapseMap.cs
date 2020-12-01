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
using System.Diagnostics;

namespace sharpRoguelike.Core.Systems
{

    public class WaveFunctionCollapseMap 
    {
        public Model model;
        public int width;
        public int height;
        DungeonMap map;

        /*
           params ripped from http://www.procjam.com/tutorials/wfc/
           wfc functionality from https://github.com/mxgmn/WaveFunctionCollapse


           
            name(string)            : The type of this parameter is usually implementation specific. For the original algorithm it's a string representing a file name ($"samples/{name}.png")
            width,height (int)      : Dimensions of the output data
            N (int)                 : Represents the width & height of the patterns that the overlap model breaks the input into. As it solves, it attempts to match up these subpatterns
                                            with each other. A higher N will capture bigger features of the input, but is computationally more intensive, and may require a larger input 
                                            sample to achieve reliable solutions.
            periodic input (bool)   : Represents whether the input pattern is tiling. If true, when WFC digests the input into N pattern chunks it will create patterns connecting the 
                                            right & bottom edges to the left & top. If you use this setting, you'll need to make sure your input "makes sense" accross these edges.
            periodic output (bool)  : Determines if the output solutions are tilable. It's usefull for creating things like tileable textures, but also has a surprising influence on 
                                            the output. When working with WFC, it's often a good idea to toggle Periodic Output on and off, checking if either setting influences the 
                                            results in a favorable way.
            symmetry (int)          : Represents which additional symmetries of the input pattern are digested. 0 is just the original input, 1-8 adds mirrored and rotated variations.
                                            These variations can help flesh out the patterns in your input, but aren't necessary. They also only work with unidirectional tiles, and 
                                            are undesirable when your final game tiles have direction dependent graphics or functionality.
            ground (int)            : When not 0, this assigns a pattern for the bottom row of the output. It's mainly useful for "vertical" words, where you want a distinct ground
                                            and sky separation. The value corresponds to the overlap models internal pattern indexes, so some experimentation is needed to figure
                                            out a suitable value.
        */
        public WaveFunctionCollapseMap(string name, int N ,int width, int height, bool peridoicInput, bool peridoicOutput, int symmetry, int ground)
        {
            this.width = width;
            this.height = height;
            model = new OverlappingModel(name, N, width, height, peridoicInput, peridoicOutput, symmetry, ground);
        }

        //seed(int) All internal random values are derived from this seed, providing 0 results in a random number.
        //limit (int) How many iterations to run, providing 0 will run until completion or a contradiction.
        public DungeonMap Run(int intSeed, int mapWidth, int mapHeight)
        {
            Stopwatch sw = Stopwatch.StartNew();

            bool finish = model.Run(30, 0);
            for (int k = 0; k < 10; k++)
            {
                int seed = Game.Random.Next(0, 1000000);
                bool finished = model.Run(seed, 0);
           
                if (finished)
                {
                    Console.WriteLine("DONE");

                    //model.Graphics().Save($"{k} qud.png");

                    map = MakeMapFromImage(model.Graphics(), mapWidth, mapHeight);
                    
                    Console.WriteLine($"time = {sw.ElapsedMilliseconds}");
                    return map;
                }
                else
                {
                    Console.WriteLine("CONTRADICTION");
                }
            }
            return null;
        }


        public static DungeonMap MakeMapFromImage(Bitmap image, int mapwidth, int mapHeight)
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
            map.Initialize(mapwidth, mapHeight);
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
            

            return map;
        }
    }

  
}

