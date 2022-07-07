using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HashLib;

namespace SplitContainer_Test
{
    class FileSynData
    {
        public string FilePath { get; private set; }
        public string Sha3Checksum { get; private set; }

        public FileSynData(string filePath)
        {
            FilePath = filePath;

            var sha3_512 = HashFactory.Crypto.SHA3.CreateKeccak512();
            HashResult hashResult = sha3_512.ComputeBytes(File.ReadAllBytes(@filePath));
            Sha3Checksum = hashResult.ToString().ToLower().Replace("-", "");

            Console.WriteLine(Sha3Checksum);
           
        }
    }
}
