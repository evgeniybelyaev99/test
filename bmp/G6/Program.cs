using System;
using System.IO;
using System.Net;
using System.Text;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing;
using System.Collections.Specialized;

namespace G6 {
    class Program {

        static string read() { return Console.ReadLine().Trim(); }

        static void Main(string[] args) {
            // Путь к исходному файлу   
            string fileSrc = Environment.CurrentDirectory + "\\";
            if (args.Length >= 1) {
                fileSrc += args[0];
            }

            // Путь к выходному файлу
            string fileRes = Environment.CurrentDirectory + "\\";
            if (args.Length >= 2) {
                fileRes += args[1];
            }

            
            Bitmap bmp;

            // Чтение исход.изображения
            while (true) {
                // Проверяем расширение файла .bmp
                string[] fileSplit = fileSrc.Split('.');
                if (fileSrc.Contains(".") && fileSplit[fileSplit.Length - 1] == "bmp") {
                    // Пытаемся прочитать из входного файла
                    try
                    {
                        bmp = new Bitmap((Bitmap)Image.FromFile(fileSrc));
                        Console.WriteLine("Входной файл успешно прочтён!");
                        // Если удалось, выходим из бесконечного цикла
                        break;
                    }
                    
                    catch (Exception ex) {
                        Console.WriteLine("Произошла ошибка при чтении файла : " + ex.Message);
                    }
                }
                else {
                    Console.WriteLine("Исходный файл должен иметь расширение bmp!");
                }

                // Предлагаем повторить ввод или выйти из программы
                Console.WriteLine("Изменить имя файла? - Y");
                Console.WriteLine("Выход из программы? - Q");
                string rd = read();
                if (rd == "Y") {
                    Console.WriteLine("Новый путь:");
                    fileSrc = Environment.CurrentDirectory + "\\" + read();
                }
                else if (rd == "Q") {
                    return;
                }
            }

            // Обработка изображения
            int WHd = (bmp.Width * bmp.Height) / 10;
            int progress = 10;
            for (int x = 0; x < bmp.Width; x++) {
                for (int y = 0; y < bmp.Height; y++) {
                    int sum = (bmp.GetPixel(x, y).R + bmp.GetPixel(x, y).G + bmp.GetPixel(x, y).B);
                    int average = Math.Min(255, (sum + sum % 3) / 3);
                    bmp.SetPixel(x, y, Color.FromArgb(255, average, average, average));

                    if ((x * bmp.Height + y + 1) % WHd == 0) {
                        Console.WriteLine("Обработано " + progress + "% изображения");
                        progress += 10;
                    }
                }
            }

            // Сохранение изображения
            Console.WriteLine("Сохранение...");
            while (true) {
                // Проверяем расширение файла .bmp
                string[] fileSplit = fileRes.Split('.');
                if (fileRes.Contains(".") && fileSplit[fileSplit.Length - 1] == "bmp") {
                    // Пытаемся сохранить
                    try
                    {
                        bmp.Save(fileRes);
                        Console.WriteLine("Обработка успешно завершена!");
                        // Выходим, если успешно сохранили файл
                        break;
                    }
                    
                    catch (Exception ex) {
                        Console.WriteLine("Произошла ошибка : " + ex.Message);
                    }
                }
                else {
                    Console.WriteLine("Файл-результат должен иметь расширение bmp!");
                }

                // Предлагаем повторить ввод или выйти из программы
                Console.WriteLine("Изменить имя файла? - Y");
                Console.WriteLine("Выход из программы? - Q");
                string rd = read();
                if (rd == "Y") {
                    Console.WriteLine("Новый путь:");
                    fileRes = Environment.CurrentDirectory + "\\" + read();
                }
                else if (rd == "Q") {
                    return;
                }
            }

            Console.Write("Нажмите любую кнопку чтобы выйти...");
            Console.ReadKey();
        }
    }
}
