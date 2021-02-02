using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MD5hash
{
    public class Algorithm
    {
        public static string HashText(string message)
        {
            // преобразование исходного сообщения в массив из бит
            // первый бит должен быть старшим (big-endian)
            BitArray msgLE = Utility.StrToBitArray(message);
            BitArray msgBE = msgLE.ToBigEndian();
            return HashBitArray(msgBE);
        }

        private static string HashBitArray(BitArray bitArrayBE)
        {
            int paddedLength;

            BitArray paddedMessage = ApplyPadding(bitArrayBE, out paddedLength);
            BitArray lowEndianArr = AppendLength(bitArrayBE, paddedLength, paddedMessage);
            string output = Digest(lowEndianArr);

            return output;
        }
        // Выравнивание потока
        private static BitArray ApplyPadding(BitArray message, out int paddedLength)
        {
            // найти следующее кратное 512
            int lenInBits = message.Length;
            paddedLength = lenInBits + (512 - lenInBits % 512);

            // вычесть 64 бита, они заполняются на шаге 2
            paddedLength -= 64;

            // убедиться, что 64 бита доступны для следующего кратного 512
            //пример: если количество бит равно 496, то следующий предел будет 512, 
            //512-496 = 16 свободных битов, но требуется 64 свободных бита
            if (paddedLength <= lenInBits)
            {
                paddedLength += 512;
            }

            // подготовить новый массив для хранения дополненного сообщения
            BitArray paddedMessage = new BitArray(paddedLength);

            // копирование исходного сообщение в новый массив
            int i = 0;
            for (; i < message.Length; i++)
            {
                paddedMessage[i] = message[i];
            }

            // добавить к сообщению "1" -бит
            paddedMessage[i] = true;
            // другие биты по умолчанию равны 0
            return paddedMessage;
        }

        // Добавление длины сообщения
        private static BitArray AppendLength(BitArray originalInput, int limit, BitArray paddedMessage)
        {
            // подготовить новый массив, который может содержать дополненное сообщение
            // и длину исходного сообщения в 64-битном представлении
            BitArray paddedMsgWLength = new BitArray(limit + 64);

            // копирование дополненного сообщения в новый массив
            int paddedMsgWLengthIndex = 0;
            for (; paddedMsgWLengthIndex < paddedMessage.Length; paddedMsgWLengthIndex++)
            {
                paddedMsgWLength[paddedMsgWLengthIndex] = paddedMessage[paddedMsgWLengthIndex];
            }


            // получить 64-битное представление длины исходного сообщения
            byte[] msgLengthBytes = BitConverter.GetBytes(originalInput.Length);
            BitArray msgLengthBitsLE = new BitArray(msgLengthBytes);

            BitArray msgLengthBE = new BitArray(msgLengthBitsLE.Length);
            for (int i = 0; i < msgLengthBitsLE.Length; i++)
            {
                bool bit = msgLengthBitsLE[i];
                msgLengthBE[msgLengthBE.Length - 1 - i] = bit;
            }
            //получить младшее 32-битное слово из длины сообщения(младшие 4 байта)
            BitArray lowOrderWord = new BitArray(32);
            for (int i = 0; i < 32; i++)
            {
                bool bitValue = false;

                if (msgLengthBE.Length - 1 - i >= 0)
                    bitValue = msgLengthBE[msgLengthBE.Length - 1 - i];


                lowOrderWord[lowOrderWord.Length - 1 - i] = bitValue;
            }

            // получить старшее 32-битное слово из длины сообщения(старшие 4 байта)
            BitArray highOrderWord = new BitArray(32);
            for (int i = 0; i < 32; i++)
            {
                bool bitValue = false;

                if (msgLengthBE.Length - 1 - i - 32 >= 0)
                    bitValue = msgLengthBE[msgLengthBE.Length - 1 - i - 32];


                highOrderWord[lowOrderWord.Length - 1 - i] = bitValue;
            }
            // инвертировать слова, последние 8 бит в слове теперь должны быть первыми и так далее
            BitArray lowOrderWordInv = lowOrderWord.InvertWordBitArray();
            BitArray highOrderWordInv = highOrderWord.InvertWordBitArray();

            // сначала добавить младшее 32-битное слово к дополненному сообщению
            for (int i = 0; paddedMsgWLengthIndex < paddedMessage.Length + 32; paddedMsgWLengthIndex++)
            {
                paddedMsgWLength[paddedMsgWLengthIndex] = lowOrderWordInv[i];
                i++;
            }

            // добавить старшее 32-битное слово к дополненному сообщению 
            for (int i = 0; paddedMsgWLengthIndex < paddedMessage.Length + 64; paddedMsgWLengthIndex++)
            {
                paddedMsgWLength[paddedMsgWLengthIndex] = highOrderWordInv[i];
                i++;
            }

            //конвертировать в little-endian
           
            BitArray paddedMsgWLengthLE = paddedMsgWLength.ToLittleEndian();


            // вернуть полностью заполненное сообщение с прямым порядком байт
            return paddedMsgWLengthLE;
        }


        private static string Digest(BitArray message)
        {

            // Для вычислений инициализируются четыре переменные размером по 32 бита, 
            //начальные значения которых задаются шестнадцатеричными числами (порядок байтов little-endian):
            uint A = 0x67452301;
            uint B = 0xefcdab89;
            uint C = 0x98badcfe;
            uint D = 0x10325476;



            //Обработка сообщения блоками по 16 слов
            for (int i = 0; i < message.Length; i += 512)
            {

                BitArray block = new BitArray(512);
                for (int k = 0; k < 512; k++)
                {
                    block[k] = message[i + k];
                }


                uint[] X = new uint[16]; // каждое поле имеет размер 1 байт
                byte[] blockByteArr = Utility.ConvertToByteArray(block);

                // Copy block i into X
                int xIndex = 0;
                for (int l = 0; l < blockByteArr.Length; l += 4)
                {
                    byte[] bytes = new byte[4];
                    bytes[0] = blockByteArr[l + 0];
                    bytes[1] = blockByteArr[l + 1];
                    bytes[2] = blockByteArr[l + 2];
                    bytes[3] = blockByteArr[l + 3];

                    X[xIndex] = BitConverter.ToUInt32(bytes, 0);
                    xIndex++;
                }


                // Сохраняются значения A, B, C и D, оставшиеся после операций над предыдущими блоками
                //(или их начальные значения, если блок первый).
                uint AA = A;
                uint BB = B;
                uint CC = C;
                uint DD = D;

                // Round 1 (16 операций)
                A = MD5Operations.Round1Op(A, B, C, D, 0, 7, 1, X);
                D = MD5Operations.Round1Op(D, A, B, C, 1, 12, 2, X);
                C = MD5Operations.Round1Op(C, D, A, B, 2, 17, 3, X);
                B = MD5Operations.Round1Op(B, C, D, A, 3, 22, 4, X);

                A = MD5Operations.Round1Op(A, B, C, D, 4, 7, 5, X);
                D = MD5Operations.Round1Op(D, A, B, C, 5, 12, 6, X);
                C = MD5Operations.Round1Op(C, D, A, B, 6, 17, 7, X);
                B = MD5Operations.Round1Op(B, C, D, A, 7, 22, 8, X);

                A = MD5Operations.Round1Op(A, B, C, D, 8, 7, 9, X);
                D = MD5Operations.Round1Op(D, A, B, C, 9, 12, 10, X);
                C = MD5Operations.Round1Op(C, D, A, B, 10, 17, 11, X);
                B = MD5Operations.Round1Op(B, C, D, A, 11, 22, 12, X);

                A = MD5Operations.Round1Op(A, B, C, D, 12, 7, 13, X);
                D = MD5Operations.Round1Op(D, A, B, C, 13, 12, 14, X);
                C = MD5Operations.Round1Op(C, D, A, B, 14, 17, 15, X);
                B = MD5Operations.Round1Op(B, C, D, A, 15, 22, 16, X);


                // Round 2 (16 операций)
                A = MD5Operations.Round2Op(A, B, C, D, 1, 5, 17, X);
                D = MD5Operations.Round2Op(D, A, B, C, 6, 9, 18, X);
                C = MD5Operations.Round2Op(C, D, A, B, 11, 14, 19, X);
                B = MD5Operations.Round2Op(B, C, D, A, 0, 20, 20, X);

                A = MD5Operations.Round2Op(A, B, C, D, 5, 5, 21, X);
                D = MD5Operations.Round2Op(D, A, B, C, 10, 9, 22, X);
                C = MD5Operations.Round2Op(C, D, A, B, 15, 14, 23, X);
                B = MD5Operations.Round2Op(B, C, D, A, 4, 20, 24, X);

                A = MD5Operations.Round2Op(A, B, C, D, 9, 5, 25, X);
                D = MD5Operations.Round2Op(D, A, B, C, 14, 9, 26, X);
                C = MD5Operations.Round2Op(C, D, A, B, 3, 14, 27, X);
                B = MD5Operations.Round2Op(B, C, D, A, 8, 20, 28, X);

                A = MD5Operations.Round2Op(A, B, C, D, 13, 5, 29, X);
                D = MD5Operations.Round2Op(D, A, B, C, 2, 9, 30, X);
                C = MD5Operations.Round2Op(C, D, A, B, 7, 14, 31, X);
                B = MD5Operations.Round2Op(B, C, D, A, 12, 20, 32, X);

                // Round 3 (16 операций)
                A = MD5Operations.Round3Op(A, B, C, D, 5, 4, 33, X);
                D = MD5Operations.Round3Op(D, A, B, C, 8, 11, 34, X);
                C = MD5Operations.Round3Op(C, D, A, B, 11, 16, 35, X);
                B = MD5Operations.Round3Op(B, C, D, A, 14, 23, 36, X);

                A = MD5Operations.Round3Op(A, B, C, D, 1, 4, 37, X);
                D = MD5Operations.Round3Op(D, A, B, C, 4, 11, 38, X);
                C = MD5Operations.Round3Op(C, D, A, B, 7, 16, 39, X);
                B = MD5Operations.Round3Op(B, C, D, A, 10, 23, 40, X);

                A = MD5Operations.Round3Op(A, B, C, D, 13, 4, 41, X);
                D = MD5Operations.Round3Op(D, A, B, C, 0, 11, 42, X);
                C = MD5Operations.Round3Op(C, D, A, B, 3, 16, 43, X);
                B = MD5Operations.Round3Op(B, C, D, A, 6, 23, 44, X);

                A = MD5Operations.Round3Op(A, B, C, D, 9, 4, 45, X);
                D = MD5Operations.Round3Op(D, A, B, C, 12, 11, 46, X);
                C = MD5Operations.Round3Op(C, D, A, B, 15, 16, 47, X);
                B = MD5Operations.Round3Op(B, C, D, A, 2, 23, 48, X);


                // Round 4 (16 операций)
                A = MD5Operations.Round4Op(A, B, C, D, 0, 6, 49, X);
                D = MD5Operations.Round4Op(D, A, B, C, 7, 10, 50, X);
                C = MD5Operations.Round4Op(C, D, A, B, 14, 15, 51, X);
                B = MD5Operations.Round4Op(B, C, D, A, 5, 21, 52, X);

                A = MD5Operations.Round4Op(A, B, C, D, 12, 6, 53, X);
                D = MD5Operations.Round4Op(D, A, B, C, 3, 10, 54, X);
                C = MD5Operations.Round4Op(C, D, A, B, 10, 15, 55, X);
                B = MD5Operations.Round4Op(B, C, D, A, 1, 21, 56, X);

                A = MD5Operations.Round4Op(A, B, C, D, 8, 6, 57, X);
                D = MD5Operations.Round4Op(D, A, B, C, 15, 10, 58, X);
                C = MD5Operations.Round4Op(C, D, A, B, 6, 15, 59, X);
                B = MD5Operations.Round4Op(B, C, D, A, 13, 21, 60, X);

                A = MD5Operations.Round4Op(A, B, C, D, 4, 6, 61, X);
                D = MD5Operations.Round4Op(D, A, B, C, 11, 10, 62, X);
                C = MD5Operations.Round4Op(C, D, A, B, 2, 15, 63, X);
                B = MD5Operations.Round4Op(B, C, D, A, 9, 21, 64, X);

                // Суммируем с результатом предыдущего цикла:
                A = A + AA;
                B = B + BB;
                C = C + CC;
                D = D + DD;

                
            }


            // Результат вычислений находится в буфере ABCD, это и есть хеш.
            //Если выводить побайтово, начиная с младшего байта A и заканчивая старшим байтом D, то мы получим MD5-хеш. 


            // конвертировать в массив байтов
            byte[] ABytes = BitConverter.GetBytes(A);
            byte[] BBytes = BitConverter.GetBytes(B);
            byte[] CBytes = BitConverter.GetBytes(C);
            byte[] DBytes = BitConverter.GetBytes(D);

            // reverse bytes
            byte[] reversedBytesA = new byte[ABytes.Length];
            byte[] reversedBytesB = new byte[BBytes.Length];
            byte[] reversedBytesC = new byte[CBytes.Length];
            byte[] reversedBytesD = new byte[DBytes.Length];

            for (int k = ABytes.Length - 1, l = 0; k >= 0; k--, l++)
                reversedBytesA[l] = ABytes[k];

            for (int k = BBytes.Length - 1, l = 0; k >= 0; k--, l++)
                reversedBytesB[l] = BBytes[k];

            for (int k = CBytes.Length - 1, l = 0; k >= 0; k--, l++)
                reversedBytesC[l] = CBytes[k];

            for (int k = DBytes.Length - 1, l = 0; k >= 0; k--, l++)
                reversedBytesD[l] = DBytes[k];

            
            var p1 = BitConverter.ToInt32(reversedBytesA, 0);
            var p2 = BitConverter.ToInt32(reversedBytesB, 0);
            var p3 = BitConverter.ToInt32(reversedBytesC, 0);
            var p4 = BitConverter.ToInt32(reversedBytesD, 0);
            var res = p1.ToString("X8") + p2.ToString("X8") + p3.ToString("X8") + p4.ToString("X8");
            res = res.ToLower();


            return res;
        }
    }
}
