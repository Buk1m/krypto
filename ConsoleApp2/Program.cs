using System;
using System.Collections;
using System.Collections.Generic;
using ConsoleApp2.ExtensionMethods;

namespace ConsoleApp2
{
    class Program
    {
        private const int left = 0;
        private const int right = 1;

        static void Main(string[] args)
        { 
            string data = @"Rozpierdoliłaś mi wakacje
Na stacji stoję stoję sam
x2

Być może miałaś jakieś racje 
Lecz cała miłość na marne
Miłość na marne

Kiedy pociąg wjeżdża na stację
pracownik kolejnictwa mi w oczy patrzy

Ma wyraz oczu taki jak ty
lecz ty rozpierdoliłaś mi wakacje

Cała miłość na marne

Rozpierdoliłaś mi wakacje
Cała miłość na marne
Miłość na marne

Jest tylko jedno światło dla mnie
Światło jest czerwone
Rozpierdoliłaś mi wakacje
Odjeżdżam w swoją stronę

I zanim znów się łudzić zacznę, wiem
Cała miłość na marne
Miłość na marne

Rozpierdoliłaś mi wakacje
Cała miłość na marne
Miłość na marne"; //Converters.BytesToString(temp);
            Console.WriteLine("MESSAGE:");
            Console.WriteLine(data);
            var key = new BitArray(0).FromInt(0x133457799BBCDFF1);
            string gmsg = Encrypt(data, key);
            Console.WriteLine("==========================================================================");
            Console.WriteLine("DECRYPTED:");
            Console.WriteLine(Decrypt(gmsg, key));

            Console.ReadKey();
        }


        static string Encrypt(string message, BitArray key)
        {
            List<BitArray> cypherBlocks = Converters.StringToBitArrays(message);
            Operations.Permutate(ref key, Constants.KeyPermutationMatrix);
            List<BitArray> keyHalves = Operations.SplitBitArrayInHalf(key);
            Operations.Permutate(ref cypherBlocks, Constants.InitialPermutationMatrix);
            Operations.SplitBitArrayInHalf(cypherBlocks, out List<BitArray> leftBlocks, out List<BitArray> rightBlocks);
            for (int i = 0; i < Constants.cycleNumber; i++)
            {
                var prevRightBlocks = new List<BitArray>(rightBlocks);
                keyHalves.ForEach(half => half.ShiftLeft(Constants.KeyBitShiftValues[i]));
                key = keyHalves[left].Append(keyHalves[right]);

                Operations.Permutate(ref key, Constants.CompressionPermutationMatrix);

                for (int j = 0; j < rightBlocks.Count; j++)
                {
                    BitArray block = rightBlocks[j];
                    Operations.Permutate(ref block, Constants.ExpandedPermutationMatrix);
                    block = block.Xor(key);
                    block = Operations.SBoxesTransformation(block);
                    Operations.Permutate(ref block, Constants.PBlockPermutationMatrix);
                    block = block.Xor(leftBlocks[j]);
                    rightBlocks[j] = block;
                }

                leftBlocks = prevRightBlocks;
            }

            List<BitArray> cypher = new List<BitArray>();
            BitArray connector = new BitArray(0);
            for (int i = 0; i < leftBlocks.Count + 1 / 2; i++)
            {
                connector = rightBlocks[i].Append(leftBlocks[i]);
                cypher.Add(connector);
            }

            Operations.Permutate(ref cypher, Constants.FinalPermutationMatrix);
            return Converters.BitArraysToString(cypher);
        }

        static string Decrypt(string message, BitArray key)
        {
            List<BitArray> cypherBlocks = Converters.StringToBitArrays(message);

            // Key Permutation 64->56
            Operations.Permutate(ref key, Constants.KeyPermutationMatrix);
            List<BitArray> keyHalves = Operations.SplitBitArrayInHalf(key);

            // Initial Permutation
            Operations.Permutate(ref cypherBlocks, Constants.InitialPermutationMatrix);
            // Split in blocks
            Operations.SplitBitArrayInHalf(cypherBlocks, out List<BitArray> leftBlocks, out List<BitArray> rightBlocks);

            for (int i = 0; i < 16; i++)
            {
                keyHalves.ForEach(half => half.ShiftLeft(Constants.KeyBitShiftValues[i]));
            }

            for (int i = Constants.cycleNumber - 1; i >= 0; i--)
            {
                var prevRightBlocks = new List<BitArray>(rightBlocks);
                if (i < 15)
                    keyHalves.ForEach(half => half.ShiftRight(Constants.KeyBitShiftValues[i + 1]));
                key = keyHalves[left].Append(keyHalves[right]);

                Operations.Permutate(ref key, Constants.CompressionPermutationMatrix);
                for (int j = 0; j < rightBlocks.Count; j++)
                {
                    BitArray block = rightBlocks[j];
                    Operations.Permutate(ref block, Constants.ExpandedPermutationMatrix);
                    block = block.Xor(key);
                    block = Operations.SBoxesTransformation(block);
                    Operations.Permutate(ref block, Constants.PBlockPermutationMatrix);
                    block = block.Xor(leftBlocks[j]);
                    rightBlocks[j] = block;
                }

                leftBlocks = prevRightBlocks;
            }

            List<BitArray> cypher = new List<BitArray>();
            for (int i = 0; i < leftBlocks.Count + 1 / 2; i++)
            {
                cypher.Add(rightBlocks[i].Append(leftBlocks[i]));
            }

            Operations.Permutate(ref cypher, Constants.FinalPermutationMatrix);
            return Converters.BitArraysToString(cypher);
        }
    }
}