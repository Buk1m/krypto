using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConsoleApp2.ExtensionMethods;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            BitArray key = Operations.GenerateRandomKeyWithParityBits();
            string data = "8787878787878787";
            key = new BitArray(0).FromInt(0x133457799BBCDFF1);
            Operations.Reverse(ref key);
            string encryptedMessage = Encrypt(data, key);
            Operations.Reverse(ref key);
            string decryptedMessage = Decrypt(encryptedMessage, key);
            Console.WriteLine("Encrypted:");
            Console.WriteLine(encryptedMessage);
            Console.WriteLine("Decrypted:");
            Console.WriteLine(decryptedMessage);
            Console.ReadKey();
        }
        static string Encrypt(string message, BitArray key)
        {
            short cycleNumber = 16;
            List<byte> cypherInBytes = Converters.StringToBytes(message);;
            var data = new BitArray(0).FromInt(0x0123456789ABCDEF);
            Operations.Reverse(ref data);
            cypherInBytes = Converters.BitArrayToByteArray(data).ToList();
            List<BitArray> blocks = Converters.BytesTo64BitArrays(cypherInBytes);

            // Key Permutation 64->56
            Operations.Permutate(ref key, Constants.KeyPermutationMatrix);
            List<BitArray> keyHalves = Operations.SplitBitArrayInHalf(key);

            // Initial Permutation
            Operations.Permutate(ref blocks, Constants.InitialPermutationMatrix);

            // Split in blocks
            blocks = Operations.SplitBitArrayInHalf(blocks);
            List<BitArray> leftBlocks = blocks.Where((x, i) => (i % 2 == 0)).ToList();
            List<BitArray> rightBlocks = blocks.Where((x, i) => (i % 2 == 1)).ToList();

            for (int i = 0; i < cycleNumber; i++)
            {
                var prevRightBlocks = new List<BitArray>(rightBlocks);
                keyHalves.ForEach(half => half.ShiftLeft(Constants.KeyBitShiftValues[i]));
                key = keyHalves[0].Append(keyHalves[1]);

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
            for (int i = 0; i < leftBlocks.Count+1/2; i++)
            {

                connector = rightBlocks[i].Append(leftBlocks[i]);
                cypher.Add(connector);
                foreach (bool o in cypher[i])
                {
                    Console.Write(o ? 1 : 0);
                }

                Console.WriteLine();
            }

            Operations.Permutate(ref cypher, Constants.EndingPermutationMatrix);

            List<byte[]> decryptedBytes = new List<byte[]>();
            for (var i = 0; i < cypher.Count; i++)
            {
                                foreach (bool o in cypher[i])
                {
                    Console.Write(o ? 1 : 0);
                }
                decryptedBytes.Add(Converters.BitArrayToByteArray(cypher[i]));
            }

            Console.WriteLine();
            string encryptedMessage = "";

            foreach (var array in decryptedBytes)
            {
                try
                {
                    using (var fs = new FileStream("encrypted.dat", FileMode.Append, FileAccess.Write))
                    {
                        fs.Write(array, 0, array.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in process: {0}", ex);
                }

                encryptedMessage += Converters.BytesToString(array);
            }


            return encryptedMessage;
        }




















        static string Decrypt(string message, BitArray key)
        {
            short cycleNumber = 16;
            List<byte> cypherInBytes = Converters.StringToBytes(message); ;
            var data = Converters.BytesTo64BitArrays(cypherInBytes);
            data.ForEach( bytes => Operations.Reverse(ref bytes));
//            cypherInBytes = Converters.BitArrayToByteArray(data).ToList();
            List<BitArray> blocks = Converters.BytesTo64BitArrays(cypherInBytes);
            blocks = data;
            // Key Permutation 64->56
            Operations.Permutate(ref key, Constants.KeyPermutationMatrix);
            List<BitArray> keyHalves = Operations.SplitBitArrayInHalf(key);

            // Initial Permutation
            Operations.Permutate(ref blocks, Constants.InitialPermutationMatrix);

            // Split in blocks
            blocks = Operations.SplitBitArrayInHalf(blocks);
            List<BitArray> leftBlocks = blocks.Where((x, i) => (i % 2 == 0)).ToList();
            List<BitArray> rightBlocks = blocks.Where((x, i) => (i % 2 == 1)).ToList();

            for (int i = 0; i < cycleNumber; i++)
            {
                var prevRightBlocks = new List<BitArray>(rightBlocks);
                keyHalves.ForEach(half => half.ShiftRight(Constants.KeyBitShiftValues[i]));
                key = keyHalves[0].Append(keyHalves[1]);

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
            for (int i = 0; i < leftBlocks.Count; i++)
            {
                connector = rightBlocks[i].Append(leftBlocks[i]);
                cypher.Add(connector);

            }

            Operations.Permutate(ref cypher, Constants.EndingPermutationMatrix);

            List<byte[]> decryptedBytes = new List<byte[]>();
            for (var i = 0; i < cypher.Count; i++)
            {
                decryptedBytes.Add(Converters.BitArrayToByteArray(cypher[i]));
            }

            string encryptedMessage = "";

            foreach (var array in decryptedBytes)
            {
                try
                {
                    using (var fs = new FileStream("encrypted.dat", FileMode.Append, FileAccess.Write))
                    {
                        fs.Write(array, 0, array.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in process: {0}", ex);
                }

                encryptedMessage += Converters.BytesToString(array);
            }


            return encryptedMessage;

        }
    }
}