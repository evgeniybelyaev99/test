using System;
using System.Text;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;

namespace HashUnitTest
{
    [TestClass]
    public class HashUnitTest
    {
        [DataTestMethod]
        [DataRow("")]
        [DataRow("a")]
        [DataRow("abc")]
        [DataRow("message digest")]
        [DataRow("abcdefghijklmnopqrstuvwxyz")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")]
        [DataRow("12345678901234567890123456789012345678901234567890123456789012345678901234567890")]
        
        public void TestMethod(string input)
        {
            string output = MD5hash.Algorithm.HashText(input);
            string output0Expected = GetMD5Hash(input);
            Assert.AreEqual(output0Expected, output, true, "Некорректный хэш для строки {0}", input);
        }
        private string GetMD5Hash(string input)
        {
            MD5 md5Hasher = MD5.Create();
            // Преобразуем входную строку в массив байт и вычисляем хэш
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            // Создаем новый Stringbuilder (Изменяемую строку) для набора байт
            StringBuilder sBuilder = new StringBuilder();
            // Преобразуем каждый байт хэша в шестнадцатеричную строку
            for (int i = 0; i < data.Length; i++)
            {
                //указывает, что нужно преобразовать элемент в шестнадцатиричную строку длиной в два символа
                sBuilder.Append(data[i].ToString("x2"));
            }
            return (sBuilder.ToString());
        }
        
    }
}

