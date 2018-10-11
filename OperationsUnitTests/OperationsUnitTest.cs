using System;
using System.Collections;
using System.Collections.Generic;
using ConsoleApp2;
using ConsoleApp2.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OperationsUnitTests
{
    [TestClass]
    public class PermutationUnitTests
    {
        [TestMethod]
        public void CheckPermutation()
        {
            BitArray initial64BitBlock = new BitArray(0).FromInt(0x0000_0000_FFFF_FFFF);
            List<BitArray> bitArrayList = new List<BitArray>();

            bitArrayList.Add(initial64BitBlock);

            BitArray expectedBitArray = new BitArray(0).FromInt(0xF0F0_F0F0_F0F0_F0F0);

            Operations.Permutate(ref bitArrayList, Constants.InitialPermutationMatrix);
            for (int i = 0; i < 64; i++)
            {
                Assert.AreEqual(expectedBitArray[i], bitArrayList[0][i]);
            }
        }

        [TestMethod]
        public void CheckCompressionPermutation()
        {
            BitArray initial64BitBlock = new BitArray(0).FromInt(0x0000_0000_FFFF_FFFF);
            List<BitArray> bitArrayList = new List<BitArray>();
            bitArrayList.Add(initial64BitBlock);

            BitArray expectedBitArray = new BitArray(0).FromInt(0xC000_44FF_FFFF, 6);

            Operations.Permutate(ref bitArrayList, Constants.CompressionPermutationMatrix);

            for (int i = 0; i < 48; i++)
            {
                Assert.AreEqual(expectedBitArray[i], bitArrayList[0][i]);
            }
        }

        [TestMethod]
        public void CheckExpandedPermutation()
        {
            BitArray initial32BitBlock = new BitArray(0).FromInt(0x0000_FFFF, 4);
            List<BitArray> bitArrayList = new List<BitArray>();
            bitArrayList.Add(initial32BitBlock);

            BitArray expectedBitArray = new BitArray(0).FromInt(0x8000_017F_BFFE, 6);
            Operations.Permutate(ref bitArrayList, Constants.ExpandedPermutationMatrix);
            for (int i = 0; i < 48; i++)
            {
                Assert.AreEqual(expectedBitArray[i], bitArrayList[0][i]);
            }
        }
    }

    [TestClass]
    public class SplitUnitTest
    {
        [TestMethod]
        public void SplitBitArrayInHalf()
        {
            List<BitArray> bitArrays = new List<BitArray>();
            bitArrays.Add(new BitArray(0).FromInt(0x0FFF_FFFF_0000_0000));
            bitArrays.Add(new BitArray(0).FromInt(0x0123_0000, 4));

            List<BitArray> expectedBitArrays = new List<BitArray>();
            expectedBitArrays.Add(new BitArray(0).FromInt(0x0FFFFFFF, 4));

            expectedBitArrays.Add(new BitArray(0).FromInt(0x00000000, 4));
            expectedBitArrays.Add(new BitArray(0).FromInt(0x0123, 2));

            expectedBitArrays.Add(new BitArray(0).FromInt(0x0000, 2));

            List<BitArray> splitBitArrays = Operations.SplitBitArrayInHalf(bitArrays);
            int[] expectedValues = new int[4];
            int[] splitValues = new int[4];
            for (int i = 0; i < splitBitArrays.Count; i++)
            {
                splitBitArrays[i].CopyTo(splitValues, i);
                expectedBitArrays[i].CopyTo(expectedValues, i);
            }

            CollectionAssert.AreEqual(splitValues, expectedValues);
        }
    }

    [TestClass]
    public class GenerateKeyUnitTest
    {
        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

        public static bool CheckParity(BitArray bitArray, int parityBit = 8)
        {
            int bitSum = 0;
            for (int i = 0; i < bitArray.Count; i++)
            {
                if (i % 8 == 7)
                {
                    if (bitArray[i] != (bitSum % 2 == 0))
                    {
                        return false;
                    }

                    bitSum = 0;
                }
                else if (bitArray[i])
                {
                    bitSum++;
                }
            }

            return true;
        }

        [TestMethod]
        public void CheckKeyGenerationWithParityBits()
        {
            BitArray generatedKey = Operations.GenerateRandomKeyWithParityBits();
            long keyValue = BitConverter.ToInt64(BitArrayToByteArray(generatedKey), 0);
            Assert.AreEqual(generatedKey.Length, 64);
            Assert.IsTrue(CheckParity(generatedKey));
        }


        [TestMethod]
        public void CheckGenerateKeySize()
        {
            Assert.AreEqual(Operations.GenerateRandomKey().Length, 64);
        }
    }

    [TestClass]
    public class SBoxesTransformationUnitTest
    {
        [TestMethod]
        public void CheckSBoxesTransformation()
        {
            BitArray bits = new BitArray(0).FromInt(0xFFFF_FFFF_FFFF, 6);
            var test = Operations.SBoxesTransformation(bits);
            //TODO: Finish this unit test 
        }
    }
}