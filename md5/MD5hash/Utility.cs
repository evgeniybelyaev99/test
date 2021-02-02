using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MD5hash
{
    class Utility
    {
        public static BitArray StrToBitArray(string input)
        {
            // конвертировать сообщение в массив байт
            Encoding enc = Encoding.UTF8;
            byte[] inputByteArr = enc.GetBytes(input);

            // конвертировать сообщение в массив из бит (little-endian)
            BitArray inputBitArrayLE = new BitArray(inputByteArr);
            return inputBitArrayLE;
        }

        public static byte[] ConvertToByteArray(BitArray bits)
        {
            byte[] bytes = new byte[bits.Length / 8];
            bits.CopyTo(bytes, 0);
            return bytes;
        }       
    }
}
