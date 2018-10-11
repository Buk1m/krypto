using System;

namespace ConsoleApp2.ExtensionMethods
{
        public static class RandomExtensionMethods
        {
            public static ulong NextLong(this Random random, ulong min, ulong max)
            {
                if (max <= min)
                    throw new ArgumentOutOfRangeException("max", "max must be > min!");

                ulong uRange = (ulong)(max - min);

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
