﻿using System;

namespace ConsoleApp2.ExtensionMethods
{
        public static class RandomExtensionMethods
        {
            /// <summary>
            /// Returns a random long from min (inclusive) to max (exclusive)
            /// </summary>
            /// <param name="random">The given random instance</param>
            /// <param name="min">The inclusive minimum bound</param>
            /// <param name="max">The exclusive maximum bound.  Must be greater than min</param>
            public static ulong NextLong(this Random random, ulong min, ulong max)
            {
                if (max <= min)
                    throw new ArgumentOutOfRangeException("max", "max must be > min!");

                //Working with ulong so that modulo works correctly with values > long.MaxValue
                ulong uRange = (ulong)(max - min);

                //Prevent a modolo bias; see https://stackoverflow.com/a/10984975/238419
                //for more information.
                //In the worst case, the expected number of calls is 2 (though usually it's
                //much closer to 1) so this loop doesn't really hurt performance at all.
                ulong ulongRand;
                do
                {
                    byte[] buf = new byte[8];
                    random.NextBytes(buf);
                    ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
                } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);

                return (ulong)(ulongRand % uRange) + min;
            }

            public static ulong NextLong(this Random random)
            {
                return random.NextLong(ulong.MinValue, ulong.MaxValue);
            }
        }
}