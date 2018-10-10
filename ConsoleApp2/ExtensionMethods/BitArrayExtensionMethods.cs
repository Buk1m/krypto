using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace ConsoleApp2.ExtensionMethods
{
    public static class BitArrayExtensionMethods
    {
        public static BitArray FromInt(this BitArray bitArray, ulong number, int bytesNumber = 8)
        {
            byte[] byteArray = BitConverter.GetBytes(number);
            return new BitArray(byteArray.Take(bytesNumber).ToArray());
        }

        public static BitArray ShiftLeft(this BitArray bitArray, int value)
        {
            for (int j = 0; j < value; j++)
            {
                var first = bitArray[0];
                for (int i = 1; i < bitArray.Count; i++)
                {
                    bitArray[i - 1] = bitArray[i];
                }

                bitArray[bitArray.Count - 1] = first;
            }

            return bitArray;
        }

        public static BitArray ShiftRight(this BitArray bitArray, int value)
        {
            for (int j = 0; j < value; j++)
            {
                var last = bitArray[bitArray.Count - 1];
                for (int i = bitArray.Count - 1; i > 1; i--)
                {
                    bitArray[i] = bitArray[i - 1];
                }

                bitArray[0] = last;
            }

            return bitArray;
        }

        public static BitArray Prepend(this BitArray current, BitArray before)
        {
            bool[] bools = new bool[current.Count + before.Count];
            before.CopyTo(bools, 0);
            current.CopyTo(bools, before.Count);
            return new BitArray(bools);
        }

        public static BitArray Append(this BitArray current, BitArray after)
        {
            bool[] bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }
    }
}