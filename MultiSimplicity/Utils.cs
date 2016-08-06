using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaToMultiplicity
{
    class Utils
    {
        public static void ColorWriteNewLine(ConsoleColor color, string s, params string[] args)
        {
            Console.WriteLine();
            ColorWrite(color, s, args);
        }

        public static void ColorWrite(ConsoleColor color, string s, params string[] args)
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            if (args.Length != 0)
            {
                s = string.Format(s, args);
            }

            Console.ForegroundColor = color;
            Console.WriteLine(s);
            Console.ForegroundColor = originalColor;
        }
    }
}
