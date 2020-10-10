using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace SharpLibs
{
    public class Crypto
    {
        public static int[] EnglishAlphabetHistogram()
        {
            var hist = new int [256];
            var letterFrequency = new Dictionary<char, int>
            {
                {'E', 21912},
                {'T', 16587},
                {'A', 14810},
                {'O', 14003},
                {'I', 13318},
                {'N', 12666},
                {'S', 11450},
                {'R', 10977},
                {'H', 10795},
                {'D', 7874},
                {'L', 7253},
                {'U', 5246},
                {'C', 4943},
                {'M', 4761},
                {'F', 4200},
                {'Y', 3853},
                {'W', 3819},
                {'G', 3693},
                {'P', 3316},
                {'B', 2715},
                {'V', 2019},
                {'K', 1257},
                {'X', 315},
                {'Q', 205},
                {'J', 188},
                {'Z', 128},
            };
            foreach (var kv in letterFrequency)
            {
                hist[kv.Key] = kv.Value;
                hist[kv.Key - 'A' + 'a'] = kv.Value;
            }

            hist[' '] = 10000;
            return hist;
        }

        public static int NumBitsSet(byte b)
        {
            var nset = 0;
            while (b > 0)
            {
                nset += (b & 1);
                b >>= 1;
            }

            return nset;
        }

        public static int HammingDistance(byte[] a, byte[] b)
        {
            return a.Zip(b, (x, y) => (byte)(x ^ y)).Select(NumBitsSet).Sum();
        }

        public static byte[] RepeatingKeyXor(byte[] key, byte[] text)
        {
            var textOut = new byte [text.Length];
            for (var i = 0; i < text.Length; i++)
            {
                textOut[i] = (byte) (text[i] ^ key[i % key.Length]);
            }

            return textOut;
        }

        public static int[] ByteHistogram(byte[] bytes)
        {
            var hist = new int [256];
            foreach (var b in bytes)
            {
                hist[b]++;
            }

            return hist;
        }

        public static int HistogramCorrelation(int [] hist1, int [] hist2)
        {
            return hist1.Select((count, i) => count * hist2[i]).Sum();
        }
    }
}
