using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using RogueSharp;
using System.Drawing;
using System;

namespace sharpRoguelike.Core.Systems
{

    static class Stuff
    {
        public static int Random(this double[] a, double r)
        {
            double sum = a.Sum();
            for (int j = 0; j < a.Length; j++) a[j] /= sum;

            int i = 0;
            double x = 0;

            while (i < a.Length)
            {
                x += a[i];
                if (r <= x) return i;
                i++;
            }

            return 0;
        }

        public static long ToPower(this int a, int n)
        {
            long product = 1;
            for (int i = 0; i < n; i++) product *= a;
            return product;
        }

        public static T Get<T>(this XElement xelem, string attribute, T defaultT = default)
        {
            XAttribute a = xelem.Attribute(attribute);
            return a == null ? defaultT : (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(a.Value);
        }

        public static IEnumerable<XElement> Elements(this XElement xelement, params string[] names) => xelement.Elements().Where(e => names.Any(n => n == e.Name));
    }


    public abstract class Model
    {
        protected bool[][] wave;

        protected int[][][] propagator;
        int[][][] compatible;
        protected int[] observed;

        (int, int)[] stack;
        int stacksize;

        protected Random random;
        protected int FMX, FMY, T;
        protected bool periodic;

        protected double[] weights;
        double[] weightLogWeights;

        int[] sumsOfOnes;
        double sumOfWeights, sumOfWeightLogWeights, startingEntropy;
        double[] sumsOfWeights, sumsOfWeightLogWeights, entropies;

        protected Model(int width, int height)
        {
            FMX = width;
            FMY = height;
        }

        void Init()
        {
            wave = new bool[FMX * FMY][];
            compatible = new int[wave.Length][][];
            for (int i = 0; i < wave.Length; i++)
            {
                wave[i] = new bool[T];
                compatible[i] = new int[T][];
                for (int t = 0; t < T; t++) compatible[i][t] = new int[4];
            }

            weightLogWeights = new double[T];
            sumOfWeights = 0;
            sumOfWeightLogWeights = 0;

            for (int t = 0; t < T; t++)
            {
                weightLogWeights[t] = weights[t] * Math.Log(weights[t]);
                sumOfWeights += weights[t];
                sumOfWeightLogWeights += weightLogWeights[t];
            }

            startingEntropy = Math.Log(sumOfWeights) - sumOfWeightLogWeights / sumOfWeights;

            sumsOfOnes = new int[FMX * FMY];
            sumsOfWeights = new double[FMX * FMY];
            sumsOfWeightLogWeights = new double[FMX * FMY];
            entropies = new double[FMX * FMY];

            stack = new (int, int)[wave.Length * T];
            stacksize = 0;
        }

        bool? Observe()
        {
            double min = 1E+3;
            int argmin = -1;

            for (int i = 0; i < wave.Length; i++)
            {
                if (OnBoundary(i % FMX, i / FMX)) continue;

                int amount = sumsOfOnes[i];
                if (amount == 0) return false;

                double entropy = entropies[i];
                if (amount > 1 && entropy <= min)
                {
                    double noise = 1E-6 * random.NextDouble();
                    if (entropy + noise < min)
                    {
                        min = entropy + noise;
                        argmin = i;
                    }
                }
            }

            if (argmin == -1)
            {
                observed = new int[FMX * FMY];
                for (int i = 0; i < wave.Length; i++) for (int t = 0; t < T; t++) if (wave[i][t]) { observed[i] = t; break; }
                return true;
            }

            double[] distribution = new double[T];
            for (int t = 0; t < T; t++) distribution[t] = wave[argmin][t] ? weights[t] : 0;
            int r = distribution.Random(random.NextDouble());

            bool[] w = wave[argmin];
            for (int t = 0; t < T; t++) if (w[t] != (t == r)) Ban(argmin, t);

            return null;
        }

        protected void Propagate()
        {
            while (stacksize > 0)
            {
                var e1 = stack[stacksize - 1];
                stacksize--;

                int i1 = e1.Item1;
                int x1 = i1 % FMX, y1 = i1 / FMX;

                for (int d = 0; d < 4; d++)
                {
                    int dx = DX[d], dy = DY[d];
                    int x2 = x1 + dx, y2 = y1 + dy;
                    if (OnBoundary(x2, y2)) continue;

                    if (x2 < 0) x2 += FMX;
                    else if (x2 >= FMX) x2 -= FMX;
                    if (y2 < 0) y2 += FMY;
                    else if (y2 >= FMY) y2 -= FMY;

                    int i2 = x2 + y2 * FMX;
                    int[] p = propagator[d][e1.Item2];
                    int[][] compat = compatible[i2];

                    for (int l = 0; l < p.Length; l++)
                    {
                        int t2 = p[l];
                        int[] comp = compat[t2];

                        comp[d]--;
                        if (comp[d] == 0) Ban(i2, t2);
                    }
                }
            }
        }

        public bool Run(int seed, int limit)
        {
            if (wave == null) Init();

            Clear();
            random = new Random(seed);

            for (int l = 0; l < limit || limit == 0; l++)
            {
                bool? result = Observe();
                Console.WriteLine($"OBSERVE {l}...");
                if (result != null) return (bool)result;
                Propagate();
                Console.WriteLine($"PROPAGATE {l}...");
            }

            return true;
        }

        protected void Ban(int i, int t)
        {
            wave[i][t] = false;

            int[] comp = compatible[i][t];
            for (int d = 0; d < 4; d++) comp[d] = 0;
            stack[stacksize] = (i, t);
            stacksize++;

            sumsOfOnes[i] -= 1;
            sumsOfWeights[i] -= weights[t];
            sumsOfWeightLogWeights[i] -= weightLogWeights[t];

            double sum = sumsOfWeights[i];
            entropies[i] = Math.Log(sum) - sumsOfWeightLogWeights[i] / sum;
        }

        protected virtual void Clear()
        {
            for (int i = 0; i < wave.Length; i++)
            {
                for (int t = 0; t < T; t++)
                {
                    wave[i][t] = true;
                    for (int d = 0; d < 4; d++) compatible[i][t][d] = propagator[opposite[d]][t].Length;
                }

                sumsOfOnes[i] = weights.Length;
                sumsOfWeights[i] = sumOfWeights;
                sumsOfWeightLogWeights[i] = sumOfWeightLogWeights;
                entropies[i] = startingEntropy;
            }
        }

        protected abstract bool OnBoundary(int x, int y);
        public abstract System.Drawing.Bitmap Graphics();

        protected static int[] DX = { -1, 0, 1, 0 };
        protected static int[] DY = { 0, 1, 0, -1 };
        static int[] opposite = { 2, 3, 0, 1 };
    }

    /*
    The MIT License(MIT)
    Copyright(c) mxgmn 2016.
    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
    The software is provided "as is", without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In no event shall the authors or copyright holders be liable for any claim, damages or other liability, whether in an action of contract, tort or otherwise, arising from, out of or in connection with the software or the use or other dealings in the software.
    */

    public class OverlappingModel : Model
    {
        int N;
        byte[][] patterns;
        List<Color> colors;
        int ground;

        public OverlappingModel(string name, int N, int width, int height, bool periodicInput, bool periodicOutput, int symmetry, int ground)
            : base(width, height)
        {
            this.N = N;
            periodic = periodicOutput;
            //          if (!File.Exists($"C:/Users/danda/source/repos/sharpRoguelike/{name}.png"))
            //          {
            //              Console.WriteLine("no file");
            //          
            //          }
            //          
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Data\", $"{name}.png");
            var bitmap = new Bitmap(path);
           // var bitmap = new Bitmap($"C:/Users/Lab42_AMD_1/source/repos/sharpRoguelike/{name}.png");

            int SMX = bitmap.Width, SMY = bitmap.Height;
            byte[,] sample = new byte[SMX, SMY];
            colors = new List<Color>();

            for (int y = 0; y < SMY; y++) for (int x = 0; x < SMX; x++)
                {
                    Color color = bitmap.GetPixel(x, y);

                    int i = 0;
                    foreach (var c in colors)
                    {
                        if (c == color) break;
                        i++;
                    }

                    if (i == colors.Count) colors.Add(color);
                    sample[x, y] = (byte)i;
                }

            int C = colors.Count;
            long W = C.ToPower(N * N);

            byte[] pattern(Func<int, int, byte> f)
            {
                byte[] result = new byte[N * N];
                for (int y = 0; y < N; y++) for (int x = 0; x < N; x++) result[x + y * N] = f(x, y);
                return result;
            };

            byte[] patternFromSample(int x, int y) => pattern((dx, dy) => sample[(x + dx) % SMX, (y + dy) % SMY]);
            byte[] rotate(byte[] p) => pattern((x, y) => p[N - 1 - y + x * N]);
            byte[] reflect(byte[] p) => pattern((x, y) => p[N - 1 - x + y * N]);

            long index(byte[] p)
            {
                long result = 0, power = 1;
                for (int i = 0; i < p.Length; i++)
                {
                    result += p[p.Length - 1 - i] * power;
                    power *= C;
                }
                return result;
            };

            byte[] patternFromIndex(long ind)
            {
                long residue = ind, power = W;
                byte[] result = new byte[N * N];

                for (int i = 0; i < result.Length; i++)
                {
                    power /= C;
                    int count = 0;

                    while (residue >= power)
                    {
                        residue -= power;
                        count++;
                    }

                    result[i] = (byte)count;
                }

                return result;
            };

            Dictionary<long, int> weights = new Dictionary<long, int>();
            List<long> ordering = new List<long>();

            for (int y = 0; y < (periodicInput ? SMY : SMY - N + 1); y++) for (int x = 0; x < (periodicInput ? SMX : SMX - N + 1); x++)
                {
                    byte[][] ps = new byte[8][];

                    ps[0] = patternFromSample(x, y);
                    ps[1] = reflect(ps[0]);
                    ps[2] = rotate(ps[0]);
                    ps[3] = reflect(ps[2]);
                    ps[4] = rotate(ps[2]);
                    ps[5] = reflect(ps[4]);
                    ps[6] = rotate(ps[4]);
                    ps[7] = reflect(ps[6]);

                    for (int k = 0; k < symmetry; k++)
                    {
                        long ind = index(ps[k]);
                        if (weights.ContainsKey(ind)) weights[ind]++;
                        else
                        {
                            weights.Add(ind, 1);
                            ordering.Add(ind);
                        }
                    }
                }

            T = weights.Count;
            this.ground = (ground + T) % T;
            patterns = new byte[T][];
            base.weights = new double[T];

            int counter = 0;
            foreach (long w in ordering)
            {
                patterns[counter] = patternFromIndex(w);
                base.weights[counter] = weights[w];
                counter++;
            }

            bool agrees(byte[] p1, byte[] p2, int dx, int dy)
            {
                int xmin = dx < 0 ? 0 : dx, xmax = dx < 0 ? dx + N : N, ymin = dy < 0 ? 0 : dy, ymax = dy < 0 ? dy + N : N;
                for (int y = ymin; y < ymax; y++) for (int x = xmin; x < xmax; x++) if (p1[x + N * y] != p2[x - dx + N * (y - dy)]) return false;
                return true;
            };

            propagator = new int[4][][];
            for (int d = 0; d < 4; d++)
            {
                propagator[d] = new int[T][];
                for (int t = 0; t < T; t++)
                {
                    List<int> list = new List<int>();
                    for (int t2 = 0; t2 < T; t2++) if (agrees(patterns[t], patterns[t2], DX[d], DY[d])) list.Add(t2);
                    propagator[d][t] = new int[list.Count];
                    for (int c = 0; c < list.Count; c++) propagator[d][t][c] = list[c];
                }
            }
        }

        protected override bool OnBoundary(int x, int y) => !periodic && (x + N > FMX || y + N > FMY || x < 0 || y < 0);

        public override Bitmap Graphics()
        {


            Bitmap result = new Bitmap(FMX, FMY);
            int[] bitmapData = new int[result.Height * result.Width];

            if (observed != null)
            {
                for (int y = 0; y < FMY; y++)
                {
                    int dy = y < FMY - N + 1 ? 0 : N - 1;
                    for (int x = 0; x < FMX; x++)
                    {
                        int dx = x < FMX - N + 1 ? 0 : N - 1;
                        Color c = colors[patterns[observed[x - dx + (y - dy) * FMX]][dx + dy * N]];
                        bitmapData[x + y * FMX] = unchecked((int)0xff000000 | (c.R << 16) | (c.G << 8) | c.B);
                    }
                }
            }
            else
            {
                for (int i = 0; i < wave.Length; i++)
                {
                    int contributors = 0, r = 0, g = 0, b = 0;
                    int x = i % FMX, y = i / FMX;

                    for (int dy = 0; dy < N; dy++) for (int dx = 0; dx < N; dx++)
                        {
                            int sx = x - dx;
                            if (sx < 0) sx += FMX;

                            int sy = y - dy;
                            if (sy < 0) sy += FMY;

                            int s = sx + sy * FMX;
                            if (OnBoundary(sx, sy)) continue;
                            for (int t = 0; t < T; t++) if (wave[s][t])
                                {
                                    contributors++;
                                    Color color = colors[patterns[t][dx + dy * N]];
                                    r += color.R;
                                    g += color.G;
                                    b += color.B;
                                }
                        }

                    bitmapData[i] = unchecked((int)0xff000000 | ((r / contributors) << 16) | ((g / contributors) << 8) | b / contributors);
                }
            }

            var bits = result.LockBits(new System.Drawing.Rectangle(0, 0, result.Width, result.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(bitmapData, 0, bits.Scan0, bitmapData.Length);
            result.UnlockBits(bits);

            return result;
        }

        protected override void Clear()
        {
            base.Clear();

            if (ground != 0)
            {
                for (int x = 0; x < FMX; x++)
                {
                    for (int t = 0; t < T; t++) if (t != ground) Ban(x + (FMY - 1) * FMX, t);
                    for (int y = 0; y < FMY - 1; y++) Ban(x + y * FMX, ground);
                }

                Propagate();
            }
        }
    }
}

