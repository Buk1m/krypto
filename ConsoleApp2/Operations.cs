using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp2
{
    public class Operations
    {
        public static void Permutate(ref List<BitArray> bits, byte[,] permutationMatrix)
        {
            int xLength = permutationMatrix.GetLength(0);
            int yLength = permutationMatrix.GetLength(1);

            for (var ind = 0; ind < bits.Count; ind++)
            {
                BitArray sourceBits = new BitArray(bits[ind]);
                bits[ind] = new BitArray(permutationMatrix.Length);
                for (int i = 0; i < xLength; i++)
                {
                    for (int j = 0; j < yLength; j++)
                    {
                        bits[ind][i * yLength + j] = sourceBits[permutationMatrix[i, j] - 1];
                    }
                }
            }
        }

        public static void Permutate(ref BitArray bits, byte[,] permutationMatrix)
        {
            int xLength = permutationMatrix.GetLength(0);
            int yLength = permutationMatrix.GetLength(1);

            BitArray sourceBits = bits;

            bits = new BitArray(permutationMatrix.Length);
            for (int i = 0; i < xLength; i++)
            {
                for (int j = 0; j < yLength; j++)
                {
                    bits[i * yLength + j] = sourceBits[permutationMatrix[i, j] - 1];
                }
            }
        }

        public static List<BitArray> SplitBitArrayInHalf(List<BitArray> bitArray)
        {
            List<BitArray> bits = new List<BitArray>();
            foreach (BitArray array in bitArray)
            {
                int blockSize = (array.Count / 2);
                BitArray leftBitsArray = new BitArray(blockSize);
                BitArray rightBitsArray = new BitArray(blockSize);
                for (int i = 0; i < blockSize; i++)
                {
                    leftBitsArray.Set(i, array[i]);
                    rightBitsArray.Set(i, array[blockSize + i]);
                }

                bits.Add(leftBitsArray);
                bits.Add(rightBitsArray);
            }

            return bits;
        }

        public static List<BitArray> SplitBitArrayInHalf(BitArray bitArray)
        {
            List<BitArray> bits = new List<BitArray>();
            int blockSize = (bitArray.Count / 2);
            BitArray leftBitsArray = new BitArray(blockSize);
            BitArray rightBitsArray = new BitArray(blockSize);
            for (int i = 0; i < blockSize; i++)
            {
                leftBitsArray.Set(i, bitArray[i]);
                rightBitsArray.Set(i, bitArray[blockSize + i]);
            }

            bits.Add(leftBitsArray);
            bits.Add(rightBitsArray);


            return bits;
        }

        public static BitArray GenerateRandomKey()
        {
            const int keyLengthInBytes = 8;
            Random random = new Random();
            BitArray key = new BitArray(64);
            int left = random.Next();
            int right = random.Next();
            byte[] leftBytes = BitConverter.GetBytes(left);
            byte[] rightBytes = BitConverter.GetBytes(right);
            byte[] keyBytes = leftBytes.Concat(rightBytes).ToArray();
            return new BitArray(keyBytes);
        }

        public static BitArray RemoveParityBitsFromKey(BitArray keyWithParity)
        {
            List<bool> keyValues = new List<bool>();
            for (int i = 0; i < keyWithParity.Length; i++)
            {
                if (i % 8 != 7)
                {
                    keyValues.Add(keyWithParity[i]);
                }
            }

            return new BitArray(keyValues.ToArray());
        }

        public static BitArray GenerateRandomKeyWithParityBits()
        {
            const int keyLengthInBytes = 8;
            Random random = new Random();
            byte[] key = new byte[keyLengthInBytes];
            int left = random.Next();
            int right = random.Next();
            byte[] leftBytes = BitConverter.GetBytes(left);
            byte[] rightBytes = BitConverter.GetBytes(right);
            key = leftBytes.Concat(rightBytes).ToArray();
            BitArray keyBits = new BitArray(key);
            int bitSum = 0;
            for (int i = 0; i < keyBits.Count; i++)
            {
                if (i % 8 == 7)
                {
                    if (bitSum % 2 == 0)
                    {
                        keyBits[i] = true;
                    }
                    else
                    {
                        keyBits[i] = false;
                    }

                    bitSum = 0;
                }
                else if (keyBits[i])
                {
                    bitSum++;
                }
            }

            return keyBits;
        }

        public static BitArray SBoxesTransformation(BitArray array)
        {
            BitArray bits = new BitArray(6);
            BitArray transformBitArray = new BitArray(32);
            for (int i = 0; i < array.Length / 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    bits[j] = array[i * 6 + j];
                }
                Reverse(ref bits);

                byte sValue = FindSBlockValue(i, bits);
                BitArray b = new BitArray(new byte[] {sValue});
           

                for (int j = 3; j >=0 ; j--)
                {
                   
                    transformBitArray.Set(i * 4 + 3-j, b[j]);
                }
            }

            return transformBitArray;
        }

        private static byte FindSBlockValue(int blockNumber, BitArray bitArray)
        {
            byte row = FindSBlockRow(bitArray);
            byte column = FindSBlockColumn(bitArray);
            return Constants.SBlocks[blockNumber, row, column];
        }

        private static byte FindSBlockRow(BitArray bitArray)
        {
            BitArray row = new BitArray(2);
            row.Set(0, bitArray[0]);
            row.Set(1, bitArray[5]);

            byte[] bytes = new byte[1];
            row.CopyTo(bytes, 0);
            return bytes[0];
        }

        private static byte FindSBlockColumn(BitArray bitArray)
        {
            BitArray column = new BitArray(4);
            for (int i = 1; i < 5; i++)
            {
                column.Set(i - 1, bitArray[i]);
            }

            byte[] bytes = new byte[1];
            column.CopyTo(bytes, 0);
            return bytes[0];
        }

        public static void Reverse(ref BitArray array)
        {
            int length = array.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
        }
    }
}