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

        public static bool ConsolePromptYesNoCancel(string reason)
        {
			ConsoleKey response;
			do
			{
				Console.Write(reason + " [y/n/c]");
				response = Console.ReadKey(false).Key;   // true is intercept key (dont show), false is show
				if (response != ConsoleKey.Enter)
					Console.WriteLine();

            } while (response != ConsoleKey.Y && response != ConsoleKey.N && response != ConsoleKey.C);

            if (response == ConsoleKey.C)
            {
                Console.WriteLine("Cancelling generation");
                Environment.Exit(0);
            }

			return response == ConsoleKey.Y;
        }
    }
}
