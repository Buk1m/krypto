using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp2
{
    public class FileReader
    {
        public byte[] ReadBytes(string filename = "clearText.txt")
        {
            try
            {
                return File.ReadAllBytes(filename);
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return null;
        }
    }
}