using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkQ.CommandLine.Demo
{
    class Program
    {
        static void Main(string[] args)
        {

            Parser parser = new Parser("Utility to zip/unzip files");
            parser.RegisterCommand(typeof(ZipCommand));
            parser.RegisterCommand(typeof(QuitCommand));
            parser.RegisterCommand(typeof(UnzipCommand));
            parser.IsFirstWordCommand = true;
            while (true)
            {
                Console.Write("Enter Command in the line below: \n");
                string input = Console.ReadLine();
                parser.CommandKeyword = CommandOption.Opt("action", CommandOption.CommandOptionType.String, false);
                CommandParseResult result = parser.ParseCommand(input.Split(" ".ToCharArray()));
                if (result.ErrorOccurred)
                {
                    Console.WriteLine(result.ErrorMessage);
                }
                else
                {
                    result.Command.Execute();
                }
            }
        }
    }

}
