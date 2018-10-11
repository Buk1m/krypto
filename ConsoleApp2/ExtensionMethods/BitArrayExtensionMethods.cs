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
            byte[] bytes = BitConverter.GetBytes(number);
            BitArray bits = new BitArray(bytes.Take(bytesNumber).ToArray());
            bits.Reverse();
            return bits;
        }

        public static void Reverse(this BitArray bitArray)
        {
            int length = bitArray.Length;

            for (int i = 0; i < length / 2; i++)
            {
                bool bit = bitArray[i];
                bitArray[i] = bitArray[length - i - 1];
                bitArray[length - i - 1] = bit;
            }
        }

        public static BitArray ShiftLeft(this BitArray bitArray, int shiftsNumber)
        {
            for (int i = 0; i < shiftsNumber; i++)
            {
                bool firstBit = bitArray[0];
                for (int j = 1; j < bitArray.Count; j++)
                {
                    bitArray[j - 1] = bitArray[j];
                }

                bitArray[bitArray.Count - 1] = firstBit;
            }

            return bitArray;
        }

        public static BitArray ShiftRight(this BitArray bitArray, int shiftsNumber)
        {
            for (int j = 0; j < shiftsNumber; j++)
            {
                bool lastBit = bitArray[bitArray.Count - 1];
                for (int i = bitArray.Count - 1; i >= 1; i--)
                {
                    bitArray[i] = bitArray[i - 1];
                }
                bitArray[0] = lastBit;
            }
            return bitArray;
        }

        public static BitArray Append(this BitArray baseArray, BitArray appendArray)
        {
            bool[] mergedArray = new bool[baseArray.Count + appendArray.Count];
            baseArray.CopyTo(mergedArray, 0);
            appendArray.CopyTo(mergedArray, baseArray.Count);
            return new BitArray(mergedArray);
        }
    }
}