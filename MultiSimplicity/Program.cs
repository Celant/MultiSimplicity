using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaToMultiplicity
{
    class Program
    {
        public static string InputFile = "";
        public static string OutputFolder = "";

        public static string[] FileContents;

        public static MarkdownConverter MarkdownConverter;
        public static ClassBuilder ClassBuilder;

        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-input":
                        InputFile = args[i + 1];
                        break;
                    case "-output":
                        OutputFolder = args[i + 1];
                        break;
                    default:
                        break;
                }
            }

            if (string.IsNullOrEmpty(InputFile) || string.IsNullOrEmpty(OutputFolder))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Usage: TerrariaToMultiplicity.exe -input [Markdown file] -output [Output folder]");
                Console.ForegroundColor = ConsoleColor.Gray;
                return;
            }

            Console.WriteLine("Input file: {0}", InputFile);
            Console.WriteLine("Output folder: {0}", OutputFolder);

            FileContents = ProcessFile(InputFile);

            MarkdownConverter = new MarkdownConverter(FileContents);
            ClassBuilder = new ClassBuilder();

            var packets = MarkdownConverter.ParseMarkdown();

            ClassBuilder.SetPackets(packets);
            ClassBuilder.WriteClasses(OutputFolder);

            Console.WriteLine();
            Console.WriteLine("Shit be done, press any key to exit.");
            Console.ReadKey();
        }

        public static string[] ProcessFile(string inputFile)
        {
            return File.ReadAllLines(inputFile);
        }
    }
}
