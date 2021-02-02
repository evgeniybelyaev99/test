using System.Collections;

namespace MD5hash
{
    public static class Extensions
    {
        public static BitArray ToLittleEndian(this BitArray bigEndianArr)
        {
            BitArray littleEndianArr = new BitArray(bigEndianArr.Length);
            for (int i = 0, newArrCounter = 0; newArrCounter < bigEndianArr.Length; i += 8)
            {
                for (int k = 7; k >= 0; k--)
                    littleEndianArr[newArrCounter++] = bigEndianArr[i + k];
            }

            return littleEndianArr;
        }

        public static BitArray ToBigEndian(this BitArray littleEndianArr)
        {
            BitArray bigEndianArr = new BitArray(littleEndianArr.Length);
            for (int i = 0, bitArrayBECounter = 0; bitArrayBECounter < littleEndianArr.Length; i += 8)
            {
                for (int k = 7; k >= 0; k--)
                    bigEndianArr[bitArrayBECounter++] = littleEndianArr[i + k];
            }

            return bigEndianArr;
        }


        public static BitArray InvertWordBitArray(this BitArray bitArray)
        {
            // создать новый массив для хранения 32 бит (4 байта)
            BitArray invertedArr = new BitArray(32);

            int invArrCounter = 0;
            for (int l = 0; l < 4; l++)
            {
                int passedBits = l * 8;
                for (int i = 8 + passedBits; i > passedBits; i--)
                {
                    invertedArr[invArrCounter] = bitArray[bitArray.Length - i];
                    invArrCounter++;
                }

            }

            return invertedArr;
        }
    }
}
