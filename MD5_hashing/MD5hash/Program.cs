using System;

namespace MD5hash
{
    public class Program
    {
         static int Main(string[] args)
         {
            if (args.Length == 0)
            {
                Console.WriteLine("Входной аргумент не указан!");
                return 0;
            }
            string message = args[0];
            Console.WriteLine("Input: {0}", message);
            Console.WriteLine();
            string output = Algorithm.HashText(message);
            Console.WriteLine("MD5 (\"{0}\") = {1}", message, output);
            return 0;
         }
    }
}
